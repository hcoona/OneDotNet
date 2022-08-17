# Memoization #

This project help C# make memoized function.

See [Memoization - Wikipedia](https://en.wikipedia.org/wiki/Memoization) for further details.

See [aspnet/Caching](https://github.com/aspnet/Caching) for cache related details.

## Getting Started ##

```csharp
// Set global default cache before use.
Memoization.DefaultCache = new MemoryCache(new MemoryCacheOptions());

var m_fib = Memoization.Create(fib);

// Use method specified cache
var m_fib2 = Memoization.Create(fib, cache);

// Use method specified cache policy
var m_fib3 = Memoization.Create(fib, option);

// Combine them together
var m_fib4 = Memoization.Create(fib, cache, option);

counter = 0;
m_fib(7);
Assert.Equal(41, counter);

counter = 0;
m_fib(7); // Will read result directly from cache if exist
Assert.Equal(0, counter);
```
