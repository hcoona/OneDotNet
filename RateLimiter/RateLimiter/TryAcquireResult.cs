// <copyright file="TryAcquireResult.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RateLimiter
{
    public class TryAcquireResult
    {
        public bool Succeed { get; set; }

        public TimeSpan MomentAvailableInterval { get; set; }
    }
}

