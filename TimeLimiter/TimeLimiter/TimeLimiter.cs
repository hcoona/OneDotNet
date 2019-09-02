using System;
using System.Threading;
using System.Threading.Tasks;

namespace TimeLimiter
{
    public static class TimeLimiter
    {
        public static void Invoke(
            Action<CancellationToken> action,
            CancellationToken cancellationToken,
            TimeSpan timeoutLimit)
        {
            InvokeAsync(action, cancellationToken, timeoutLimit).GetAwaiter().GetResult();
        }

        public static T Invoke<T>(
            Func<CancellationToken, T> func,
            CancellationToken cancellationToken,
            TimeSpan timeoutLimit)
        {
            return InvokeAsync(func, cancellationToken, timeoutLimit).GetAwaiter().GetResult();
        }

        public static Task InvokeAsync(
            Func<CancellationToken, Task> func,
            CancellationToken cancellationToken,
            TimeSpan timeoutLimit)
        {
            var timeoutCancellationTokenSource = new CancellationTokenSource();
            var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                timeoutCancellationTokenSource.Token
            );

            var taskCompletionSource = new TaskCompletionSource<object>();

            Task.Delay(timeoutLimit, cancellationToken).ContinueWith(t =>
            {
                if (t.Status == TaskStatus.RanToCompletion)
                {
                    timeoutCancellationTokenSource.Cancel();
                    taskCompletionSource.TrySetException(new TimeoutException());
                }
            });

            func(linkedCancellationTokenSource.Token).ContinueWith(t =>
            {
                if (t.IsCanceled)
                {
                    if (!taskCompletionSource.Task.IsCompleted)
                    {
                        taskCompletionSource.TrySetCanceled();
                    }
                }
                else if (t.IsFaulted)
                {
                    foreach (var ex in t.Exception.InnerExceptions)
                    {
                        taskCompletionSource.TrySetException(ex);
                    }
                }
                else
                {
                    try
                    {
                        t.GetAwaiter().GetResult();
                        taskCompletionSource.TrySetResult(null);
                    }
                    catch (Exception ex)
                    {
                        taskCompletionSource.TrySetException(ex);
                    }
                }
            });

            return taskCompletionSource.Task;
        }

        public static Task<T> InvokeAsync<T>(
            Func<CancellationToken, Task<T>> func,
            CancellationToken cancellationToken,
            TimeSpan timeoutLimit)
        {
            var timeoutCancellationTokenSource = new CancellationTokenSource();
            var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                timeoutCancellationTokenSource.Token
            );

            var taskCompletionSource = new TaskCompletionSource<T>();

            Task.Delay(timeoutLimit, cancellationToken).ContinueWith(t =>
            {
                if (t.Status == TaskStatus.RanToCompletion)
                {
                    timeoutCancellationTokenSource.Cancel();
                    taskCompletionSource.TrySetException(new TimeoutException());
                }
            });

            func(linkedCancellationTokenSource.Token).ContinueWith(t =>
            {
                if (t.IsCanceled)
                {
                    taskCompletionSource.TrySetCanceled();
                }
                else if (t.IsFaulted)
                {
                    foreach (var ex in t.Exception.InnerExceptions)
                    {
                        taskCompletionSource.TrySetException(ex);
                    }
                }
                else
                {
                    try
                    {
                        taskCompletionSource.TrySetResult(t.GetAwaiter().GetResult());
                    }
                    catch (Exception ex)
                    {
                        taskCompletionSource.TrySetException(ex);
                    }
                }
            });

            return taskCompletionSource.Task;
        }

        public static Task InvokeAsync(
            Action<CancellationToken> action,
            CancellationToken cancellationToken,
            TimeSpan timeoutLimit)
        {
            return InvokeAsync(ct => Task.Run(() => action(ct), ct), cancellationToken, timeoutLimit);
        }

        public static Task<T> InvokeAsync<T>(
            Func<CancellationToken, T> func,
            CancellationToken cancellationToken,
            TimeSpan timeoutLimit)
        {
            return InvokeAsync(ct => Task.Run(() => func(ct), ct), cancellationToken, timeoutLimit);
        }
    }
}
