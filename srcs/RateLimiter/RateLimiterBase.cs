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
using System.Diagnostics;
using System.Threading;
#if !NET20
using System.Threading.Tasks;
#endif
using Clocks;

namespace RateLimiter
{
    public abstract class RateLimiterBase : IRateLimiter
    {
        internal readonly IStopwatchProvider<long> StopwatchProvider;
#if NET20
        private readonly object lockObject = new object();
#else
        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly IAsyncBlocker asyncBlocker;
#endif

#if !NET20
        internal RateLimiterBase(
            IStopwatchProvider<long> stopwatchProvider,
            IAsyncBlocker asyncBlocker)
        {
            this.asyncBlocker = asyncBlocker ?? TaskDelayAsyncBlocker.Instance;
            this.StopwatchProvider = stopwatchProvider ?? throw new ArgumentNullException(nameof(stopwatchProvider));
        }

        protected RateLimiterBase(IStopwatchProvider<long> stopwatchProvider)
            : this(stopwatchProvider, null)
        {
        }
#else
        protected RateLimiterBase(IStopwatchProvider<long> stopwatchProvider)
        {
            this.StopwatchProvider = stopwatchProvider ?? throw new ArgumentNullException(nameof(stopwatchProvider));
        }
#endif

        public double PermitsPerSecond
        {
            get
            {
#if NET20
                Monitor.Enter(lockObject);
#else
                this.semaphoreSlim.Wait();
#endif
                try
                {
                    return this.DoGetRate();
                }
                finally
                {
#if NET20
                    Monitor.Exit(lockObject);
#else
                    this.semaphoreSlim.Release();
#endif
                }
            }

            set
            {
                if (!(value > 0 && !double.IsNaN(value)))
                {
                    throw new ArgumentOutOfRangeException(nameof(this.PermitsPerSecond));
                }

#if NET20
                Monitor.Enter(lockObject);
#else
                this.semaphoreSlim.Wait();
#endif
                try
                {
                    this.DoSetRate(value, this.StopwatchProvider.GetTimestamp());
                }
                finally
                {
#if NET20
                    Monitor.Exit(lockObject);
#else
                    this.semaphoreSlim.Release();
#endif
                }
            }
        }

        [DebuggerStepThrough]
        public TimeSpan Acquire()
        {
            return this.Acquire(1);
        }

#if !NET20
        [DebuggerStepThrough]
        public Task<TimeSpan> AcquireAsync()
        {
            return this.AcquireAsync(CancellationToken.None);
        }

        [DebuggerStepThrough]
        public Task<TimeSpan> AcquireAsync(CancellationToken cancellationToken)
        {
            return this.AcquireAsync(1, cancellationToken);
        }
#endif

#if !NET20
        [DebuggerStepThrough]
#endif
        public TimeSpan Acquire(int permits)
#if !NET20
        {
            return this.AcquireAsync(permits).GetAwaiter().GetResult();
        }

        [DebuggerStepThrough]
        public Task<TimeSpan> AcquireAsync(int permits)
        {
            return this.AcquireAsync(permits, CancellationToken.None);
        }

        public async Task<TimeSpan> AcquireAsync(int permits, CancellationToken cancellationToken)
        {
            var waitTimeout = await this.ReserveAsync(permits, cancellationToken);
            await this.asyncBlocker.WaitAsync(waitTimeout, cancellationToken);
            return waitTimeout;
        }
#else
        {
            var waitTimeout = Reserve(permits);
            Thread.Sleep(waitTimeout);
            return waitTimeout;
        }
#endif

        [DebuggerStepThrough]
        public TryAcquireResult TryAcquire()
        {
            return this.TryAcquire(1, TimeSpan.Zero);
        }

#if !NET20
        [DebuggerStepThrough]
        public Task<TryAcquireResult> TryAcquireAsync()
        {
            return this.TryAcquireAsync(CancellationToken.None);
        }

        [DebuggerStepThrough]
        public Task<TryAcquireResult> TryAcquireAsync(CancellationToken cancellationToken)
        {
            return this.TryAcquireAsync(1, TimeSpan.Zero, cancellationToken);
        }
#endif

        [DebuggerStepThrough]
        public TryAcquireResult TryAcquire(int permits)
        {
            return this.TryAcquire(permits, TimeSpan.Zero);
        }

#if !NET20
        [DebuggerStepThrough]
        public Task<TryAcquireResult> TryAcquireAsync(int permits)
        {
            return this.TryAcquireAsync(permits, CancellationToken.None);
        }

