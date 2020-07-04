// <copyright file="Program.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Buffers.Binary;

namespace ParseHeader
{
    internal class Program
    {
        internal static void Main()
        {
            byte[] headerBytes = new byte[]
            {
                0x32, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x32, 0x30, 0x32, 0x30, 0x30, 0x36, 0x32, 0x38,
                0x31, 0x31, 0x33, 0x36, 0x31, 0x34, 0x30, 0x31,
            };
            ulong s1 = BinaryPrimitives.ReadUInt64BigEndian(new ReadOnlySpan<byte>(headerBytes, 0, 8));
            ulong s2 = BinaryPrimitives.ReadUInt64BigEndian(new ReadOnlySpan<byte>(headerBytes, 8, 8));
            ulong s3 = BinaryPrimitives.ReadUInt64BigEndian(new ReadOnlySpan<byte>(headerBytes, 16, 8));
            Console.WriteLine($"s1 = {s1}");
            Console.WriteLine($"s2 = {s2}");
            Console.WriteLine($"s3 = {s3}");
        }
    }
}
