using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IO.Github.Hcoona.Collections
{
    public class CircularList<T> : IList<T>
    {
        internal readonly T[] m_objects;
        internal int m_count;
        internal int m_firstItemIdx;

        public CircularList(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("capacity must be a non-negative number.");
            }

            m_objects = new T[capacity];
            m_count = 0;
            m_firstItemIdx = 0;
        }

        public event EventHandler<T> OnOverflow;

        public int Capacity => m_objects.Length;

        public int Count => m_count;

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get => m_objects[GetPhysicalIndex(index)];
            set => this.m_objects[this.GetPhysicalIndex(index)] = value;
        }

        public void Add(T item)
        {
            if (Count == Capacity)
            {
                var overrittenObject = this[0];
                this[0] = item;
                m_firstItemIdx = (m_firstItemIdx + 1) % Capacity;

                OnOverflow?.Invoke(this, overrittenObject);
            }
            else
            {
                this[Count] = item;
                m_count++;
            }
        }

        public bool Contains(T item)
        {
            if (Count == Capacity)
            {
                return Array.Exists(m_objects, v => v.Equals(item));
            }
            else
            {
                return this.AsEnumerable().Contains(item);
            }
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            m_count = 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            // TODO: Support only first & last pos if possible
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            // TODO: Support only first & last pos if possible
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (m_firstItemIdx + Count < Capacity)
            {
                return this.Skip(m_firstItemIdx).Take(Count).GetEnumerator();
            }
            else
            {
                return this.Skip(m_firstItemIdx).Take(Count).Concat(this.Take(Count - m_firstItemIdx)).GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private int GetPhysicalIndex(int logicalIndex)
        {
            return (m_firstItemIdx + logicalIndex) % Capacity;
        }

        private void RemoveFirst()
        {
            throw new NotImplementedException();
        }

        private void RemoveLast()
        {
            throw new NotImplementedException();
        }
    }
}
