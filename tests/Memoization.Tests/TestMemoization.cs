// <copyright file="TestMemoization.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace Memoization.Tests
{
    public class TestMemoization
    {
        static TestMemoization()
        {
            Memoization.DefaultCache = new MemoryCache(new MemoryCacheOptions());
        }

        [Fact]
        public void Test1()
        {
            var counter = 0;
            var fib = YCombinator<int>.Fix(f => x =>
            {
                counter++;
                return x < 2 ? 1 : f(x - 1) + f(x - 2);
            });

            counter = 0;
            fib(7);
            Assert.Equal(41, counter);

            counter = 0;
            fib(7);
            Assert.Equal(41, counter);
        }

        [Fact]
        public void Test2()
        {
            var counter = 0;
            var fib = YCombinator<int>.Fix(f => x =>
            {
                counter++;
                return x < 2 ? 1 : f(x - 1) + f(x - 2);
            });
            var m_fib = Memoization.Create(fib);

            counter = 0;
            m_fib(7);
            Assert.Equal(41, counter);

            counter = 0;
            m_fib(7);
            Assert.Equal(0, counter);
        }

        internal static class YCombinator<T>
        {
            private static Func<Func<Func<T, T>, Func<T, T>>, Func<T, T>> fix =
              f => ((Recursive)(g =>
                  f(x => g(g)(x))))((Recursive)(g => f(x => g(g)(x))));

            private delegate Func<T, T> Recursive(Recursive recursive);

            public static Func<Func<Func<T, T>, Func<T, T>>, Func<T, T>> Fix { get => fix; set => fix = value; }
        }
    }
}
