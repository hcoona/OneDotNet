// <copyright file="FakeStopwatchProviderAndBlocker.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

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
