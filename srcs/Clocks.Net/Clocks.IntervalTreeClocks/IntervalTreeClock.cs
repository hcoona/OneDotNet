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
using System.Threading;
using Itc4net;

namespace Clocks
{
    public class IntervalTreeClock : ILogicalClock<Stamp>
    {
        private Stamp stamp;

        public IntervalTreeClock()
            : this(new Stamp())
        {
        }

        public IntervalTreeClock(Stamp intialStamp)
        {
            this.stamp = intialStamp;
        }

        public Stamp Now => this.stamp.Peek();

        [Obsolete("Use ILogicalClock<long>.Increment instead")]
        public Stamp Increment()
        {
            while (true)
            {
                var originStamp = this.stamp;
                var newStamp = originStamp.Event();
                if (originStamp == Interlocked.CompareExchange(ref this.stamp, newStamp, originStamp))
                {
                    return originStamp;
                }
            }
        }

        public Tuple<IntervalTreeClock, IntervalTreeClock> Fork()
        {
            this.stamp.Fork(out var s1, out var s2);
            return Tuple.Create(new IntervalTreeClock(s1), new IntervalTreeClock(s2));
        }

        public Tuple<IntervalTreeClock, IntervalTreeClock, IntervalTreeClock> Fork3()
        {
            this.stamp.Fork(out var s1, out var s2, out var s3);
            return Tuple.Create(new IntervalTreeClock(s1), new IntervalTreeClock(s2), new IntervalTreeClock(s3));
        }

        public Tuple<IntervalTreeClock, IntervalTreeClock, IntervalTreeClock, IntervalTreeClock> Fork4()
        {
            this.stamp.Fork(out var s1, out var s2, out var s3, out var s4);
            return Tuple.Create(
                new IntervalTreeClock(s1),
                new IntervalTreeClock(s2),
                new IntervalTreeClock(s3),
                new IntervalTreeClock(s4));
        }

        public Stamp Join(IntervalTreeClock other) => InterlockedUpdate(ref this.stamp, originStamp => originStamp.Join(other.stamp));

        public Stamp Join(Stamp other) => InterlockedUpdate(ref this.stamp, originStamp => originStamp.Join(other));

        void ILogicalClock<Stamp>.Increment()
        {
#pragma warning disable CS0618 // 类型或成员已过时
            _ = this.Increment();
#pragma warning restore CS0618 // 类型或成员已过时
        }

        /// <summary>
        /// Adjust internal counter because know about other logical time. It use <code>Receive</code> method of <seealso cref="Stamp"/>.
        /// </summary>
        /// <param name="other">Other timestamp</param>
        void ILogicalClock<Stamp>.Witness(Stamp other)
        {
            InterlockedUpdate(ref this.stamp, originStamp => originStamp.Receive(other));
        }

        public Stamp IncrementAndGet() => InterlockedUpdate(ref this.stamp, originStamp => originStamp.Event());

        private static T InterlockedUpdate<T>(ref T location, Func<T, T> newValueFunc)
            where T : class
        {
            while (true)
            {
                var originValue = location;
                var newValue = newValueFunc(originValue);
                if (originValue == Interlocked.CompareExchange(ref location, newValue, originValue))
                {
                    return newValue;
                }
            }
        }
    }
}
