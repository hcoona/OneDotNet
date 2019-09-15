// <copyright file="ILogicalClock.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Clocks
{
    /// <summary>
    /// Basic interface for a logical clock
    /// <para>Typical logical clocks are <strong>lamport scalar clock</strong> &amp; <strong>vector clock</strong></para>
    /// </summary>
    /// <typeparam name="T">The concrete type of time point</typeparam>
    public interface ILogicalClock<T> : IClock<T>
        where T : IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// Increments the internal counter representing current logical time.
        /// </summary>
        void Increment();

        /// <summary>
        /// Increments the internal clock and get the timepoint.
        /// </summary>
        /// <returns>The incremented clock timepoint</returns>
        T IncrementAndGet();

        /// <summary>
        /// Adjust internal counter because know about other logical time.
        /// </summary>
        void Witness(T other);
    }
}
