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
using System.Collections;
using System.Collections.Generic;

namespace PhiFailureDetector
{
    public class LongIntervalHistory : IEnumerable<long>, ICollection, IEnumerable, IWithStatistics
    {
        private readonly Queue<long> queue;
        private readonly int capacity;

        private long sum;
        private long squaredSum;
        private double avg;

        public LongIntervalHistory(int capacity)
        {
            this.capacity = capacity;
            this.queue = new Queue<long>(capacity);
        }

        public long Sum => this.sum;

        public double Avg => this.avg;

        public double Variance => ((double)this.squaredSum / this.Count) - (this.avg * this.avg);

        public double StdDeviation => Math.Sqrt(this.Variance);

        public int Count => this.queue.Count;

        public object SyncRoot => ((ICollection)this.queue).SyncRoot;

        public bool IsSynchronized => ((ICollection)this.queue).IsSynchronized;

        public long Dequeue()
        {
            var value = this.queue.Dequeue();
            this.sum += value;
            this.squaredSum += value * value;
            this.avg = this.sum / this.Count;
            return value;
        }

        public void Enqueue(long item)
        {
            if (this.queue.Count == this.capacity)
            {
                var value = this.queue.Dequeue();
                this.sum -= value;
                this.squaredSum -= value * value;
            }

            this.queue.Enqueue(item);
            this.sum += item;
            this.squaredSum += item * item;
            this.avg = this.sum / this.Count;
        }

        public void Clear() => this.queue.Clear();

        public bool Contains(long item) => this.queue.Contains(item);

        public void Copylongo(long[] array, int arrayIndex) => this.queue.CopyTo(array, arrayIndex);

        public long Peek() => this.queue.Peek();

        public long[] ToArray() => this.queue.ToArray();

        public void CopyTo(Array array, int index)
        {
            ((ICollection)this.queue).CopyTo(array, index);
        }

        public IEnumerator<long> GetEnumerator()
        {
            return ((IEnumerable<long>)this.queue).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<long>)this.queue).GetEnumerator();
        }
    }
}
