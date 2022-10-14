// <copyright file="LongIntervalHistory.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

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
