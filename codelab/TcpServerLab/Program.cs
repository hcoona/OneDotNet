// <copyright file="Program.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using GeothermalResearchInstitute.v2;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace TcpServerLab
{
    internal class Program
    {
        private static void Main()
        {
            var listener = TcpListener.Create(8888);
            listener.Start(20);
            Console.WriteLine("TCP Server is listening on port 8888");
            Console.WriteLine();

            using var memoryStream = new MemoryStream();
            using var writer = new BinaryWriter(memoryStream, Encoding.ASCII, true);
            var header = new Header { Path = "/bjdire.v2.DeviceService/Connect" };
            WriteHeader(writer, streamId: 1, header: header);
            Console.WriteLine("Sending header frame, content size = {0}, frame size = {1}", header.CalculateSize(), memoryStream.Position);
            Console.WriteLine(HexUtils.Dump(memoryStream.GetBuffer().AsMemory(0, (int)memoryStream.Position)));
            Console.WriteLine();

            var dataStartingPosition = (int)memoryStream.Position;
            var request = new TestRequest()
            {
                A = 42,
                B = 3.1415926F,
                C = "Hello World!",
                D = Timestamp.FromDateTimeOffset(DateTimeOffset.Parse("2019-10-29T21:42:13.00000+8:00", CultureInfo.InvariantCulture)),
            };
            WriteData(writer, streamId: 1, contents: request.ToByteString().Span);
            Console.WriteLine("Sending data frame, content size = {0}, frame size = {1}", request.CalculateSize(), memoryStream.Position - dataStartingPosition);
            Console.WriteLine(HexUtils.Dump(memoryStream.GetBuffer()
                .AsMemory(dataStartingPosition, (int)(memoryStream.Position - dataStartingPosition))));
            Console.WriteLine();

            using var client = listener.AcceptTcpClient();
            using var networkStream = client.GetStream();
            memoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.CopyTo(networkStream);

            using var reader = new BinaryReader(networkStream, Encoding.ASCII, true);
            var headerFrameHeaderBytes = reader.ReadBytes(20);
            var headerFrameHeader = FrameHeader.Parse(headerFrameHeaderBytes);
            var headerFrameContentBytes = reader.ReadBytes(headerFrameHeader.ContentLength);
        }

        private static void WriteHeader(BinaryWriter writer, int streamId, Header header)
        {
            WriteFrame(writer, 1, 0, streamId, header.ToByteString().Span);
        }

        private static void WriteData(BinaryWriter writer, int streamId, ReadOnlySpan<byte> contents)
        {
            const int trunkSize = 8192;
            int quot = Math.DivRem(contents.Length, trunkSize, out int rem);
            int trunksNum = rem == 0 ? quot : quot + 1;

            for (int i = 0; i < trunksNum; i++)
            {
                var offset = i * trunkSize;
                var length = Math.Min(contents.Length - offset, trunkSize);
                var trunk = contents.Slice(offset, length);
                WriteFrame(writer, 0, (byte)(i + 1), streamId, trunk);
            }
        }

        private static void WriteFrame(BinaryWriter writer, byte type, byte seqNum, int streamId, ReadOnlySpan<byte> contents)
        {
            uint contentChecksum = Crc32C.Crc32CAlgorithm.Compute(contents.ToArray());

            var frameHeaderBytes = new byte[20];
            var frameHeader = new FrameHeader(1, type, seqNum, (uint)streamId, 20, (ushort)contents.Length, contentChecksum);
            frameHeader.WriteTo(frameHeaderBytes);

            writer.Write(frameHeaderBytes);
            writer.Write(contents);
        }
    }
}
