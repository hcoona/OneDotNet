// <copyright file="SystemClock.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Clocks
{
    /// <summary>
    /// System built-in clock implemented by <seealso cref="DateTime"/>
    /// </summary>
    /// <seealso cref="Clocks.IPhysicalClock{System.DateTime}" />
    public class SystemClock : IPhysicalClock<DateTime>
    {
        /// <summary>
        /// Gets a value indicating whether the instance is high resolution. Always <c>false</c> because <seealso cref="DateTime"/> is not high resolution.
        /// </summary>
        /// <value>
        /// Always <c>false</c>.
        /// </value>
        // The Ticks property expresses date and time values in units of one ten-millionth of a second
        public bool IsHighResolution => false;

        /// <summary>
        /// Gets a value indicating whether this instance is monotonic. Always <c>false</c> because <seealso cref="DateTime"/> is not monotonic.
        /// </summary>
        /// <value>
        /// Always <c>false</c>.
        /// </value>
        // Using repeated calls to the DateTime.Now property to measure elapsed time is dependent on the system clock.
        public bool IsMonotonic => false;

        public DateTime Now => DateTime.Now;

        public DateTime UtcNow => DateTime.UtcNow;

        public DateTime ParseTimePoint(DateTime timepoint)
        {
            return timepoint;
        }
    }
}
