using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using TL = TimeLimiter.TimeLimiter;

namespace TimeLimiterUnitTest
{
    public class SynchronousInvokeTestSuit
    {
        [Fact]
        public void TestInvokeAction()
        {
            var stopwatch = Stopwatch.StartNew();
            Assert.Throws<TimeoutException>(() =>
            {
                TL.Invoke(
                    ct => { Task.Delay(2000, ct).GetAwaiter().GetResult(); },
                    CancellationToken.None,
                    TimeSpan.FromMilliseconds(400));
            });
            stopwatch.Stop();

            Assert.True(stopwatch.ElapsedMilliseconds < 1000);
        }

        [Fact]
        public void TestInvokeActionNotTimeout()
        {
            bool flag = false;

            var stopwatch = Stopwatch.StartNew();
            TL.Invoke(
                ct => { Task.Delay(200, ct).ContinueWith(_ => flag = true).GetAwaiter().GetResult(); },
                CancellationToken.None,
                TimeSpan.FromMilliseconds(400));
            stopwatch.Stop();

            Assert.True(flag);
            Assert.InRange(stopwatch.ElapsedMilliseconds, 190, 220);
        }

        [Fact]
        public void TestInvokeActionWithCancellation()
        {
            var stopwatch = Stopwatch.StartNew();
            Assert.Throws<TaskCanceledException>(() =>
            {
                TL.Invoke(
                    ct => { Task.Delay(2000, ct).GetAwaiter().GetResult(); },
                    new CancellationTokenSource(200).Token,
                    TimeSpan.FromMilliseconds(1000));
            });
            stopwatch.Stop();

            Assert.InRange(stopwatch.ElapsedMilliseconds, 190, 220);
        }

        [Fact]
        public void TestInvokeActionThrowException()
        {
            var stopwatch = Stopwatch.StartNew();
            Assert.Throws<ByDesignException>(() =>
            {
                TL.Invoke(
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
        public void TestInvokeFunction()
        {
            var stopwatch = Stopwatch.StartNew();
            Assert.Throws<TimeoutException>(() =>
            {
                TL.Invoke(
                    ct => Task.Delay(2000, ct).ContinueWith(_ => 42, ct).GetAwaiter().GetResult(),
                    CancellationToken.None,
                    TimeSpan.FromMilliseconds(400));
            });
            stopwatch.Stop();

            Assert.True(stopwatch.ElapsedMilliseconds < 1000);
        }

        [Fact]
        public void TestInvokeFunctionNotTimeout()
        {
            var stopwatch = Stopwatch.StartNew();
            var answer = TL.Invoke(
                ct => Task.Delay(200, ct).ContinueWith(_ => 42, ct).GetAwaiter().GetResult(),
                CancellationToken.None,
                TimeSpan.FromMilliseconds(400));
            stopwatch.Stop();

            Assert.StrictEqual(42, answer);
            Assert.InRange(stopwatch.ElapsedMilliseconds, 190, 220);
        }

        [Fact]
        public void TestInvokeFunctionWithCancellation()
        {
            var stopwatch = Stopwatch.StartNew();
            Assert.Throws<TaskCanceledException>(() =>
            {
                TL.Invoke(
                    ct => Task.Delay(2000, ct).ContinueWith(_ => 42, ct).GetAwaiter().GetResult(),
                    new CancellationTokenSource(200).Token,
                    TimeSpan.FromMilliseconds(1000));
            });
            stopwatch.Stop();

            Assert.InRange(stopwatch.ElapsedMilliseconds, 190, 220);
        }

        [Fact]
        public void TestInvokeFunctionThrowException()
        {
            var stopwatch = Stopwatch.StartNew();
            Assert.Throws<ByDesignException>(() =>
            {
                TL.Invoke<object>(
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
