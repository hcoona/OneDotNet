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

namespace RateLimiter
{
    internal interface IAsyncBlocker
    {
        Task WaitAsync(TimeSpan timeout, CancellationToken cancellationToken);
    }

    internal class TaskDelayAsyncBlocker : IAsyncBlocker
    {
        public static readonly IAsyncBlocker Instance = new TaskDelayAsyncBlocker();

        public Task WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            return Task.Delay(timeout, cancellationToken);
        }
    }
}
