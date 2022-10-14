// <copyright file="PhiFailureDetector.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Clocks;

namespace PhiFailureDetector
{
    public class PhiFailureDetector
    {
        private readonly LongIntervalHistory arrivalWindow;
        private readonly long initialHeartbeatInterval;
        private readonly IStopwatchProvider<long> stopwatchProvider;
        private readonly PhiFunc phiFunc;

        private long last;

        public PhiFailureDetector(
            int capacity,
            long initialHeartbeatInterval,
            IStopwatchProvider<long> stopwatchProvider,
            PhiFunc phiFunc)
        {
            this.arrivalWindow = new LongIntervalHistory(capacity);
            this.initialHeartbeatInterval = initialHeartbeatInterval;
            this.stopwatchProvider = stopwatchProvider;
            this.phiFunc = phiFunc;
        }

        public delegate double PhiFunc(long timestamp, long lastTimestamp, IWithStatistics statistics);

        /**
         * https://issues.apache.org/jira/browse/CASSANDRA-2597
         * Regular message transmissions experiencing typical random jitter will follow a normal distribution,
         * but since gossip messages from endpoint A to endpoint B are sent at random intervals,
         * they likely make up a Poisson process, making the exponential distribution appropriate.
         *
         * P_later(t) = 1 - F(t)
         * P_later(t) = 1 - (1 - e^(-Lt))
         *
         * The maximum likelihood estimation for the rate parameter L is given by 1/mean
         *
         * P_later(t) = 1 - (1 - e^(-t/mean))
         * P_later(t) = e^(-t/mean)
         * phi(t) = -log10(P_later(t))
         * phi(t) = -log10(e^(-t/mean))
         * phi(t) = -log(e^(-t/mean)) / log(10)
         * phi(t) = (t/mean) / log(10)
         * phi(t) = 0.4342945 * t/mean
         */
        public static double Exponential(long nowTimestamp, long lastTimestamp, IWithStatistics statistics)
        {
            var duration = nowTimestamp - lastTimestamp;
            return duration / statistics.Avg;
        }

        /**
         * https://github.com/akka/akka/blob/master/akka-remote/src/main/scala/akka/remote/PhiAccrualFailureDetector.scala
         * Calculation of phi, derived from the Cumulative distribution function for
         * N(mean, stdDeviation) normal distribution, given by
         * 1.0 / (1.0 + math.exp(-y * (1.5976 + 0.070566 * y * y)))
         * where y = (x - mean) / standard_deviation
         * This is an approximation defined in β Mathematics Handbook (Logistic approximation).
         * Error is 0.00014 at +- 3.16
         * The calculated value is equivalent to -log10(1 - CDF(y))
         */
        public static double Normal(long nowTimestamp, long lastTimestamp, IWithStatistics statistics)
        {
            var duration = nowTimestamp - lastTimestamp;
            var y = (duration - statistics.Avg) / statistics.StdDeviation;
            var exp = Math.Exp(-y * (1.5976 + (0.070566 * y * y)));
            if (duration > statistics.Avg)
            {
                return -Math.Log10(exp / (1 + exp));
            }
            else
            {
                return -Math.Log10(1 - (1 / (1 + exp)));
            }
        }

        public double Phi()
        {
            return this.phiFunc(this.stopwatchProvider.GetTimestamp(), this.last, this.arrivalWindow);
        }

        public void Report()
        {
            var now = this.stopwatchProvider.GetTimestamp();
            this.last = now;

            if (this.arrivalWindow.Count == 0)
            {
                this.arrivalWindow.Enqueue(this.initialHeartbeatInterval);
            }
            else
            {
                var interval = now - this.last;
                this.arrivalWindow.Enqueue(interval);
            }
        }
    }
}
