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
using GeothermalResearchInstitute.v2;
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

        public Metric Metric { get; } = new Metric
        {
            OutputWaterCelsiusDegree = 11F,
            InputWaterCelsiusDegree = 13F,
            HeaterOutputWaterCelsiusDegree = 17F,
            EnvironmentCelsiusDegree = 19F,
            OutputWaterPressureMeter = 23F,
            InputWaterPressureMeter = 29F,
            HeaterPowerKilowatt = 31F,
            WaterPumpFlowRateCubicMeterPerHour = 37F,
        };

        public Switch Switch { get; } = new Switch
        {
            DevicePowerOn = true,
            ExhausterPowerOn = true,
            HeaterAutoOn = true,
            HeaterPowerOn = true,
            HeaterFanOn = true,
            HeaterCompressorOn = true,
            HeaterFourWayReversingOn = true,
        };

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
                PlcFrame responseFrame;
                switch (messageType)
                {
                    case PlcMessageType.ConnectRequest:
                        responseFrame = PlcFrame.Create(
                            PlcMessageType.ConnectResponse,
                            ByteString.CopyFrom(PhysicalAddress.Parse("10BF4879B2A4").GetAddressBytes()));
                        break;
                    case PlcMessageType.GetMetricRequest:
                        responseFrame = this.CreateGetMetricResponse();
                        break;
                    case PlcMessageType.GetSwitchRequest:
                        responseFrame = this.CreateGetSwitchResponseFrame();
                        break;
                    case PlcMessageType.UpdateSwitchRequest:
                        this.UpdateSwitch(content);
                        responseFrame = this.CreateGetSwitchResponseFrame();
                        break;
                    default:
                        responseFrame = null;
                        break;
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

        private PlcFrame CreateGetMetricResponse()
        {
            byte[] responseContent = new byte[0x20];
            using (var writer = new BinaryWriter(new MemoryStream(responseContent)))
            {
                writer.Write(this.Metric.OutputWaterCelsiusDegree);
                writer.Write(this.Metric.InputWaterCelsiusDegree);
                writer.Write(this.Metric.HeaterOutputWaterCelsiusDegree);
                writer.Write(this.Metric.EnvironmentCelsiusDegree);
                writer.Write(this.Metric.OutputWaterPressureMeter);
                writer.Write(this.Metric.InputWaterPressureMeter);
                writer.Write(this.Metric.HeaterPowerKilowatt);
                writer.Write(this.Metric.WaterPumpFlowRateCubicMeterPerHour);
            }

            return PlcFrame.Create(
                PlcMessageType.GetMetricResponse,
                ByteString.CopyFrom(responseContent));
        }

        private void UpdateSwitch(ReadOnlySpan<byte> content)
        {
            if ((content[0] & 0x10) != 0)
            {
                this.Switch.DevicePowerOn = (content[0] & 0x01) == 1;
            }

            if ((content[1] & 0x10) != 0)
            {
                this.Switch.ExhausterPowerOn = (content[1] & 0x01) == 1;
            }

            if ((content[2] & 0x10) != 0)
            {
                this.Switch.HeaterAutoOn = (content[2] & 0x01) == 1;
            }

            if ((content[3] & 0x10) != 0)
            {
                this.Switch.HeaterPowerOn = (content[3] & 0x01) == 1;
            }

            if ((content[4] & 0x10) != 0)
            {
                this.Switch.HeaterFanOn = (content[4] & 0x01) == 1;
            }

            if ((content[5] & 0x10) != 0)
            {
                this.Switch.HeaterCompressorOn = (content[5] & 0x01) == 1;
            }

            if ((content[6] & 0x10) != 0)
            {
                this.Switch.HeaterFourWayReversingOn = (content[6] & 0x01) == 1;
            }
        }

        private PlcFrame CreateGetSwitchResponseFrame()
        {
            byte[] responseContent = new byte[0x07];
            responseContent[0] = (byte)(this.Switch.DevicePowerOn ? 0x01 : 0x00);
            responseContent[1] = (byte)(this.Switch.ExhausterPowerOn ? 0x01 : 0x00);
            responseContent[2] = (byte)(this.Switch.HeaterAutoOn ? 0x01 : 0x00);
            responseContent[3] = (byte)(this.Switch.HeaterPowerOn ? 0x01 : 0x00);
            responseContent[4] = (byte)(this.Switch.HeaterFanOn ? 0x01 : 0x00);
            responseContent[5] = (byte)(this.Switch.HeaterCompressorOn ? 0x01 : 0x00);
            responseContent[6] = (byte)(this.Switch.HeaterFourWayReversingOn ? 0x01 : 0x00);

            return PlcFrame.Create(
                PlcMessageType.GetSwitchResponse,
                ByteString.CopyFrom(responseContent));
        }
    }
}
