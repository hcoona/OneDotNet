// <copyright file="FrameHeader.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Buffers.Binary;
using System.Text.Json;
using System.Threading.Tasks;
using Overby.Extensions.AsyncBinaryReaderWriter;

namespace GeothermalResearchInstitute.Plc
{
    internal class FrameHeader
    {
        public FrameHeader(
            byte version,
            byte type,
            byte sequenceNumber,
            uint streamId,
            ushort contentOffset,
            ushort contentLength,
            uint contentChecksum)
        {
            this.Version = version;
            this.Type = type;
            this.SequenceNumber = sequenceNumber;
            this.StreamId = streamId;
            this.ContentOffset = contentOffset;
            this.ContentLength = contentLength;
            this.ContentChecksum = contentChecksum;
        }

        public byte Version { get; }

        public byte Type { get; }

        public byte SequenceNumber { get; }

        public uint StreamId { get; }

        public ushort ContentOffset { get; }

        public ushort ContentLength { get; }

        public uint ContentChecksum { get; }

        public static FrameHeader Parse(ReadOnlySpan<byte> buffer)
        {
            return new FrameHeader(
                version: buffer[4],
                type: buffer[5],
                sequenceNumber: buffer[6],
                streamId: BinaryPrimitives.ReadUInt32BigEndian(buffer.Slice(8, 4)),
                contentOffset: BinaryPrimitives.ReadUInt16BigEndian(buffer.Slice(12, 2)),
                contentLength: BinaryPrimitives.ReadUInt16BigEndian(buffer.Slice(14, 2)),
                contentChecksum: BinaryPrimitives.ReadUInt32BigEndian(buffer.Slice(16, 4)));
        }

        public void WriteTo(Span<byte> buffer)
        {
            // Write magic signature.
            buffer[0] = (byte)'D';
            buffer[1] = (byte)'R';
            buffer[2] = (byte)'Y';
            buffer[3] = 0;

            // Write version, type, sequence number.
            buffer[4] = this.Version;
            buffer[5] = this.Type;
            buffer[6] = this.SequenceNumber;
            buffer[7] = 0;

            // Write stream id.
            BinaryPrimitives.WriteUInt32BigEndian(buffer.Slice(8, 4), this.StreamId);

            // Write content offset & size.
            BinaryPrimitives.WriteUInt16BigEndian(buffer.Slice(12, 2), this.ContentOffset);
            BinaryPrimitives.WriteUInt16BigEndian(buffer.Slice(14, 2), this.ContentLength);

            // Write content checksum.
            BinaryPrimitives.WriteUInt32BigEndian(buffer.Slice(16, 4), this.ContentChecksum);
        }

        public async Task WriteTo(AsyncBinaryWriter binaryWriter)
        {
            // Write magic signature.
            await binaryWriter.WriteAsync((byte)'D').ConfigureAwait(false);
            await binaryWriter.WriteAsync((byte)'R').ConfigureAwait(false);
            await binaryWriter.WriteAsync((byte)'Y').ConfigureAwait(false);
            await binaryWriter.WriteAsync((byte)0).ConfigureAwait(false);

            // Write version, type, sequence number.
            await binaryWriter.WriteAsync((byte)this.Version).ConfigureAwait(false);
            await binaryWriter.WriteAsync((byte)this.Type).ConfigureAwait(false);
            await binaryWriter.WriteAsync((byte)this.SequenceNumber).ConfigureAwait(false);
            await binaryWriter.WriteAsync((byte)0).ConfigureAwait(false);

            // Write stream id.
            await binaryWriter
                .WriteAsync(BinaryPrimitives.ReverseEndianness((uint)this.StreamId))
                .ConfigureAwait(false);

            // Write content offset & size.
            await binaryWriter
                .WriteAsync(BinaryPrimitives.ReverseEndianness((ushort)this.ContentOffset))
                .ConfigureAwait(false);
            await binaryWriter
                .WriteAsync(BinaryPrimitives.ReverseEndianness((ushort)this.ContentLength))
                .ConfigureAwait(false);

            // Write content checksum.
            await binaryWriter
                .WriteAsync(BinaryPrimitives.ReverseEndianness((uint)this.ContentChecksum))
                .ConfigureAwait(false);
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
