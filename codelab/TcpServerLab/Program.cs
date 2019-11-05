// <copyright file="Program.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Globalization;
using System.IO;
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
            WriteHeaderFrame(writer, streamId: 1, header: header);
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
            WriteDataFrame(writer, streamId: 1, contents: request.ToByteString().Span);
            Console.WriteLine("Sending data frame, content size = {0}, frame size = {1}", request.CalculateSize(), memoryStream.Position - dataStartingPosition);
            Console.WriteLine(HexUtils.Dump(memoryStream.GetBuffer()
                .AsMemory(dataStartingPosition, (int)(memoryStream.Position - dataStartingPosition))));
            Console.WriteLine();

            Stream inputStream;
            if (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                NetworkStream networkStream = client.GetStream();
                memoryStream.Seek(0, SeekOrigin.Begin);
                memoryStream.CopyTo(networkStream);
                inputStream = networkStream;
            }
            else
            {
                memoryStream.Seek(0, SeekOrigin.Begin);
                inputStream = memoryStream;
            }

            var bufferMemory = new Memory<byte>(new byte[8192]);
            ReadHeaderFrame(inputStream, bufferMemory.Span, out var headerFrameSpan, out var headerFrameHeader, out var headerFrameContent);
            Console.WriteLine("Receiving header frame, content size = {0}, frame size = {1}", headerFrameHeader.ContentLength, headerFrameSpan.Length);
            Console.WriteLine(HexUtils.Dump(bufferMemory.Slice(0, headerFrameSpan.Length)));
            Console.WriteLine();

            Console.WriteLine(headerFrameHeader.ToString());
            Console.WriteLine(headerFrameContent.ToString());
            Console.WriteLine();

            ReadDataFrame(inputStream, bufferMemory.Span, out var dataFrameSpan, out var dataFrameHeader, out var dataFrameContentSpan);
            Console.WriteLine("Receiving data frame, content size = {0}, frame size = {1}", dataFrameHeader.ContentLength, dataFrameSpan.Length);
            Console.WriteLine(HexUtils.Dump(bufferMemory.Slice(0, dataFrameSpan.Length)));
            Console.WriteLine();

            var response = TestResponse.Parser.ParseFrom(dataFrameContentSpan.ToArray());
            Console.WriteLine(dataFrameHeader.ToString());
            Console.WriteLine(response.ToString());
            Console.WriteLine();
        }

        private static void ReadHeaderFrame(Stream stream, Span<byte> buffer, out Span<byte> frameSpan, out FrameHeader frameHeader, out Header header)
        {
            ReadFrame(stream, buffer, out var frameHeaderSpan, out var frameContentSpan, out frameSpan, out frameHeader);
            header = Header.Parser.ParseFrom(frameContentSpan.ToArray());
        }

        private static void ReadDataFrame(Stream stream, Span<byte> buffer, out Span<byte> frameSpan, out FrameHeader frameHeader, out Span<byte> contentSpan)
        {
            ReadFrame(stream, buffer, out var frameHeaderSpan, out contentSpan, out frameSpan, out frameHeader);
        }

        private static void ReadFrame(Stream stream, Span<byte> buffer, out Span<byte> frameHeaderSpan, out Span<byte> frameContentSpan, out Span<byte> frameSpan, out FrameHeader frameHeader)
        {
            frameHeaderSpan = buffer.Slice(0, 20);
            ReadBytes(stream, frameHeaderSpan);
            frameHeader = FrameHeader.Parse(frameHeaderSpan);
            frameContentSpan = buffer.Slice(20, frameHeader.ContentLength);
            ReadBytes(stream, frameContentSpan);
            frameSpan = buffer.Slice(0, frameHeaderSpan.Length + frameContentSpan.Length);
        }

        private static void ReadBytes(Stream stream, Span<byte> buffer)
        {
            while (buffer.Length != 0)
            {
                int count = stream.Read(buffer);
                buffer = buffer.Slice(count);
            }
        }

        private static void WriteHeaderFrame(BinaryWriter writer, int streamId, Header header)
        {
            WriteFrame(writer, 1, 0, streamId, header.ToByteString().Span);
        }

        private static void WriteDataFrame(BinaryWriter writer, int streamId, ReadOnlySpan<byte> contents)
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
            uint contentChecksum = 0;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT || Environment.OSVersion.Platform == PlatformID.Win32Windows)
            {
                contentChecksum = Crc32C.Crc32CAlgorithm.Compute(contents.ToArray());
            }

            var frameHeaderBytes = new byte[20];
            var frameHeader = new FrameHeader(1, type, seqNum, (uint)streamId, 20, (ushort)contents.Length, contentChecksum);
            frameHeader.WriteTo(frameHeaderBytes);

            writer.Write(frameHeaderBytes);
            writer.Write(contents);
        }
    }
}
