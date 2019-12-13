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
using Google.Protobuf.WellKnownTypes;
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
            if (response.FrameHeader.MessageType != PlcMessageType.ConnectResponse)
            {
                throw new InvalidDataException(
                    "Response message type mismatch: " + response.FrameHeader.MessageType);
            }

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
                PlcFrame.Create(PlcMessageType.GetMetricRequest, ByteString.Empty),
                deadline)
                .ConfigureAwait(false);
            if (response.FrameHeader.MessageType != PlcMessageType.GetMetricResponse)
            {
                throw new InvalidDataException(
                    "Response message type mismatch: " + response.FrameHeader.MessageType);
            }

            using var reader = new BinaryReader(new MemoryStream(response.FrameBody.ToByteArray()));
            return new Metric
            {
                CreateTime = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow),
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

        public async Task<Switch> GetSwitchAsync(GetSwitchRequest request, DateTime? deadline)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            PlcFrame response = await this.InvokeAsync(
                PlcFrame.Create(PlcMessageType.GetSwitchRequest, ByteString.Empty),
                deadline)
                .ConfigureAwait(false);
            if (response.FrameHeader.MessageType != PlcMessageType.GetSwitchResponse)
            {
                throw new InvalidDataException(
                    "Response message type mismatch: " + response.FrameHeader.MessageType);
            }

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
            switch (request.UpdateMask.Paths.Single())
            {
                case "device_power_on":
                    bytes[0] = (byte)(0x10 | (request.Switch.DevicePowerOn ? 1 : 0));
                    break;
                case "exhauster_power_on":
                    bytes[1] = (byte)(0x10 | (request.Switch.ExhausterPowerOn ? 1 : 0));
                    break;
                case "heater_auto_on":
                    bytes[2] = (byte)(0x10 | (request.Switch.HeaterAutoOn ? 1 : 0));
                    break;
                case "heater_power_on":
                    bytes[3] = (byte)(0x10 | (request.Switch.HeaterPowerOn ? 1 : 0));
                    break;
                case "heater_fan_on":
                    bytes[4] = (byte)(0x10 | (request.Switch.HeaterFanOn ? 1 : 0));
                    break;
                case "heater_compressor_on":
                    bytes[5] = (byte)(0x10 | (request.Switch.HeaterCompressorOn ? 1 : 0));
                    break;
                case "heater_four_way_reversing_on":
                    bytes[6] = (byte)(0x10 | (request.Switch.HeaterFourWayReversingOn ? 1 : 0));
                    break;
                default:
                    throw new InvalidDataException("Unrecognized update mask " + request.UpdateMask.Paths.Single());
            }

            PlcFrame response = await this.InvokeAsync(
                PlcFrame.Create(PlcMessageType.UpdateSwitchRequest, ByteString.CopyFrom(bytes)),
                deadline)
                .ConfigureAwait(false);
            if (response.FrameHeader.MessageType != PlcMessageType.GetSwitchResponse)
            {
                throw new InvalidDataException(
                    "Response message type mismatch: " + response.FrameHeader.MessageType);
            }

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

        public async Task<WorkingMode> GetWorkingModeAsync(GetWorkingModeRequest request, DateTime? deadline)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            PlcFrame response = await this.InvokeAsync(
                PlcFrame.Create(PlcMessageType.GetWorkingModeRequest, ByteString.Empty),
                deadline)
                .ConfigureAwait(false);
            if (response.FrameHeader.MessageType != PlcMessageType.GetWorkingModeResponse)
            {
                throw new InvalidDataException(
                    "Response message type mismatch: " + response.FrameHeader.MessageType);
            }

            using var reader = new BinaryReader(new MemoryStream(response.FrameBody.ToByteArray()));
            return new WorkingMode
            {
                DeviceWorkingMode = (DeviceWorkingMode)reader.ReadByte(),
                DeviceFlowRateControlMode = (DeviceFlowRateControlMode)reader.ReadByte(),
                WaterPumpWorkingMode = (WaterPumpWorkingMode)reader.ReadByte(),
            };
        }

        public async Task<WorkingMode> UpdateWorkingModeAsync(UpdateWorkingModeRequest request, DateTime? deadline)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            byte[] bytes = new byte[0x03];
            foreach (string path in request.UpdateMask.Paths)
            {
                switch (path)
                {
                    case "device_working_mode":
                        bytes[0] = (byte)request.WorkingMode.DeviceWorkingMode;
                        break;
                    case "device_flow_rate_control_mode":
                        bytes[1] = (byte)request.WorkingMode.DeviceFlowRateControlMode;
                        break;
                    case "water_pump_working_mode":
                        bytes[2] = (byte)request.WorkingMode.WaterPumpWorkingMode;
                        break;
                    default:
                        throw new InvalidDataException("Unrecognized update mask " + path);
                }
            }

            PlcFrame response = await this.InvokeAsync(
                PlcFrame.Create(PlcMessageType.UpdateWorkingModeRequest, ByteString.CopyFrom(bytes)),
                deadline)
                .ConfigureAwait(false);
            if (response.FrameHeader.MessageType != PlcMessageType.GetWorkingModeResponse)
            {
                throw new InvalidDataException(
                    "Response message type mismatch: " + response.FrameHeader.MessageType);
            }

            using var reader = new BinaryReader(new MemoryStream(response.FrameBody.ToByteArray()));
            return new WorkingMode
            {
                DeviceWorkingMode = (DeviceWorkingMode)reader.ReadByte(),
                DeviceFlowRateControlMode = (DeviceFlowRateControlMode)reader.ReadByte(),
                WaterPumpWorkingMode = (WaterPumpWorkingMode)reader.ReadByte(),
            };
        }

        private Task<PlcFrame> InvokeAsync(PlcFrame request, DateTime? deadline)
        {
            if (this.closingCancellationTokenSource.IsCancellationRequested)
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
