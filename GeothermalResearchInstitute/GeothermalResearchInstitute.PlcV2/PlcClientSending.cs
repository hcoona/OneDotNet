// <copyright file="PlcClientSending.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GeothermalResearchInstitute.PlcV2
{
    public partial class PlcClient
    {
        private async void SendingBackgroundTaskEntryPoint()
        {
            try
            {
                while (await this.requestContextSendingBufferBlock
                    .OutputAvailableAsync()
                    .ConfigureAwait(false))
                {
                    DateTime utcNow = DateTime.UtcNow;
                    while (this.requestContextSendingBufferBlock.TryReceive(out PlcRequestContext requestContext))
                    {
                        this.ProcessRequest(utcNow, requestContext);
                    }
                }
            }
            catch (IOException e)
            {
                this.logger.LogError(e, "Failed to send message to {0}", this.RemoteEndPoint);
                this.Close().ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        private void ProcessRequest(DateTime utcNow, PlcRequestContext requestContext)
        {
            if (requestContext.Deadline?.ToUniversalTime() < utcNow)
            {
                requestContext.TaskCompletionSource.SetException(new RpcException(
                    new Status(StatusCode.DeadlineExceeded, string.Empty)));
                return;
            }

            int sequenceNumber = Interlocked.Increment(ref this.sequenceNumberGenerator);
            requestContext.RequestFrame.FrameHeader.SequenceNumber = (uint)sequenceNumber;

            if (!this.requestContextReceivingDictionary.TryAdd(sequenceNumber, requestContext))
            {
                throw new InvalidOperationException();  // Should not happen.
            }

            if (this.logger.IsEnabled(LogLevel.Debug))
            {
                using var stream = new MemoryStream();
                requestContext.RequestFrame.WriteTo(stream);
                this.logger.LogDebug(
                    "Sending {0} to {1}{2}{3}",
                    sequenceNumber,
                    this.RemoteEndPoint,
                    Environment.NewLine,
                    HexUtils.Dump(stream.ToArray()));
                this.OnDebugSending?.Invoke(this, stream.ToArray());

                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(this.tcpClient.GetStream());
            }
            else
            {
                requestContext.RequestFrame.WriteTo(this.tcpClient.GetStream());
            }
        }
    }
}
