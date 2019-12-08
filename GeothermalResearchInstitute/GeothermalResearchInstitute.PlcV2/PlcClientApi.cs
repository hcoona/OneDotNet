// <copyright file="PlcClientApi.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using GeothermalResearchInstitute.v2;
using Google.Protobuf;
using Grpc.Core;

namespace GeothermalResearchInstitute.PlcV2
{
    public partial class PlcClient
    {
        public async Task<ConnectResponse> ConnectAsync(ConnectRequest request, DateTime? deadline)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            PlcFrame response = await this.InvokeAsync(
                PlcFrame.Create(PlcMessageType.ConnectRequest, ByteString.Empty),
                deadline)
                .ConfigureAwait(false);
            return new ConnectResponse
            {
                Id = response.FrameBody,
            };
        }

        public async Task<Metric> GetMetricAsync(GetMetricRequest request, DateTime? deadline)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            PlcFrame response = await this.InvokeAsync(
                PlcFrame.Create(PlcMessageType.ConnectRequest, ByteString.Empty),
                deadline)
                .ConfigureAwait(false);
            using var reader = new BinaryReader(new MemoryStream(response.FrameBody.ToByteArray()));
            return new Metric
            {
                OutputWaterCelsiusDegree = reader.ReadSingle(),
                InputWaterCelsiusDegree = reader.ReadSingle(),
                HeaterOutputWaterCelsiusDegree = reader.ReadSingle(),
                EnvironmentCelsiusDegree = reader.ReadSingle(),
                OutputWaterPressureMeter = reader.ReadSingle(),
                InputWaterPressureMeter = reader.ReadSingle(),
                HeaterPowerKilowatt = reader.ReadSingle(),
                WaterPumpFlowRateCubicMeterPerHour = reader.ReadSingle(),
            };
        }

        public async Task<Switch> UpdateSwitchAsync(UpdateSwitchRequest request, DateTime? deadline)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.UpdateMask.Paths.Count != 1)
            {
                throw new ArgumentException("Update not exact 1 field", nameof(request));
            }

            byte[] bytes = new byte[0x07];
            bytes[0] = request.UpdateMask.Paths.Single() switch
            {
                "device_power_on" => (byte)(0x10 | (request.Switch.DevicePowerOn ? 1 : 0)),
                "exhauster_power_on" => (byte)(0x10 | (request.Switch.ExhausterPowerOn ? 1 : 0)),
                "heater_auto_on" => (byte)(0x10 | (request.Switch.HeaterAutoOn ? 1 : 0)),
                "heater_power_on" => (byte)(0x10 | (request.Switch.HeaterPowerOn ? 1 : 0)),
                "heater_fan_on" => (byte)(0x10 | (request.Switch.HeaterFanOn ? 1 : 0)),
                "heater_compressor_on" => (byte)(0x10 | (request.Switch.HeaterCompressorOn ? 1 : 0)),
                "heater_four_way_reversing_on" => (byte)(0x10 | (request.Switch.HeaterFourWayReversingOn ? 1 : 0)),
                _ => throw new InvalidDataException("Unrecognized update mask " + request.UpdateMask.Paths.Single()),
            };

            PlcFrame response = await this.InvokeAsync(
                PlcFrame.Create(PlcMessageType.ConnectRequest, ByteString.CopyFrom(bytes)),
                deadline)
                .ConfigureAwait(false);
            using var reader = new BinaryReader(new MemoryStream(response.FrameBody.ToByteArray()));
            return new Switch
            {
                DevicePowerOn = reader.ReadByte() != 0,
                ExhausterPowerOn = reader.ReadByte() != 0,
                HeaterAutoOn = reader.ReadByte() != 0,
                HeaterPowerOn = reader.ReadByte() != 0,
                HeaterFanOn = reader.ReadByte() != 0,
                HeaterCompressorOn = reader.ReadByte() != 0,
                HeaterFourWayReversingOn = reader.ReadByte() != 0,
            };
        }

        private Task<PlcFrame> InvokeAsync(PlcFrame request, DateTime? deadline)
        {
            if (this.closed)
            {
                return Task.FromException<PlcFrame>(new RpcException(Status.DefaultCancelled));
            }

            var promise = new TaskCompletionSource<PlcFrame>();
            bool accepted = this.requestContextSendingBufferBlock.Post(new PlcRequestContext
            {
                TaskCompletionSource = promise,
                RequestFrame = request,
                Deadline = deadline,
            });

            if (!accepted)
            {
                promise.SetException(new RpcException(Status.DefaultCancelled));
            }

            return promise.Task;
        }
    }
}
