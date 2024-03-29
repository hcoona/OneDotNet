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
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RateLimiter.Tests
{
    public class RateLimiterTest
    {
        private readonly FakeStopwatchProviderAndBlocker stopwatchProviderAndBlocker = new();

        [Fact]
        public void TestSimple()
        {
            var limiter = this.Create(5, 1);
            limiter.Acquire(); // R0.00, since it's the first request
            limiter.Acquire(); // R0.20
            limiter.Acquire(); // R0.20
            Assert.Equal(
                new[]
                {
                    0L, TimeSpan.FromSeconds(0.2).Ticks, TimeSpan.FromSeconds(0.2).Ticks,
                },
                this.stopwatchProviderAndBlocker.Events);
        }

        [Fact]
        public void TestImmediateTryAcquire()
        {
            var limiter = this.Create(1);
            Assert.True(limiter.TryAcquire().Succeed, "Unable to acquire initial permit");
            Assert.False(limiter.TryAcquire().Succeed, "Capable of acquiring secondary permit");
        }

        [Fact]
        public async Task TestDoubleMinValueCanAcquireExactlyOnceAsync()
        {
            var r = this.Create(double.Epsilon);
            Assert.True(r.TryAcquire().Succeed, "Unable to acquire initial permit");
            Assert.False(r.TryAcquire().Succeed, "Capable of acquiring an additional permit");
            await this.stopwatchProviderAndBlocker
                .WaitAsync(TimeSpan.MaxValue.Subtract(TimeSpan.FromTicks(1)), CancellationToken.None);
            Assert.False(r.TryAcquire().Succeed, "Capable of acquiring an additional permit after sleeping");
        }

        [Fact]
        public void TestSimpleRateUpdate()
        {
            var limiter = this.Create(5.0, TimeSpan.FromSeconds(5));
            Assert.Equal(5.0, limiter.PermitsPerSecond);
            limiter.PermitsPerSecond = 10.0;
            Assert.Equal(10.0, limiter.PermitsPerSecond);

            Assert.Throws<ArgumentOutOfRangeException>(() => limiter.PermitsPerSecond = 0);
            Assert.Throws<ArgumentOutOfRangeException>(() => limiter.PermitsPerSecond = -10);
        }

        internal IRateLimiter Create(double permitsPerSecond)
        {
            return this.Create(permitsPerSecond, 1.0);
        }

        internal IRateLimiter Create(double permitsPerSecond, double maxBurstSeconds)
        {
            return new SmoothBurstyRateLimiter(this.stopwatchProviderAndBlocker, maxBurstSeconds, this.stopwatchProviderAndBlocker)
            {
                PermitsPerSecond = permitsPerSecond,
            };
        }

        internal IRateLimiter Create(double permitsPerSecond, TimeSpan warmupPeriod)
        {
            return new SmoothWarmingUpRateLimiter(this.stopwatchProviderAndBlocker, warmupPeriod, 3, this.stopwatchProviderAndBlocker)
            {
                PermitsPerSecond = permitsPerSecond,
            };
        }
    }
}
