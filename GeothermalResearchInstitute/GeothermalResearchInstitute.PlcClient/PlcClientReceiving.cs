// <copyright file="PlcClientReceiving.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GeothermalResearchInstitute.v2;
using Microsoft.Extensions.Logging;
using Overby.Extensions.AsyncBinaryReaderWriter;

namespace GeothermalResearchInstitute.Plc
{
    public partial class PlcClient
    {
        private async void BackgroundReceivingTaskEntryPoint()
        {
            try
            {
                using var binaryReader = new AsyncBinaryReader(this.client.GetStream(), Encoding.ASCII, true);

                while (!this.stoppedEvent.WaitOne(1000))
                {
                    await this.DrainReceivingContexts(binaryReader).ConfigureAwait(false);
                }

                await this.DrainReceivingContexts(binaryReader).ConfigureAwait(false);
            }
            catch (IOException e)
            {
                this.logger.LogError(e, "Error when receiving data from {0}", this.remoteEndPoint);
                this.client.Close();
            }
        }

        private async Task DrainReceivingContexts(AsyncBinaryReader binaryReader)
        {
            while (!(this.receivingContexts.IsEmpty && this.requestQueue.Count == 0))
            {
                byte[] frameHeaderBytes = await binaryReader.ReadBytesAsync(20).ConfigureAwait(false);
                if (frameHeaderBytes.Length < 20)
                {
                    throw new IOException("Socket closed during reading.");
                }

                var frameHeader = FrameHeader.Parse(frameHeaderBytes);
                if (frameHeader.ContentOffset != 20)
                {
                    this.logger.LogCritical(
                        "Invalid frame header with unexpected content offset, expected=20 actual={0}",
                        frameHeader.ContentOffset);
                    throw new InvalidDataException("Invalid frame header");
                }

                if (frameHeader.Version != 1)
                {
                    this.logger.LogWarning(
                        "Invalid frame header with unexpected version {0}",
                        frameHeader.Version);
                    throw new InvalidDataException("Invalid frame header");
                }

                byte[] framePayloadBytes = await binaryReader
                    .ReadBytesAsync(frameHeader.ContentLength)
                    .ConfigureAwait(false);
                if (framePayloadBytes.Length < frameHeader.ContentLength)
                {
                    throw new IOException("Socket closed during reading.");
                }

                if (this.logger.IsEnabled(LogLevel.Debug))
                {
                    var buffer = new byte[frameHeaderBytes.Length + framePayloadBytes.Length];
                    Buffer.BlockCopy(frameHeaderBytes, 0, buffer, 0, frameHeaderBytes.Length);
                    Buffer.BlockCopy(framePayloadBytes, 0, buffer, frameHeaderBytes.Length, framePayloadBytes.Length);

                    this.logger.LogDebug(
                        "Received streamId={0}:" + Environment.NewLine + "{1}",
                        frameHeader.StreamId,
                        HexUtils.Dump(buffer.AsMemory()));
                }

                if (this.receivingContexts.TryRemove((int)frameHeader.StreamId, out RequestContext requestContext))
                {
                    this.Process(requestContext, frameHeader, framePayloadBytes);
                }
                else
                {
                    this.logger.LogWarning("Dropped a frame with unexpected streamId {0}", frameHeader.StreamId);
                }
            }
        }

        private void Process(RequestContext requestContext, FrameHeader frameHeader, byte[] framePayloadBytes)
        {
            if (frameHeader.Type != 2)
            {
                this.logger.LogWarning("Dropped a frame with unexpected type {0}", frameHeader.Type);
                requestContext.TaskCompletionSource.SetException(new InvalidDataException("Invalid data received"));
                return;
            }

            var expectedChecksum = Crc32C.Crc32CAlgorithm.Compute(framePayloadBytes);
            if (frameHeader.ContentChecksum != expectedChecksum)
            {
                this.logger.LogWarning(
                    "Dropped a frame with unexpected checksum {0}(expected {1})",
                    frameHeader.ContentChecksum,
                    expectedChecksum);
                requestContext.TaskCompletionSource.SetException(new InvalidDataException("Invalid data received"));
                return;
            }

            // TODO(zhangshuai.ustc): check deadline.
            var unifiedFrameContent = UnifiedFrameContent.Parser.ParseFrom(framePayloadBytes);
            requestContext.TaskCompletionSource.SetResult(unifiedFrameContent);
        }
    }
}
