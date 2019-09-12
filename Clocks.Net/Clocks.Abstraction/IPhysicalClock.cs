using System;

namespace Clocks
{
    /// <summary>
    /// Basic interface for a physical clock
    /// <para>Physical clock could be regarded as a logical clock</para>
    /// </summary>
    /// <typeparam name="T">The concrete type of time point</typeparam>
    /// <seealso cref="Clocks.ILogicalClock{T}" />
    public interface IPhysicalClock<T> : IClock<T>
        where T : IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// Gets a value indicating whether this instance is high resolution.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is high resolution; otherwise, <c>false</c>.
        /// </value>
        bool IsHighResolution { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is monotonic.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is monotonic; otherwise, <c>false</c>.
        /// </value>
        bool IsMonotonic { get; }

        /// <summary>
        /// Parses the time point to readable type <seealso cref="System.DateTime"/>.
        /// </summary>
        /// <param name="timepoint">The timepoint.</param>
        /// <returns>The readable type <seealso cref="System.DateTime"/> representing the given time point.</returns>
        DateTime ParseTimePoint(T timepoint);
    }
}
