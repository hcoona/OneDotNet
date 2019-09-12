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
        private static readonly DateTime baseline;

        private static readonly long baselineTimestamp;

        static HighResolutionSystemClock()
        {
            baseline = DateTime.UtcNow;
            baselineTimestamp = Stopwatch.GetTimestamp();
        }

        public bool IsHighResolution => Stopwatch.IsHighResolution;

        public bool IsMonotonic => true;

        public long Now => Stopwatch.GetTimestamp();

        public DateTime ParseTimePoint(long timepoint)
        {
            var elapsedSeconds = (timepoint - baselineTimestamp) / (double)Stopwatch.Frequency;
            return baseline.AddSeconds(elapsedSeconds);
        }
    }
}
