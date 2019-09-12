using System;

namespace Clocks
{
    /// <summary>
    /// The factory providing <seealso cref="IStopwatch"/>
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
        /// Parses the duration from <paramref name="from"/> to <paramref name="to"/> to a readable <seealso cref="TimeSpan"/>.
        /// </summary>
        /// <param name="from">The starting timestamp.</param>
        /// <param name="to">The ending timestamp.</param>
        /// <returns>The readable <seealso cref="TimeSpan"/> representing the duration from <paramref name="from"/> to <paramref name="to"/>.</returns>
        TimeSpan ParseDuration(T from, T to);

        /// <summary>
        /// Gets the next timestamp elapsed <paramref name="interval"/> from <paramref name="from"/>.
        /// </summary>
        /// <param name="from">The starting timestamp.</param>
        /// <param name="interval">The elpased interval.</param>
        /// <returns>The next timestamp elapsed <paramref name="interval"/> from <paramref name="from"/>.</returns>
        T GetNextTimestamp(T from, TimeSpan interval);
    }
}