        [DebuggerStepThrough]
        public Task<TryAcquireResult> TryAcquireAsync(int permits, CancellationToken cancellationToken)
        {
            return this.TryAcquireAsync(permits, TimeSpan.Zero, cancellationToken);
        }
#endif

        [DebuggerStepThrough]
        public TryAcquireResult TryAcquire(TimeSpan timeout)
        {
            return this.TryAcquire(1, timeout);
        }

#if !NET20
        [DebuggerStepThrough]
        public Task<TryAcquireResult> TryAcquireAsync(TimeSpan timeout)
        {
            return this.TryAcquireAsync(timeout, CancellationToken.None);
        }

        [DebuggerStepThrough]
        public Task<TryAcquireResult> TryAcquireAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            return this.TryAcquireAsync(1, timeout, cancellationToken);
        }
#endif

#if !NET20
        [DebuggerStepThrough]
#endif
        public TryAcquireResult TryAcquire(int permits, TimeSpan timeout)
#if !NET20
        {
            return this.TryAcquireAsync(permits, timeout, CancellationToken.None).GetAwaiter().GetResult();
        }

        [DebuggerStepThrough]
        public Task<TryAcquireResult> TryAcquireAsync(int permits, TimeSpan timeout)
        {
            return this.TryAcquireAsync(permits, timeout, CancellationToken.None);
        }

        public async Task<TryAcquireResult> TryAcquireAsync(int permits, TimeSpan timeout, CancellationToken cancellationToken)
#endif
        {
            if (!(permits > 0))
            {
                throw new ArgumentOutOfRangeException(nameof(permits));
            }

            TimeSpan waitTimeout;
#if NET20
            Monitor.Enter(lockObject);
#else
            await this.semaphoreSlim.WaitAsync(cancellationToken);
#endif
            try
            {
                var nowTimestamp = this.StopwatchProvider.GetTimestamp();
                if (!this.CanAcquire(nowTimestamp, timeout, out var momentAvailableInterval))
                {
                    return new TryAcquireResult
                    {
                        Succeed = false,
                        MomentAvailableInterval = momentAvailableInterval,
                    };
                }
                else
                {
                    waitTimeout = this.ReserveAndGetWaitLength(permits, nowTimestamp);
                }
            }
            finally
            {
#if NET20
                Monitor.Exit(lockObject);
#else
                this.semaphoreSlim.Release();
#endif
            }

#if NET20
            Thread.Sleep(waitTimeout);
#else
            await this.asyncBlocker.WaitAsync(waitTimeout, cancellationToken);
#endif
            return new TryAcquireResult
            {
                Succeed = true,
                MomentAvailableInterval = TimeSpan.Zero,
            };
        }

#if !NET20
        [DebuggerStepThrough]
#endif
        public TimeSpan Reserve(int permits)
#if !NET20
        {
            return this.ReserveAsync(permits).GetAwaiter().GetResult();
        }

        [DebuggerStepThrough]
        public Task<TimeSpan> ReserveAsync(int permits)
        {
            return this.ReserveAsync(permits, CancellationToken.None);
        }

        public async Task<TimeSpan> ReserveAsync(int permits, CancellationToken cancellationToken)
#endif
        {
            if (!(permits > 0))
            {
                throw new ArgumentOutOfRangeException(nameof(permits));
            }

#if NET20
            Monitor.Enter(lockObject);
#else
            await this.semaphoreSlim.WaitAsync(cancellationToken);
#endif
            try
            {
                return this.ReserveAndGetWaitLength(permits, this.StopwatchProvider.GetTimestamp());
            }
            finally
            {
#if NET20
                Monitor.Exit(lockObject);
#else
                this.semaphoreSlim.Release();
#endif
            }
        }

        protected abstract double DoGetRate();

        protected abstract void DoSetRate(double permitsPerSecond, long nowTimestamp);

        protected TimeSpan ReserveAndGetWaitLength(int permits, long nowTimestamp)
        {
            var momentAvailable = this.StopwatchProvider.ParseDuration(
                nowTimestamp,
                this.ReserveEarliestAvailable(permits, nowTimestamp));
            return momentAvailable.Ticks > 0 ? momentAvailable : TimeSpan.Zero;
        }

        protected bool CanAcquire(long nowTimestamp, TimeSpan timeout, out TimeSpan momentAvailableInterval)
        {
            momentAvailableInterval = this.StopwatchProvider.ParseDuration(
                nowTimestamp,
                this.QueryEarliestAvailable(nowTimestamp));
            return momentAvailableInterval <= timeout;
        }

        protected abstract long QueryEarliestAvailable(long nowTimestamp);

        protected abstract long ReserveEarliestAvailable(int permits, long nowTimestamp);
    }
}
