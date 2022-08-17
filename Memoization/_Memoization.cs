using Microsoft.Extensions.Caching.Memory;

namespace Memoization
{
    public static partial class Memoization
    {
        public static IMemoryCache DefaultCache { get; set; }
    }
}
