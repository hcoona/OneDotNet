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
    public static class RateLimiter
    {
        public static IRateLimiter CreateBursty(
            double permitsPerSecond,
            double maxBurstSeconds,
            IStopwatchProvider<long> stopwatchProvider)
        {
            return new SmoothBurstyRateLimiter(stopwatchProvider, maxBurstSeconds)
            {
                PermitsPerSecond = permitsPerSecond,
            };
        }

        public static IRateLimiter CreateWarmingUp(
            double permitsPerSecond,
            TimeSpan warmupPeriod,
            double coldFactor,
            IStopwatchProvider<long> stopwatchProvider)
        {
            return new SmoothWarmingUpRateLimiter(stopwatchProvider, warmupPeriod, coldFactor)
            {
                PermitsPerSecond = permitsPerSecond,
            };
        }

        public static IRateLimiter Create(
            double permitsPerSecond,
            IStopwatchProvider<long> stopwatchProvider)
        {
            return CreateBursty(permitsPerSecond, 1, stopwatchProvider);
        }

        public static IRateLimiter Create(
            double permitsPerSecond,
            TimeSpan warmupPeriod,
            IStopwatchProvider<long> stopwatchProvider)
        {
            return CreateWarmingUp(permitsPerSecond, warmupPeriod, 3, stopwatchProvider);
        }
    }
}
