// <copyright file="IAsyncBlocker.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

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

