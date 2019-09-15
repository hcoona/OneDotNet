// <copyright file="IStopwatch.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

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
