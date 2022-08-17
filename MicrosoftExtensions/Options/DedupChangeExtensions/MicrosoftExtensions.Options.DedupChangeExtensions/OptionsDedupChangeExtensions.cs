using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Threading;

namespace Microsoft.Extensions.Options
{
    public static class OptionsDedupChangeExtensions
    {
        private readonly static ThreadLocal<IFormatter> formatterLocal =
            new ThreadLocal<IFormatter>(() => new BinaryFormatter());
        private readonly static ThreadLocal<HashAlgorithm> hashAlgorithmLocal =
            new ThreadLocal<HashAlgorithm>(SHA1.Create);

        public static IDisposable OnChangeDedup<TOptions>(
            this IOptionsMonitor<TOptions> monitor,
            string name,
            Action<TOptions, string> listener)
        {
            var originValueHashToken = GetHashToken(monitor.Get(name));
            return monitor.OnChange((newValue, key) =>
            {
                if (key == name)
                {
                    var newValueHashToken = GetHashToken(newValue);
                    var oldValueHashToken = Interlocked.Exchange(
                        ref originValueHashToken,
                        newValueHashToken);

                    if (!IsHashTokenEqual(oldValueHashToken, newValueHashToken))
                    {
                        listener(newValue, key);
                    }
                }
            });
        }

        public static IDisposable OnChangeDedup<TOptions>(
            this IOptionsMonitor<TOptions> monitor,
            Action<TOptions> listener)
        {
            return OnChangeDedup(
                monitor,
                Options.DefaultName,
                (options, _) => listener(options));
        }

        private static byte[] GetHashToken(object graph)
        {
            using (var stream = new MemoryStream())
            {
                formatterLocal.Value.Serialize(stream, graph);
                stream.Seek(0, SeekOrigin.Begin);
                return hashAlgorithmLocal.Value.ComputeHash(stream);
            }
        }

        private static bool IsHashTokenEqual(byte[] lhs, byte[] rhs)
        {
            return System.Linq.Enumerable.SequenceEqual(lhs, rhs);
        }
    }
}
