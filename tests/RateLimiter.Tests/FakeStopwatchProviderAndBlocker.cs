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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Clocks;

namespace RateLimiter.Tests
{
    internal class FakeStopwatchProviderAndBlocker : IStopwatchProvider<long>, IAsyncBlocker
    {
        private long instant = 0;

        public bool IsHighResolution => throw new NotImplementedException();

        internal IList<long> Events { get; } = new List<long>();

        public long GetTimestamp()
        {
            return this.instant;
        }

        public TimeSpan ParseDuration(long from, long to)
        {
            return TimeSpan.FromTicks(to - from);
        }

        public long GetNextTimestamp(long from, TimeSpan interval)
        {
            return from + interval.Ticks;
        }

        public Task WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            this.instant += timeout.Ticks;
            this.Events.Add(timeout.Ticks);
            return Task.FromResult<object>(null);
        }

        public IStopwatch Create()
        {
            throw new NotImplementedException();
        }

        public IStopwatch StartNew()
        {
            throw new NotImplementedException();
        }
    }
}
