// <copyright file="PlcFrame.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.IO;
using Google.Protobuf;

namespace GeothermalResearchInstitute.PlcV2
{
    public class PlcFrame
    {
        public PlcFrameHeader FrameHeader { get; set; }

        public ByteString FrameBody { get; set; }

        public static PlcFrame Create(PlcMessageType messageType, ByteString messageBody)
        {
            if (messageBody is null)
            {
                throw new System.ArgumentNullException(nameof(messageBody));
            }

            return new PlcFrame
            {
                FrameHeader = new PlcFrameHeader
                {
                    MessageType = messageType,
                },
                FrameBody = messageBody,
            };
        }

        public void WriteTo(Stream outputStream)
        {
            this.FrameHeader.Crc32cChecksum = this.FrameBody.IsEmpty
                ? 0
                : Crc32C.Crc32CAlgorithm.Compute(this.FrameBody.ToByteArray());
            this.FrameHeader.ContentOffset = 0x14;
            this.FrameHeader.ContentLength = (ushort)this.FrameBody.Length;

            this.FrameHeader.WriteTo(outputStream);
            this.FrameBody.WriteTo(outputStream);
        }
    }
}
