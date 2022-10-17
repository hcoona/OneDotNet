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
using System.Linq;

namespace IO.Github.Hcoona.Collections
{
#pragma warning disable CA1710 // 标识符应具有正确的后缀
    public class CircularList<T> : IList<T>
#pragma warning restore CA1710 // 标识符应具有正确的后缀
    {
        private readonly T[] objects;
        private int countValue;
        private int firstItemIdx;

        public CircularList(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(capacity), capacity, "must be a non-negative number.");
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
