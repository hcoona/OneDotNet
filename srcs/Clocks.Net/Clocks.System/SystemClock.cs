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
    /// System built-in clock implemented by <seealso cref="DateTime"/>.
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Performance",
            "CA1822: Member UtcNow does not access instance data and can be marked as static",
            Justification = "By-design to align with `Now`, will mark static in next version")]
        public DateTime UtcNow => DateTime.UtcNow;

        public DateTime ParseTimePoint(DateTime timepoint)
        {
            return timepoint;
        }
    }
}
