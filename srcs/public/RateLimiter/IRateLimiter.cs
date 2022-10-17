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
#if !NET20
using System.Threading;
using System.Threading.Tasks;
#endif

namespace RateLimiter
{
    public interface IRateLimiter
    {
        double PermitsPerSecond { get; set; }

        TimeSpan Acquire();

        TimeSpan Acquire(int permits);

#if !NET20
        Task<TimeSpan> AcquireAsync();

        Task<TimeSpan> AcquireAsync(CancellationToken cancellationToken);

        Task<TimeSpan> AcquireAsync(int permits);

        Task<TimeSpan> AcquireAsync(int permits, CancellationToken cancellationToken);
#endif

        TryAcquireResult TryAcquire();

#if !NET20
        Task<TryAcquireResult> TryAcquireAsync();

        Task<TryAcquireResult> TryAcquireAsync(CancellationToken cancellationToken);
#endif

        TryAcquireResult TryAcquire(int permits);

#if !NET20
        Task<TryAcquireResult> TryAcquireAsync(int permits);

        Task<TryAcquireResult> TryAcquireAsync(int permits, CancellationToken cancellationToken);
#endif

        TryAcquireResult TryAcquire(TimeSpan timeout);

#if !NET20
        Task<TryAcquireResult> TryAcquireAsync(TimeSpan timeout);

        Task<TryAcquireResult> TryAcquireAsync(TimeSpan timeout, CancellationToken cancellationToken);
#endif

        TryAcquireResult TryAcquire(int permits, TimeSpan timeout);

#if !NET20
        Task<TryAcquireResult> TryAcquireAsync(int permits, TimeSpan timeout);

        Task<TryAcquireResult> TryAcquireAsync(int permits, TimeSpan timeout, CancellationToken cancellationToken);
#endif

        TimeSpan Reserve(int permits);

#if !NET20
        Task<TimeSpan> ReserveAsync(int permits);

        Task<TimeSpan> ReserveAsync(int permits, CancellationToken cancellationToken);
#endif
    }
}
