using System;

namespace Clocks
{
    public interface IClock<T>
        where T : IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// Get current time point.
        /// </summary>
        /// <value>
        /// The current time point.
        /// </value>
        T Now { get; }
    }
}
