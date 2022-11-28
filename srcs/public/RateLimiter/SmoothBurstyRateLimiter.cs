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
                this.storedPermits = (oldMaxPermits == 0)
                    ? 0
                    : this.storedPermits * this.maxPermits / oldMaxPermits;
            }
        }

        protected override TimeSpan StoredPermitsToWaitTime(
            double storedPermits, double permitsToTake)
        {
            return TimeSpan.Zero;
        }
    }
}
