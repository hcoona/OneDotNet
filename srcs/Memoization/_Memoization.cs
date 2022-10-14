// <copyright file="_Memoization.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Caching.Memory;

namespace Memoization
{
    public static partial class Memoization
    {
        public static IMemoryCache DefaultCache { get; set; }
    }
}
