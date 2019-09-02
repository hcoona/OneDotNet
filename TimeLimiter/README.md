# Time Limiter #

This is an optimized time limiter for asynchronous tasks.

## Getting Started ##

You can run either sync/async action/function/task with a specified timeout limit. For performance consideration,
it forces you consume a `CancellationToken` to gracefully stop your task when timeout limit reached.

### Asynchronous Scenario ###

```csharp
public static Task InvokeAsync(
  Func<CancellationToken, Task> func,
  CancellationToken cancellationToken,
  TimeSpan timeoutLimit);

public static Task<T> InvokeAsync<T>(
  Func<CancellationToken, Task<T>> func,
  CancellationToken cancellationToken,
  TimeSpan timeoutLimit);

public static Task InvokeAsync(
  Action<CancellationToken> action,
  CancellationToken cancellationToken,
  TimeSpan timeoutLimit);

public static Task<T> InvokeAsync<T>(
  Func<CancellationToken, T> func,
  CancellationToken cancellationToken,
  TimeSpan timeoutLimit);
```

### Synchronous Scenario ###

```csharp
public static void Invoke(
  Action<CancellationToken> action,
  CancellationToken cancellationToken,
  TimeSpan timeoutLimit);

public static T Invoke<T>(
  Func<CancellationToken, T> func,
  CancellationToken cancellationToken,
  TimeSpan timeoutLimit);
```
