using System;
using System.Threading;

namespace Clocks
{
    /// <summary>
    /// The Lamport scalar logical clock. Check the paper "Time, Clocks, and the Ordering of Events in a Distributed System" for further details.
    /// <para>The type is thread-safe.</para>
    /// </summary>
    /// <seealso cref="Clocks.ILogicalClock{System.Int64}" />
    public class LamportClock : ILogicalClock<long>
    {
        protected long counter;

        public LamportClock() : this(0) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LamportClock"/> class with given value as its internal counter initial value.
        /// </summary>
        /// <param name="initial">The internal counter initial value.</param>
        public LamportClock(long initial)
        {
            counter = initial;
        }

        public long Now => Interlocked.Read(ref counter);

        /// <summary>
        /// Increments the internal counter.
        /// <para><strong>IR1.</strong>Each process P<sub>i</sub> increments C<sub>i</sub> between any two successive events.</para>
        /// </summary>
        /// <returns>The incremented counter value.</returns>
        [Obsolete("Use ILogicalClock<long>.IncrementAndGet instead")]
        public long Increment()
        {
            return Interlocked.Increment(ref counter);
        }

        public long IncrementAndGet()
        {
            return Interlocked.Increment(ref counter);
        }

        /// <summary>
        /// Witnesses another timepoint.
        /// <para><strong>IR2.</strong>(a) If event a is the sending of a message m by process P<sub>i</sub>,
        /// then the message m contains a timestamp T<sub>m</sub> = C<sub>i</sub>< a >.
        /// (b) Upon receiving a message m, process P<sub>j</sub> sets C<sub>j</sub> greater than or equal to its present value and greater than T<sub>m</sub>.</para>
        /// </summary>
        /// <param name="timepoint">The timepoint.</param>
        /// <returns>The new counter value.</returns>
        [Obsolete("Use ILogicalClock<long>.Witness instead")]
        public long Witness(long timepoint)
        {
            while (true)
            {
                var current = Interlocked.Read(ref counter);
                if (timepoint < current)
                {
                    return current;
                }
                else
                {
                    var next = timepoint + 1;
                    if (current == Interlocked.CompareExchange(ref counter, next, current))
                    {
                        return next;
                    }
                }
            }
        }

        void ILogicalClock<long>.Increment()
        {
#pragma warning disable CS0618
            this.Increment();
#pragma warning restore CS0618
        }

        void ILogicalClock<long>.Witness(long other)
        {
#pragma warning disable CS0618
            this.Witness(other);
#pragma warning restore CS0618
        }
    }
}
