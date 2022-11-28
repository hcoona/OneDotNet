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
    /// The factory providing. <seealso cref="IStopwatch"/>
    /// </summary>
    public interface IStopwatchProvider
    {
        /// <summary>
        /// Gets a value indicating whether this instance is high resolution.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is high resolution; otherwise, <c>false</c>.
        /// </value>
        bool IsHighResolution { get; }

        /// <summary>
        /// Creates a <seealso cref="IStopwatch"/> in stopped state.
        /// </summary>
        /// <returns><seealso cref="IStopwatch"/> in stopped state.</returns>
        IStopwatch Create();

        /// <summary>
        /// Creates a <seealso cref="IStopwatch"/> in running state.
        /// </summary>
        /// <returns><seealso cref="IStopwatch"/> in running state.</returns>
        IStopwatch StartNew();
    }

    /// <summary>
    /// Could provide timestamp if not necessary to create a <seealso cref="IStopwatch"/>.
    /// </summary>
    /// <typeparam name="T">The concrete type of timestamp.</typeparam>
    public interface IStopwatchProvider<T> : IStopwatchProvider
    {
        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        /// <returns>Current timestamp.</returns>
        T GetTimestamp();

        /// <summary>
        /// Parses the duration from <paramref name="fromTimestamp"/> to
        /// <paramref name="toTimestamp"/> to a readable <seealso cref="TimeSpan"/>.
        /// </summary>
        /// <param name="fromTimestamp">The starting timestamp.</param>
        /// <param name="toTimestamp">The ending timestamp.</param>
        /// <returns>
        /// The readable <seealso cref="TimeSpan"/> representing
        /// the duration from <paramref name="fromTimestamp"/>
        /// to <paramref name="toTimestamp"/>.
        /// </returns>
        TimeSpan ParseDuration(T fromTimestamp, T toTimestamp);

        /// <summary>
        /// Gets the next timestamp elapsed <paramref name="interval"/>
        /// from <paramref name="from"/>.
        /// </summary>
        /// <param name="from">The starting timestamp.</param>
        /// <param name="interval">The elpased interval.</param>
        /// <returns>
        /// The next timestamp elapsed <paramref name="interval"/> from <paramref name="from"/>.
        /// </returns>
        T GetNextTimestamp(T from, TimeSpan interval);
    }
}
