// <copyright file="FakePlc.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Buffers.Binary;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GeothermalResearchInstitute.PlcV2;
using Google.Protobuf;

namespace GeothermalResearchInstitute.FakePlcV2
{
    public class FakePlc : IDisposable
    {
        private TcpClient client;
        private CancellationTokenSource cancellationTokenSource;
        private Task backgroundTask;
        private bool disposedValue = false;

        ~FakePlc()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task StartAsync(IPAddress address, int port)
        {
            this.client = new TcpClient();
            await this.client.ConnectAsync(address, port).ConfigureAwait(false);

            this.cancellationTokenSource = new CancellationTokenSource();
            this.backgroundTask = Task.Factory.StartNew(
                this.BackgroundTaskEntryPoint,
                this.cancellationTokenSource.Token,
                TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default);
        }

        public async Task StopAsync()
        {
            this.cancellationTokenSource.Cancel();
            await this.backgroundTask.ConfigureAwait(false);
            this.client.Close();

            this.backgroundTask.Dispose();
            this.backgroundTask = null;
            this.cancellationTokenSource.Dispose();
            this.cancellationTokenSource = null;
            this.client.Dispose();
            this.client = null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    if (this.backgroundTask != null)
                    {
                        this.StopAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                    }
                }

                this.disposedValue = true;
            }
        }

        private void BackgroundTaskEntryPoint()
        {
            using var reader = new BinaryReader(this.client.GetStream(), Encoding.ASCII, true);
            while (!this.cancellationTokenSource.IsCancellationRequested)
            {
                byte[] header = reader.ReadBytes(20);
                ushort contentLength = BinaryPrimitives.ReadUInt16BigEndian(header.AsSpan(0x12, 2));
                byte[] content = contentLength == 0 ? Array.Empty<byte>() : reader.ReadBytes(contentLength);

                var messageType = (PlcMessageType)BinaryPrimitives.ReadUInt16BigEndian(header.AsSpan(0x06, 2));
                PlcFrame responseFrame = null;
                if (messageType == PlcMessageType.ConnectRequest)
                {
                    responseFrame = PlcFrame.Create(
                        PlcMessageType.ConnectResponse,
                        ByteString.CopyFrom(PhysicalAddress.Parse("10BF4879B2A4").GetAddressBytes()));
                }

                if (responseFrame == null)
                {
                    continue;
                }

                responseFrame.FrameHeader.SequenceNumber =
                    BinaryPrimitives.ReadUInt32BigEndian(header.AsSpan(0x08, 4));
                responseFrame.WriteTo(this.client.GetStream());
            }
        }
    }
}
