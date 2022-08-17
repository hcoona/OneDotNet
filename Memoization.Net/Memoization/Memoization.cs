using System;
using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;

namespace Memoization
{
    public static partial class Memoization
    {

        [DebuggerStepThrough]
        public static Func<T1, TResult> Create<T1, TResult>(Func<T1, TResult> func)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"));
        }
        
        [DebuggerStepThrough]
        public static Func<T1, TResult> Create<T1, TResult>(Func<T1, TResult> func, MemoryCacheEntryOptions options)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"), options);
        }
        
        public static Func<T1, TResult> Create<T1, TResult>(Func<T1, TResult> func, IMemoryCache cache)
        {
            return (T1 t1) => cache.GetOrCreate((t1), ignored => func(t1));
        }

        public static Func<T1, TResult> Create<T1, TResult>(Func<T1, TResult> func, IMemoryCache cache, MemoryCacheEntryOptions options)
        {
            return (T1 t1) =>
            {
                var key = (t1);
                if (!cache.TryGetValue<TResult>(key, out var result))
                {
                    var entry = cache.CreateEntry(key);
                    result = func(t1);
                    entry.SetOptions(options);
                    entry.SetValue(result);
                    // need to manually call dispose instead of having a using
                    // in case the factory passed in throws, in which case we
                    // do not want to add the entry to the cache
                    entry.Dispose();
                }

                return result;
            };
        }

        [DebuggerStepThrough]
        public static Func<T1, T2, TResult> Create<T1, T2, TResult>(Func<T1, T2, TResult> func)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"));
        }
        
        [DebuggerStepThrough]
        public static Func<T1, T2, TResult> Create<T1, T2, TResult>(Func<T1, T2, TResult> func, MemoryCacheEntryOptions options)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"), options);
        }
        
        public static Func<T1, T2, TResult> Create<T1, T2, TResult>(Func<T1, T2, TResult> func, IMemoryCache cache)
        {
            return (T1 t1, T2 t2) => cache.GetOrCreate((t1, t2), ignored => func(t1, t2));
        }

        public static Func<T1, T2, TResult> Create<T1, T2, TResult>(Func<T1, T2, TResult> func, IMemoryCache cache, MemoryCacheEntryOptions options)
        {
            return (T1 t1, T2 t2) =>
            {
                var key = (t1, t2);
                if (!cache.TryGetValue<TResult>(key, out var result))
                {
                    var entry = cache.CreateEntry(key);
                    result = func(t1, t2);
                    entry.SetOptions(options);
                    entry.SetValue(result);
                    // need to manually call dispose instead of having a using
                    // in case the factory passed in throws, in which case we
                    // do not want to add the entry to the cache
                    entry.Dispose();
                }

                return result;
            };
        }

        [DebuggerStepThrough]
        public static Func<T1, T2, T3, TResult> Create<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"));
        }
        
        [DebuggerStepThrough]
        public static Func<T1, T2, T3, TResult> Create<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, MemoryCacheEntryOptions options)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"), options);
        }
        
        public static Func<T1, T2, T3, TResult> Create<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, IMemoryCache cache)
        {
            return (T1 t1, T2 t2, T3 t3) => cache.GetOrCreate((t1, t2, t3), ignored => func(t1, t2, t3));
        }

        public static Func<T1, T2, T3, TResult> Create<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, IMemoryCache cache, MemoryCacheEntryOptions options)
        {
            return (T1 t1, T2 t2, T3 t3) =>
            {
                var key = (t1, t2, t3);
                if (!cache.TryGetValue<TResult>(key, out var result))
                {
                    var entry = cache.CreateEntry(key);
                    result = func(t1, t2, t3);
                    entry.SetOptions(options);
                    entry.SetValue(result);
                    // need to manually call dispose instead of having a using
                    // in case the factory passed in throws, in which case we
                    // do not want to add the entry to the cache
                    entry.Dispose();
                }

                return result;
            };
        }

        [DebuggerStepThrough]
        public static Func<T1, T2, T3, T4, TResult> Create<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"));
        }
        
        [DebuggerStepThrough]
        public static Func<T1, T2, T3, T4, TResult> Create<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, MemoryCacheEntryOptions options)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"), options);
        }
        
        public static Func<T1, T2, T3, T4, TResult> Create<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, IMemoryCache cache)
        {
            return (T1 t1, T2 t2, T3 t3, T4 t4) => cache.GetOrCreate((t1, t2, t3, t4), ignored => func(t1, t2, t3, t4));
        }

        public static Func<T1, T2, T3, T4, TResult> Create<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, IMemoryCache cache, MemoryCacheEntryOptions options)
        {
            return (T1 t1, T2 t2, T3 t3, T4 t4) =>
            {
                var key = (t1, t2, t3, t4);
                if (!cache.TryGetValue<TResult>(key, out var result))
                {
                    var entry = cache.CreateEntry(key);
                    result = func(t1, t2, t3, t4);
                    entry.SetOptions(options);
                    entry.SetValue(result);
                    // need to manually call dispose instead of having a using
                    // in case the factory passed in throws, in which case we
                    // do not want to add the entry to the cache
                    entry.Dispose();
                }

                return result;
            };
        }

        [DebuggerStepThrough]
        public static Func<T1, T2, T3, T4, T5, TResult> Create<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"));
        }
        
        [DebuggerStepThrough]
        public static Func<T1, T2, T3, T4, T5, TResult> Create<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, MemoryCacheEntryOptions options)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"), options);
        }
        
        public static Func<T1, T2, T3, T4, T5, TResult> Create<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, IMemoryCache cache)
        {
            return (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) => cache.GetOrCreate((t1, t2, t3, t4, t5), ignored => func(t1, t2, t3, t4, t5));
        }

        public static Func<T1, T2, T3, T4, T5, TResult> Create<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, IMemoryCache cache, MemoryCacheEntryOptions options)
        {
            return (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) =>
            {
                var key = (t1, t2, t3, t4, t5);
                if (!cache.TryGetValue<TResult>(key, out var result))
                {
                    var entry = cache.CreateEntry(key);
                    result = func(t1, t2, t3, t4, t5);
                    entry.SetOptions(options);
                    entry.SetValue(result);
                    // need to manually call dispose instead of having a using
                    // in case the factory passed in throws, in which case we
                    // do not want to add the entry to the cache
                    entry.Dispose();
                }

                return result;
            };
        }

        [DebuggerStepThrough]
        public static Func<T1, T2, T3, T4, T5, T6, TResult> Create<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"));
        }
        
        [DebuggerStepThrough]
        public static Func<T1, T2, T3, T4, T5, T6, TResult> Create<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func, MemoryCacheEntryOptions options)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"), options);
        }
        
        public static Func<T1, T2, T3, T4, T5, T6, TResult> Create<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func, IMemoryCache cache)
        {
            return (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) => cache.GetOrCreate((t1, t2, t3, t4, t5, t6), ignored => func(t1, t2, t3, t4, t5, t6));
        }

        public static Func<T1, T2, T3, T4, T5, T6, TResult> Create<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func, IMemoryCache cache, MemoryCacheEntryOptions options)
        {
            return (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) =>
            {
                var key = (t1, t2, t3, t4, t5, t6);
                if (!cache.TryGetValue<TResult>(key, out var result))
                {
                    var entry = cache.CreateEntry(key);
                    result = func(t1, t2, t3, t4, t5, t6);
                    entry.SetOptions(options);
                    entry.SetValue(result);
                    // need to manually call dispose instead of having a using
                    // in case the factory passed in throws, in which case we
                    // do not want to add the entry to the cache
                    entry.Dispose();
                }

                return result;
            };
        }

        [DebuggerStepThrough]
        public static Func<T1, T2, T3, T4, T5, T6, T7, TResult> Create<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"));
        }
        
        [DebuggerStepThrough]
        public static Func<T1, T2, T3, T4, T5, T6, T7, TResult> Create<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, MemoryCacheEntryOptions options)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"), options);
        }
        
        public static Func<T1, T2, T3, T4, T5, T6, T7, TResult> Create<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, IMemoryCache cache)
        {
            return (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) => cache.GetOrCreate((t1, t2, t3, t4, t5, t6, t7), ignored => func(t1, t2, t3, t4, t5, t6, t7));
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, TResult> Create<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> func, IMemoryCache cache, MemoryCacheEntryOptions options)
        {
            return (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) =>
            {
                var key = (t1, t2, t3, t4, t5, t6, t7);
                if (!cache.TryGetValue<TResult>(key, out var result))
                {
                    var entry = cache.CreateEntry(key);
                    result = func(t1, t2, t3, t4, t5, t6, t7);
                    entry.SetOptions(options);
                    entry.SetValue(result);
                    // need to manually call dispose instead of having a using
                    // in case the factory passed in throws, in which case we
                    // do not want to add the entry to the cache
                    entry.Dispose();
                }

                return result;
            };
        }

        [DebuggerStepThrough]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"));
        }
        
        [DebuggerStepThrough]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, MemoryCacheEntryOptions options)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"), options);
        }
        
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, IMemoryCache cache)
        {
            return (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8) => cache.GetOrCreate((t1, t2, t3, t4, t5, t6, t7, t8), ignored => func(t1, t2, t3, t4, t5, t6, t7, t8));
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func, IMemoryCache cache, MemoryCacheEntryOptions options)
        {
            return (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8) =>
            {
                var key = (t1, t2, t3, t4, t5, t6, t7, t8);
                if (!cache.TryGetValue<TResult>(key, out var result))
                {
                    var entry = cache.CreateEntry(key);
                    result = func(t1, t2, t3, t4, t5, t6, t7, t8);
                    entry.SetOptions(options);
                    entry.SetValue(result);
                    // need to manually call dispose instead of having a using
                    // in case the factory passed in throws, in which case we
                    // do not want to add the entry to the cache
                    entry.Dispose();
                }

                return result;
            };
        }

        [DebuggerStepThrough]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"));
        }
        
        [DebuggerStepThrough]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, MemoryCacheEntryOptions options)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"), options);
        }
        
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, IMemoryCache cache)
        {
            return (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9) => cache.GetOrCreate((t1, t2, t3, t4, t5, t6, t7, t8, t9), ignored => func(t1, t2, t3, t4, t5, t6, t7, t8, t9));
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func, IMemoryCache cache, MemoryCacheEntryOptions options)
        {
            return (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9) =>
            {
                var key = (t1, t2, t3, t4, t5, t6, t7, t8, t9);
                if (!cache.TryGetValue<TResult>(key, out var result))
                {
                    var entry = cache.CreateEntry(key);
                    result = func(t1, t2, t3, t4, t5, t6, t7, t8, t9);
                    entry.SetOptions(options);
                    entry.SetValue(result);
                    // need to manually call dispose instead of having a using
                    // in case the factory passed in throws, in which case we
                    // do not want to add the entry to the cache
                    entry.Dispose();
                }

                return result;
            };
        }

        [DebuggerStepThrough]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"));
        }
        
        [DebuggerStepThrough]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, MemoryCacheEntryOptions options)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"), options);
        }
        
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, IMemoryCache cache)
        {
            return (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10) => cache.GetOrCreate((t1, t2, t3, t4, t5, t6, t7, t8, t9, t10), ignored => func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10));
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func, IMemoryCache cache, MemoryCacheEntryOptions options)
        {
            return (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10) =>
            {
                var key = (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
                if (!cache.TryGetValue<TResult>(key, out var result))
                {
                    var entry = cache.CreateEntry(key);
                    result = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
                    entry.SetOptions(options);
                    entry.SetValue(result);
                    // need to manually call dispose instead of having a using
                    // in case the factory passed in throws, in which case we
                    // do not want to add the entry to the cache
                    entry.Dispose();
                }

                return result;
            };
        }

        [DebuggerStepThrough]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"));
        }
        
        [DebuggerStepThrough]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, MemoryCacheEntryOptions options)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"), options);
        }
        
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, IMemoryCache cache)
        {
            return (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11) => cache.GetOrCreate((t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11), ignored => func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11));
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> func, IMemoryCache cache, MemoryCacheEntryOptions options)
        {
            return (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11) =>
            {
                var key = (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
                if (!cache.TryGetValue<TResult>(key, out var result))
                {
                    var entry = cache.CreateEntry(key);
                    result = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
                    entry.SetOptions(options);
                    entry.SetValue(result);
                    // need to manually call dispose instead of having a using
                    // in case the factory passed in throws, in which case we
                    // do not want to add the entry to the cache
                    entry.Dispose();
                }

                return result;
            };
        }

        [DebuggerStepThrough]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"));
        }
        
        [DebuggerStepThrough]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, MemoryCacheEntryOptions options)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"), options);
        }
        
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, IMemoryCache cache)
        {
            return (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12) => cache.GetOrCreate((t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12), ignored => func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12));
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> func, IMemoryCache cache, MemoryCacheEntryOptions options)
        {
            return (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12) =>
            {
                var key = (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
                if (!cache.TryGetValue<TResult>(key, out var result))
                {
                    var entry = cache.CreateEntry(key);
                    result = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
                    entry.SetOptions(options);
                    entry.SetValue(result);
                    // need to manually call dispose instead of having a using
                    // in case the factory passed in throws, in which case we
                    // do not want to add the entry to the cache
                    entry.Dispose();
                }

                return result;
            };
        }

        [DebuggerStepThrough]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"));
        }
        
        [DebuggerStepThrough]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, MemoryCacheEntryOptions options)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"), options);
        }
        
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, IMemoryCache cache)
        {
            return (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13) => cache.GetOrCreate((t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13), ignored => func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13));
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> func, IMemoryCache cache, MemoryCacheEntryOptions options)
        {
            return (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13) =>
            {
                var key = (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
                if (!cache.TryGetValue<TResult>(key, out var result))
                {
                    var entry = cache.CreateEntry(key);
                    result = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
                    entry.SetOptions(options);
                    entry.SetValue(result);
                    // need to manually call dispose instead of having a using
                    // in case the factory passed in throws, in which case we
                    // do not want to add the entry to the cache
                    entry.Dispose();
                }

                return result;
            };
        }

        [DebuggerStepThrough]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"));
        }
        
        [DebuggerStepThrough]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, MemoryCacheEntryOptions options)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"), options);
        }
        
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, IMemoryCache cache)
        {
            return (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14) => cache.GetOrCreate((t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14), ignored => func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14));
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> func, IMemoryCache cache, MemoryCacheEntryOptions options)
        {
            return (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14) =>
            {
                var key = (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
                if (!cache.TryGetValue<TResult>(key, out var result))
                {
                    var entry = cache.CreateEntry(key);
                    result = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
                    entry.SetOptions(options);
                    entry.SetValue(result);
                    // need to manually call dispose instead of having a using
                    // in case the factory passed in throws, in which case we
                    // do not want to add the entry to the cache
                    entry.Dispose();
                }

                return result;
            };
        }

        [DebuggerStepThrough]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"));
        }
        
        [DebuggerStepThrough]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, MemoryCacheEntryOptions options)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"), options);
        }
        
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, IMemoryCache cache)
        {
            return (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15) => cache.GetOrCreate((t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15), ignored => func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15));
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> func, IMemoryCache cache, MemoryCacheEntryOptions options)
        {
            return (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15) =>
            {
                var key = (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
                if (!cache.TryGetValue<TResult>(key, out var result))
                {
                    var entry = cache.CreateEntry(key);
                    result = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
                    entry.SetOptions(options);
                    entry.SetValue(result);
                    // need to manually call dispose instead of having a using
                    // in case the factory passed in throws, in which case we
                    // do not want to add the entry to the cache
                    entry.Dispose();
                }

                return result;
            };
        }

        [DebuggerStepThrough]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"));
        }
        
        [DebuggerStepThrough]
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, MemoryCacheEntryOptions options)
        {
            return Create(func, DefaultCache ?? throw new InvalidOperationException("Memoization.DefaultCache is null"), options);
        }
        
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, IMemoryCache cache)
        {
            return (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16) => cache.GetOrCreate((t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16), ignored => func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16));
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> func, IMemoryCache cache, MemoryCacheEntryOptions options)
        {
            return (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16) =>
            {
                var key = (t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16);
                if (!cache.TryGetValue<TResult>(key, out var result))
                {
                    var entry = cache.CreateEntry(key);
                    result = func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16);
                    entry.SetOptions(options);
                    entry.SetValue(result);
                    // need to manually call dispose instead of having a using
                    // in case the factory passed in throws, in which case we
                    // do not want to add the entry to the cache
                    entry.Dispose();
                }

                return result;
            };
        }

    }
}
