// Copyright (c) 2022 Zhang Shuai<zhangshuai.ustc@gmail.com>.
// All rights reserved.
//
// This file is part of OneDotNet.
//
// OneDotNet is free software: you can redistribute it and/or modify it under
// the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// OneDotNet is distributed in the hope that it will be useful, but WITHOUT ANY
// WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
// details.
//
// You should have received a copy of the GNU General Public License along with
// OneDotNet. If not, see <https://www.gnu.org/licenses/>.

using System;
using Clocks;

namespace RateLimiter
{
    internal sealed class SmoothWarmingUpRateLimiter : SmoothRateLimiter
    {
        private readonly TimeSpan warmupPeriod;

        private readonly double coldFactor;

        private double thresholdPermits;

        private TimeSpan slope;

        public SmoothWarmingUpRateLimiter(
            IStopwatchProvider<long> stopwatchProvider,
            TimeSpan warmupPeriod,
            double coldFactor)
#if !NET20
            : this(stopwatchProvider, warmupPeriod, coldFactor, null)
        {
        }

        internal SmoothWarmingUpRateLimiter(
            IStopwatchProvider<long> stopwatchProvider,
            TimeSpan warmupPeriod,
            double coldFactor,
            IAsyncBlocker asyncBlocker)
            : base(stopwatchProvider, asyncBlocker)
#else
            : base(stopwatchProvider)
#endif
        {
            this.warmupPeriod = warmupPeriod;
            this.coldFactor = coldFactor;
        }

        protected override TimeSpan CoolDownInterval => this.warmupPeriod.Divide(this.maxPermits);

        protected override void DoSetRate(double permitsPerSecond, TimeSpan stableInterval)
        {
            var oldMaxPermits = this.maxPermits;
            var coldInterval = stableInterval.Multiply(this.coldFactor);
            this.thresholdPermits = 0.5 * this.warmupPeriod.Ticks / stableInterval.Ticks;
            this.maxPermits = this.thresholdPermits + (2.0 * this.warmupPeriod.Ticks / (stableInterval + coldInterval).Ticks);
            this.slope = (coldInterval - stableInterval).Divide(this.maxPermits - this.thresholdPermits);
            if (oldMaxPermits == double.PositiveInfinity)
            {
                this.storedPermits = 0;
            }
            else
            {
                this.storedPermits = (oldMaxPermits == 0) ? this.maxPermits : (this.storedPermits * this.maxPermits / oldMaxPermits);
            }
        }

        protected override TimeSpan StoredPermitsToWaitTime(double storedPermits, double permitsToTake)
        {
            var availablePermitsAboveThreshold = storedPermits - this.thresholdPermits;
            var returnValue = TimeSpan.Zero;
            if (availablePermitsAboveThreshold > 0)
            {
                var permitsAboveThresholdToTake = Math.Min(availablePermitsAboveThreshold, permitsToTake);
                var length = this.PermitsToTime(availablePermitsAboveThreshold)
                    + this.PermitsToTime(availablePermitsAboveThreshold - permitsAboveThresholdToTake);
                returnValue = length.Multiply(permitsAboveThresholdToTake / 2.0);
                permitsToTake -= permitsAboveThresholdToTake;
            }

            returnValue += this.stableInterval.Multiply(permitsToTake);
            return returnValue;
        }

        private TimeSpan PermitsToTime(double permits)
        {
            return this.stableInterval + this.slope.Multiply(permits);
        }
    }
}
