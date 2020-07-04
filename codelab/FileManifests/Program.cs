// <copyright file="Program.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace FileManifests
{
    internal class Program
    {
        internal static void Main()
        {
            var options = new EnumerationOptions
            {
                RecurseSubdirectories = true,
                ReturnSpecialDirectories = false,
            };

            long counter = 0;
            using var sha512 = new ThreadLocal<SHA512>(() => SHA512.Create());
            using StreamWriter writer = File.CreateText("manifest.csv");
            writer.WriteLine("\"" + string.Join(
                    "\",\"",
                    nameof(FileInfo.Name),
                    nameof(FileInfo.Directory.FullName),
                    nameof(FileInfo.Length),
                    nameof(FileInfo.CreationTimeUtc),
                    nameof(FileInfo.LastWriteTimeUtc),
                    "SHA512") + "\"");
            foreach (FileInfo f in new DirectoryInfo(".")
                .EnumerateFiles(string.Empty, SearchOption.AllDirectories)
                .AsParallel())
            {
                string sha512Hex;
                try
                {
                    using FileStream stream = f.OpenRead();
                    sha512Hex = ByteArrayToHexString(sha512.Value.ComputeHash(stream));
                }
                catch (IOException e)
                {
                    sha512Hex = string.Empty;
                    Console.WriteLine("WARN {0}", e);
                }

                writer.WriteLine("\"" + string.Join(
                    "\",\"",
                    f.Name,
                    f.Directory.FullName,
                    f.Length,
                    new DateTimeOffset(f.CreationTimeUtc).ToUnixTimeSeconds(),
                    new DateTimeOffset(f.LastWriteTimeUtc).ToUnixTimeSeconds(),
                    sha512Hex) + "\"");

                long current = Interlocked.Increment(ref counter);
                if (current % 1000 == 0)
                {
                    Console.WriteLine($"INFO {current} files proceeded.");
                }
            }

            Console.WriteLine($"INFO All {counter} files proceeded.");
        }

        private static string ByteArrayToHexString(byte[] bytes)
        {
            var result = new StringBuilder(bytes.Length * 2);
            string hexAlphabet = "0123456789ABCDEF";

            foreach (byte b in bytes)
            {
                result.Append(hexAlphabet[(int)(b >> 4)]);
                result.Append(hexAlphabet[(int)(b & 0xF)]);
            }

            return result.ToString();
        }
    }
}
