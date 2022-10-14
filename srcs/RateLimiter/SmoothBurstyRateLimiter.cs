// <copyright file="SmoothBurstyRateLimiter.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Clocks;

namespace RateLimiter
{
    internal sealed class SmoothBurstyRateLimiter : SmoothRateLimiter
    {
        private readonly double maxBurstSeconds;

        public SmoothBurstyRateLimiter(
            IStopwatchProvider<long> stopwatchProvider,
            double maxBurstSeconds)
#if !NET20
            : this(stopwatchProvider, maxBurstSeconds, null)
        {
        }

        internal SmoothBurstyRateLimiter(
            IStopwatchProvider<long> stopwatchProvider,
            double maxBurstSeconds,
            IAsyncBlocker asyncBlocker)
            : base(stopwatchProvider, asyncBlocker)
#else
            : base(stopwatchProvider)
#endif
        {
            this.maxBurstSeconds = maxBurstSeconds;
        }

        protected override TimeSpan CoolDownInterval => this.stableInterval;

        protected override void DoSetRate(double permitsPerSecond, TimeSpan stableInterval)
        {
            var oldMaxPermits = this.maxPermits;
            this.maxPermits = this.maxBurstSeconds * permitsPerSecond;
            if (double.IsPositiveInfinity(oldMaxPermits))
            {
                this.storedPermits = this.maxPermits;
            }
            else
            {
                this.storedPermits = (oldMaxPermits == 0) ? 0 : this.storedPermits * this.maxPermits / oldMaxPermits;
            }
        }

        protected override TimeSpan StoredPermitsToWaitTime(double storedPermits, double permitsToTake)
        {
            return TimeSpan.Zero;
        }
    }
}
