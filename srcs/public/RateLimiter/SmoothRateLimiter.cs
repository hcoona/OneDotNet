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
    public abstract class SmoothRateLimiter : RateLimiterBase
    {
        protected double storedPermits;

        protected double maxPermits;

        protected TimeSpan stableInterval;

        private long nextFreeTicketTimestamp;

#if !NET20
        internal SmoothRateLimiter(IStopwatchProvider<long> stopwatchProvider, IAsyncBlocker asyncBlocker)
            : base(stopwatchProvider, asyncBlocker)
        {
        }

        protected SmoothRateLimiter(IStopwatchProvider<long> stopwatchProvider)
            : this(stopwatchProvider, null)
        {
        }
#else
        protected SmoothRateLimiter(IStopwatchProvider<long> stopwatchProvider)
            : base(stopwatchProvider)
        {
        }
#endif

        protected abstract TimeSpan CoolDownInterval { get; }

        protected sealed override void DoSetRate(double permitsPerSecond, long nowTimestamp)
        {
            this.Resync(nowTimestamp);
            var stableIntervalSeconds = 1 / permitsPerSecond;
            var stableInterval = stableIntervalSeconds < TimeSpan.MaxValue.TotalSeconds
                ? TimeSpan.FromSeconds(1 / permitsPerSecond)
                : TimeSpan.MaxValue;
            this.stableInterval = stableInterval;
            this.DoSetRate(permitsPerSecond, stableInterval);
        }

        protected sealed override double DoGetRate()
        {
            return 1 / this.stableInterval.TotalSeconds;
        }

        protected sealed override long QueryEarliestAvailable(long nowTimestamp)
        {
            return this.nextFreeTicketTimestamp;
        }

        protected sealed override long ReserveEarliestAvailable(int permits, long nowTimestamp)
        {
            this.Resync(nowTimestamp);
            var returnValue = this.nextFreeTicketTimestamp;
            var storedPermitsToSpend = Math.Min(permits, this.storedPermits);
            var freshPermits = permits - storedPermitsToSpend;
            var waitTimeout = this.StoredPermitsToWaitTime(this.storedPermits, storedPermitsToSpend)
                + this.FreshPermitsToWaitTime(freshPermits);
            this.nextFreeTicketTimestamp = this.StopwatchProvider.GetNextTimestamp(this.nextFreeTicketTimestamp, waitTimeout);
            this.storedPermits -= storedPermitsToSpend;
            return returnValue;
        }

        protected abstract TimeSpan StoredPermitsToWaitTime(double storedPermits, double permitsToTake);

        protected void Resync(long nowTimestamp)
        {
            if (nowTimestamp > this.nextFreeTicketTimestamp)
            {
                var newDuration = this.StopwatchProvider.ParseDuration(this.nextFreeTicketTimestamp, nowTimestamp);
                double newPermits = newDuration.Ticks / (double)this.CoolDownInterval.Ticks;
                this.storedPermits = Math.Min(this.maxPermits, this.storedPermits + newPermits);
                this.nextFreeTicketTimestamp = nowTimestamp;
            }
        }

        protected abstract void DoSetRate(double permitsPerSecond, TimeSpan stableInterval);

        private TimeSpan FreshPermitsToWaitTime(double permits)
        {
            return this.stableInterval.Multiply(permits);
        }
    }
}
