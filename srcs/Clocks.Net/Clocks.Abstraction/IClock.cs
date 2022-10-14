// <copyright file="IClock.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Clocks
{
    public interface IClock<T>
        where T : IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// Gets current time point.
        /// </summary>
        /// <value>
        /// The current time point.
        /// </value>
        T Now { get; }
    }
}
