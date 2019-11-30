// <copyright file="PlcClientSending.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using GeothermalResearchInstitute.v2;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Overby.Extensions.AsyncBinaryReaderWriter;

namespace GeothermalResearchInstitute.Plc
{
    public partial class PlcClient
    {
        private static async Task WriteFrame(
            AsyncBinaryWriter writer,
            byte type,
            byte seqNum,
            int streamId,
            byte[] contents)
        {
            uint contentChecksum = 0;

            // TODO(zhangshuai.ustc): Support other platform
            if (Environment.OSVersion.Platform == PlatformID.Win32NT
                || Environment.OSVersion.Platform == PlatformID.Win32Windows)
            {
                contentChecksum = Crc32C.Crc32CAlgorithm.Compute(contents);
            }

            var frameHeader = new FrameHeader(
                version: 1,
                type: type,
                sequenceNumber: seqNum,
                streamId: (uint)streamId,
                contentOffset: 20,
                contentLength: (ushort)contents.Length,
                contentChecksum: contentChecksum);
            await frameHeader.WriteTo(writer).ConfigureAwait(false);
            await writer.WriteAsync(contents).ConfigureAwait(false);
        }

        private async void BackgroundSendingTaskEntryPoint()
        {
            try
            {
                using var binaryWriter = new AsyncBinaryWriter(this.client.GetStream(), Encoding.ASCII, true);
                while (await this.requestQueue
                        .OutputAvailableAsync()
                        .ConfigureAwait(false))
                {
                    await this.DrainSendingRequestContexts(binaryWriter).ConfigureAwait(false);
                }
            }
            catch (IOException e)
            {
                this.logger.LogError(e, "Error when receiving data from {0}", this.client.Client.RemoteEndPoint);
                this.client.Close();
            }
        }

        private async Task DrainSendingRequestContexts(AsyncBinaryWriter binaryWriter)
        {
            while (this.requestQueue.TryReceive(out RequestContext requestContext))
            {
                if (requestContext.CancellationToken.IsCancellationRequested
                    || DateTime.Now > requestContext.Deadline)
                {
                    requestContext.TaskCompletionSource.SetResult(new UnifiedFrameContent
                    {
                        Header = new Header
                        {
                            Status = (int)StatusCode.DeadlineExceeded,
                            StatusMessage = Status.DefaultCancelled.Detail,
                        },
                    });
                }

                var unifiedFrameContent = new UnifiedFrameContent()
                {
                    Header = new Header()
                    {
                        Path = requestContext.Path,
                    },
                    Payload = requestContext.Message.ToByteString(),
                };
                int streamId = Interlocked.Increment(ref this.counter);
                await this.WriteUnifiedFrameContent(binaryWriter, streamId, unifiedFrameContent)
                    .ConfigureAwait(false);
                if (!this.receivingContexts.TryAdd(streamId, requestContext))
                {
                    throw new InvalidOperationException("Impossible");
                }

                this.logger.LogDebug("Request recorded as streamId={0}: {1}", streamId, requestContext.Message);
            }
        }

        private async Task WriteUnifiedFrameContent(
            AsyncBinaryWriter writer,
            int streamId,
            UnifiedFrameContent unifiedFrameContent)
        {
            if (this.logger.IsEnabled(LogLevel.Debug))
            {
                using var memoryStream = new MemoryStream();
                using var memoryWriter = new AsyncBinaryWriter(memoryStream, Encoding.ASCII, false);

                await WriteFrame(memoryWriter, 2, 0, streamId, unifiedFrameContent.ToByteArray())
                    .ConfigureAwait(false);
                byte[] sendingBytes = memoryStream.ToArray();

                this.logger.LogDebug(
                    "Sending to {0} with streamId={1}:" + Environment.NewLine + "{2}",
                    unifiedFrameContent.Header.Path,
                    streamId,
                    HexUtils.Dump(new ReadOnlyMemory<byte>(sendingBytes)));
                await writer.WriteAsync(sendingBytes).ConfigureAwait(false);
            }
            else
            {
                await WriteFrame(writer, 2, 0, streamId, unifiedFrameContent.ToByteArray())
                    .ConfigureAwait(false);
            }
        }
    }
}
