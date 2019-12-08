// <copyright file="PlcClientReceiving.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;
using System.Text;
using System.Threading;
using Google.Protobuf;
using Microsoft.Extensions.Logging;

namespace GeothermalResearchInstitute.PlcV2
{
    public partial class PlcClient
    {
        private void ReceivingBackgroundTaskEntryPoint()
        {
            CancellationToken closingCancellationToken = this.closingCancellationTokenSource.Token;
            using var reader = new BinaryReader(this.tcpClient.GetStream(), Encoding.ASCII, true);
            try
            {
                while (!closingCancellationToken.IsCancellationRequested)
                {
                    byte[] headerBytes = reader.ReadBytes(20);
                    if (headerBytes.Length != 20)
                    {
                        throw new IOException("Failed to read frame header");
                    }

                    var header = PlcFrameHeader.Parse(headerBytes);
                    if (header.ContentOffset != 20)
                    {
                        throw new InvalidDataException("Content offset is not 20");
                    }

                    byte[] bodyBytes = reader.ReadBytes(header.ContentLength);
                    if (bodyBytes.Length != header.ContentLength)
                    {
                        throw new IOException("Failed to read frame body");
                    }

                    if (this.logger.IsEnabled(LogLevel.Debug))
                    {
                        byte[] buffer = new byte[headerBytes.Length + bodyBytes.Length];
                        Buffer.BlockCopy(headerBytes, 0, buffer, 0, headerBytes.Length);
                        Buffer.BlockCopy(bodyBytes, 0, buffer, headerBytes.Length, bodyBytes.Length);

                        this.logger.LogDebug(
                            "Received {0} from {1}{2}{3}",
                            header.SequenceNumber,
                            this.RemoteEndPoint,
                            Environment.NewLine,
                            HexUtils.Dump(buffer));
                        this.OnDebugReceiving?.Invoke(this, buffer);
                    }

                    if (this.requestContextReceivingDictionary.TryRemove((int)header.SequenceNumber, out PlcRequestContext requestContext))
                    {
                        requestContext.TaskCompletionSource.TrySetResult(new PlcFrame
                        {
                            FrameHeader = header,
                            FrameBody = ByteString.CopyFrom(bodyBytes),
                        });
                    }
                    else
                    {
                        this.logger.LogWarning("Received unknown sequence number from {0}", this.RemoteEndPoint);
                    }
                }
            }
            catch (IOException e)
            {
                this.logger.LogError(e, "Failed to receiving from {0}", this.RemoteEndPoint);
                this.Close();
            }
            catch (InvalidDataException e)
            {
                this.logger.LogError(e, "Received invalid data from {0}", this.RemoteEndPoint);
                this.Close();
            }
        }
    }
}
