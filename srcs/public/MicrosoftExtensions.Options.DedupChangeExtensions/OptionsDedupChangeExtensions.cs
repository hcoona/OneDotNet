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
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Threading;

namespace Microsoft.Extensions.Options
{
    public static class OptionsDedupChangeExtensions
    {
        private static readonly ThreadLocal<IFormatter> FormatterLocal =
            new ThreadLocal<IFormatter>(() => new BinaryFormatter());

        private static readonly ThreadLocal<HashAlgorithm> HashAlgorithmLocal =
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
                FormatterLocal.Value.Serialize(stream, graph);
                stream.Seek(0, SeekOrigin.Begin);
                return HashAlgorithmLocal.Value.ComputeHash(stream);
            }
        }

        private static bool IsHashTokenEqual(byte[] lhs, byte[] rhs)
        {
            return System.Linq.Enumerable.SequenceEqual(lhs, rhs);
        }
    }
}
