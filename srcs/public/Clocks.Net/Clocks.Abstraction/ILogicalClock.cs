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

namespace Clocks
{
    /// <summary>
    /// Basic interface for a logical clock.
    /// <para>Typical logical clocks are <strong>lamport scalar clock</strong> &amp;. <strong>vector clock</strong></para>
    /// </summary>
    /// <typeparam name="T">The concrete type of time point.</typeparam>
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
        /// <returns>The incremented clock timepoint.</returns>
        T IncrementAndGet();

        /// <summary>
        /// Adjust internal counter because know about other logical time.
        /// </summary>
        void Witness(T other);
    }
}
