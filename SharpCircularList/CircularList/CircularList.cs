// <copyright file="CircularList.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IO.Github.Hcoona.Collections
{
    public class CircularList<T> : IList<T>
    {
        private readonly T[] objects;
        private int countValue;
        private int firstItemIdx;

        public CircularList(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("capacity must be a non-negative number.");
            }

            this.objects = new T[capacity];
            this.countValue = 0;
            this.firstItemIdx = 0;
        }

        public event EventHandler<T> OnOverflow;

        public int Capacity => this.objects.Length;

        public int Count => this.countValue;

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get => this.objects[this.GetPhysicalIndex(index)];
            set => this.objects[this.GetPhysicalIndex(index)] = value;
        }

        public void Add(T item)
        {
            if (this.Count == this.Capacity)
            {
                var overrittenObject = this[0];
                this[0] = item;
                this.firstItemIdx = (this.firstItemIdx + 1) % this.Capacity;

                this.OnOverflow?.Invoke(this, overrittenObject);
            }
            else
            {
                this[this.Count] = item;
                this.countValue++;
            }
        }

        public bool Contains(T item)
        {
            if (this.Count == this.Capacity)
            {
                return Array.Exists(this.objects, v => v.Equals(item));
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
            this.countValue = 0;
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
            if (this.firstItemIdx + this.Count < this.Capacity)
            {
                return this.Skip(this.firstItemIdx).Take(this.Count).GetEnumerator();
            }
            else
            {
                return this.Skip(this.firstItemIdx).Take(this.Count).Concat(this.Take(this.Count - this.firstItemIdx)).GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private int GetPhysicalIndex(int logicalIndex)
        {
            return (this.firstItemIdx + logicalIndex) % this.Capacity;
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
