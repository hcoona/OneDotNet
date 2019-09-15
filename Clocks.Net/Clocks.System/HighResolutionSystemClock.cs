// <copyright file="HighResolutionSystemClock.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Diagnostics;

namespace Clocks
{
    /// <summary>
    /// System built-in clock implemented by <seealso cref="Stopwatch"/>
    /// </summary>
    /// <seealso cref="Clocks.IPhysicalClock{System.Int64}" />
    public class HighResolutionSystemClock : IPhysicalClock<long>
    {
        private static readonly DateTime Baseline = DateTime.UtcNow;

        private static readonly long BaselineTimestamp = Stopwatch.GetTimestamp();

        public bool IsHighResolution => Stopwatch.IsHighResolution;

        public bool IsMonotonic => true;

        public long Now => Stopwatch.GetTimestamp();

        public DateTime ParseTimePoint(long timepoint)
        {
            var elapsedSeconds = (timepoint - BaselineTimestamp) / (double)Stopwatch.Frequency;
            return Baseline.AddSeconds(elapsedSeconds);
        }
    }
}
