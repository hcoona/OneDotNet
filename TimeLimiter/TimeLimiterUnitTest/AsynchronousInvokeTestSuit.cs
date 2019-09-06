// <copyright file="AsynchronousInvokeTestSuit.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using TL = TimeLimiter.TimeLimiter;

namespace TimeLimiterUnitTest
{
    // TODO: Add Asynchronous Invoke Func<Task> Test Suit
    public class AsynchronousInvokeTestSuit
    {
        [Fact]
        public async Task TestInvokeActionAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            await Assert.ThrowsAsync<TimeoutException>(() =>
            {
                return TL.InvokeAsync(
                    ct => { Task.Delay(2000, ct).GetAwaiter().GetResult(); },
                    CancellationToken.None,
                    TimeSpan.FromMilliseconds(400));
            });
            stopwatch.Stop();

            Assert.True(stopwatch.ElapsedMilliseconds < 1000);
        }

        [Fact]
        public async Task TestInvokeActionNotTimeoutAsync()
        {
            bool flag = false;

            var stopwatch = Stopwatch.StartNew();
            await TL.InvokeAsync(
                ct => { Task.Delay(200, ct).ContinueWith(_ => flag = true).GetAwaiter().GetResult(); },
                CancellationToken.None,
                TimeSpan.FromMilliseconds(400));
            stopwatch.Stop();

            Assert.True(flag);
            Assert.InRange(stopwatch.ElapsedMilliseconds, 190, 220);
        }

        [Fact]
        public async Task TestInvokeActionWithCancellationAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
            {
                return TL.InvokeAsync(
                    ct => { Task.Delay(2000, ct).GetAwaiter().GetResult(); },
                    new CancellationTokenSource(200).Token,
                    TimeSpan.FromMilliseconds(1000));
            });
            stopwatch.Stop();

            Assert.InRange(stopwatch.ElapsedMilliseconds, 190, 220);
        }

        [Fact]
        public async Task TestInvokeActionThrowExceptionAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            await Assert.ThrowsAsync<ByDesignException>(() =>
            {
                return TL.InvokeAsync(
                    ct =>
                    {
                        Task.Delay(100).GetAwaiter().GetResult();
                        throw new ByDesignException();
                    },
                    CancellationToken.None,
                    TimeSpan.FromMilliseconds(1000));
            });
            stopwatch.Stop();

            Assert.InRange(stopwatch.ElapsedMilliseconds, 90, 120);
        }

        [Fact]
        public async Task TestInvokeFunctionAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            await Assert.ThrowsAsync<TimeoutException>(() =>
            {
                return TL.InvokeAsync(
                    ct => Task.Delay(2000, ct).ContinueWith(_ => 42, ct).GetAwaiter().GetResult(),
                    CancellationToken.None,
                    TimeSpan.FromMilliseconds(400));
            });
            stopwatch.Stop();

            Assert.True(stopwatch.ElapsedMilliseconds < 1000);
        }

        [Fact]
        public async Task TestInvokeFunctionNotTimeoutAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            var answer = await TL.InvokeAsync(
                ct => Task.Delay(200, ct).ContinueWith(_ => 42, ct).GetAwaiter().GetResult(),
                CancellationToken.None,
                TimeSpan.FromMilliseconds(400));
            stopwatch.Stop();

            Assert.StrictEqual(42, answer);
            Assert.InRange(stopwatch.ElapsedMilliseconds, 190, 220);
        }

        [Fact]
        public async Task TestInvokeFunctionWithCancellationAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
            {
                return TL.InvokeAsync(
                    ct => Task.Delay(2000, ct).ContinueWith(_ => 42, ct).GetAwaiter().GetResult(),
                    new CancellationTokenSource(200).Token,
                    TimeSpan.FromMilliseconds(1000));
            });
            stopwatch.Stop();

            Assert.InRange(stopwatch.ElapsedMilliseconds, 190, 220);
        }

        [Fact]
        public async Task TestInvokeFuncionThrowExceptionAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            await Assert.ThrowsAsync<ByDesignException>(() =>
            {
                return TL.InvokeAsync<object>(
                    ct =>
                    {
                        Task.Delay(100).GetAwaiter().GetResult();
                        throw new ByDesignException();
                    },
                    CancellationToken.None,
                    TimeSpan.FromMilliseconds(1000));
            });
            stopwatch.Stop();

            Assert.InRange(stopwatch.ElapsedMilliseconds, 90, 120);
        }
    }
}
