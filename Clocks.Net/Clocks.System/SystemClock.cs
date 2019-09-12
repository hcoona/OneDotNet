using System;

namespace Clocks
{
    /// <summary>
    /// System built-in clock implemented by <seealso cref="DateTime"/>
    /// </summary>
    /// <seealso cref="Clocks.IPhysicalClock{System.DateTime}" />
    public class SystemClock : IPhysicalClock<DateTime>
    {
        // The Ticks property expresses date and time values in units of one ten-millionth of a second
        /// <summary>
        /// Whether the instance is high resolution. Always <c>false</c> because <seealso cref="DateTime"/> is not high resolution.
        /// </summary>
        /// <value>
        /// Always <c>false</c>.
        /// </value>
        public bool IsHighResolution => false;

        // Using repeated calls to the DateTime.Now property to measure elapsed time is dependent on the system clock.
        /// <summary>
        /// Whether this instance is monotonic. Always <c>false</c> because <seealso cref="DateTime"/> is not monotonic.
        /// </summary>
        /// <value>
        /// Always <c>false</c>.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool IsMonotonic => false;

        public DateTime Now => DateTime.Now;

        public DateTime UtcNow => DateTime.UtcNow;

        public DateTime ParseTimePoint(DateTime timepoint)
        {
            return timepoint;
        }
    }
}
