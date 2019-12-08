// <copyright file="PlcFrameHeader.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace GeothermalResearchInstitute.PlcV2
{
    public class PlcFrameHeader
    {
        public PlcMessageType MessageType { get; set; }

        public uint SequenceNumber { get; set; }

        public uint Crc32cChecksum { get; set; }

        public ushort ContentOffset { get; set; }

        public ushort ContentLength { get; set; }

        public static PlcFrameHeader Parse(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length != 20)
            {
                throw new ArgumentException("Header must has 20 bytes", nameof(bytes));
            }

            byte version = bytes[0x04];
            if (version != 0x02)
            {
                throw new InvalidDataException("Header version must be 2, but " + version + " received.");
            }

            return new PlcFrameHeader
            {
                MessageType = (PlcMessageType)BinaryPrimitives.ReadUInt16BigEndian(bytes.Slice(0x06, 2)),
                SequenceNumber = BinaryPrimitives.ReadUInt32BigEndian(bytes.Slice(0x08, 4)),
                Crc32cChecksum = BinaryPrimitives.ReadUInt32BigEndian(bytes.Slice(0x0C, 4)),
                ContentOffset = BinaryPrimitives.ReadUInt16BigEndian(bytes.Slice(0x10, 2)),
                ContentLength = BinaryPrimitives.ReadUInt16BigEndian(bytes.Slice(0x12, 2)),
            };
        }

        public void WriteTo(Stream outputStream)
        {
            using var writer = new BinaryWriter(outputStream, Encoding.ASCII, true);

            writer.Write('D');
            writer.Write('R');
            writer.Write('Y');
            writer.Write((byte)0x00);

            writer.Write((byte)0x02);
            writer.Write((byte)0x00);
            writer.Write(BinaryPrimitives.ReverseEndianness((ushort)this.MessageType));

            writer.Write(BinaryPrimitives.ReverseEndianness(this.SequenceNumber));

            writer.Write(BinaryPrimitives.ReverseEndianness(this.Crc32cChecksum));

            writer.Write(BinaryPrimitives.ReverseEndianness(this.ContentOffset));
            writer.Write(BinaryPrimitives.ReverseEndianness(this.ContentLength));
        }
    }
}
