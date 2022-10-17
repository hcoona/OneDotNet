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
    /// Basic interface for a stopwatch.
    /// <para>Stopwatch is different from a clock because it could not give a readable time point.
    /// Instead, it gives a readable elapsed duration.</para>
    /// </summary>
    public interface IStopwatch
    {
        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        bool IsRunning { get; }

        /// <summary>
        /// Gets the elapsed <seealso cref="TimeSpan"/> of the running time.
        /// </summary>
        /// <value>
        /// The elapsed <seealso cref="TimeSpan"/> of the running time.
        /// </value>
        TimeSpan Elapsed { get; }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        void Reset();

        /// <summary>
        /// Starts timing.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops timing.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Naming", "CA1716:标识符不应与关键字匹配", Justification = "TODO(zhangshuai.ds): Rename it.")]
        void Stop();

        /// <summary>
        /// Restarts timing.
        /// </summary>
        void Restart();
    }
}
