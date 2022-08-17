using System;
using System.Collections;
using System.Collections.Generic;

namespace PhiFailureDetector
{
    public class LongIntervalHistory : IEnumerable<long>, ICollection, IEnumerable, IWithStatistics
    {
        private readonly Queue<long> m_queue;
        private readonly int m_capacity;
        
        private long m_sum;
        private long m_squaredSum;
        private double m_avg;

        public LongIntervalHistory(int capacity)
        {
            this.m_capacity = capacity;
            this.m_queue = new Queue<long>(capacity);
        }

        public long Sum => m_sum;

        public double Avg => m_avg;

        public double Variance => ((double)m_squaredSum / Count) - (m_avg * m_avg);

        public double StdDeviation => Math.Sqrt(Variance);

        public long Dequeue()
        {
            var value = m_queue.Dequeue();
            m_sum += value;
            m_squaredSum += value * value;
            m_avg = m_sum / Count;
            return value;
        }

        public void Enqueue(long item)
        {
            if (m_queue.Count == m_capacity)
            {
                var value = m_queue.Dequeue();
                m_sum -= value;
                m_squaredSum -= value * value;
            }
            m_queue.Enqueue(item);
            m_sum += item;
            m_squaredSum += item * item;
            m_avg = m_sum / Count;
        }

        public void Clear() => this.m_queue.Clear();

        public bool Contains(long item) => m_queue.Contains(item);

        public void Copylongo(long[] array, int arrayIndex) => m_queue.CopyTo(array, arrayIndex);

        public long Peek() => m_queue.Peek();

        public long[] ToArray() => m_queue.ToArray();

        public int Count => this.m_queue.Count;

        public object SyncRoot => ((ICollection)this.m_queue).SyncRoot;

        public bool IsSynchronized => ((ICollection)this.m_queue).IsSynchronized;

        public void CopyTo(Array array, int index)
        {
            ((ICollection)this.m_queue).CopyTo(array, index);
        }

        public IEnumerator<long> GetEnumerator()
        {
            return ((IEnumerable<long>)this.m_queue).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<long>)this.m_queue).GetEnumerator();
        }
    }
}
