// <copyright file="DeviceServiceImpl.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GeothermalResearchInstitute.ServerConsole.Model;
using GeothermalResearchInstitute.v1;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GeothermalResearchInstitute.ServerConsole.GrpcService
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Performance", "CA1812", Justification = "Instantiated with reflection.")]
    internal class DeviceServiceImpl : DeviceService.DeviceServiceBase
    {
        private readonly ILogger<DeviceServiceImpl> logger;
        private readonly BjdireContext bjdireContext;
        private readonly IServiceProvider serviceProvider;

        public DeviceServiceImpl(
            ILogger<DeviceServiceImpl> logger,
            BjdireContext bjdireContext,
            IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.bjdireContext = bjdireContext;
            this.serviceProvider = serviceProvider;

            if (this.logger.IsEnabled(LogLevel.Debug))
            {
                var deviceOptions = this.serviceProvider.GetRequiredService<IOptionsSnapshot<DeviceOptions>>();
                foreach (var d in deviceOptions.Value.Devices)
                {
                    this.logger.LogDebug(
                        "{0}={1}",
                        string.Join(string.Empty, d.ComputeIdBinary().Select(b => b.ToString("X2", CultureInfo.InvariantCulture))),
                        d.Name);
                }
            }
        }

        public override Task<ListDevicesResponse> ListDevices(ListDevicesRequest request, ServerCallContext context)
        {
            var deviceOptions = this.serviceProvider.GetRequiredService<IOptionsSnapshot<DeviceOptions>>();
            var response = new ListDevicesResponse();
            response.Devices.Add(deviceOptions.Value.Devices.Select(d => new Device
            {
                Id = ByteString.CopyFrom(d.ComputeIdBinary()),
                Name = d.Name,
            }));
            return Task.FromResult(response);
        }

        public override Task<Device> GetDevice(GetDeviceRequest request, ServerCallContext context)
        {
            var deviceOptions = this.serviceProvider.GetRequiredService<IOptionsSnapshot<DeviceOptions>>();
            var deviceBasicInformation = deviceOptions.Value.Devices.SingleOrDefault(d => d.ComputeIdBinary().SequenceEqual(request.Id));
            if (deviceBasicInformation == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Id"));
            }

            var deviceAdditionalInformation = this.bjdireContext.DevicesActualStates.SingleOrDefault(d => d.Id.SequenceEqual(request.Id));
            if (deviceAdditionalInformation == null)
            {
                deviceAdditionalInformation = new DeviceActualStates();
            }

            var device = new Device();
            switch (request.View)
            {
                case DeviceView.NameOnly:
                    device.Name = deviceBasicInformation.Name;
                    break;
                case DeviceView.WorkingModeOnly:
                    device.WorkingMode = deviceAdditionalInformation.WorkingMode;
                    break;
                case DeviceView.DeviceOptionOnly:
                    device.DeviceOption = new DeviceOption
                    {
                        SummerTemperature = deviceAdditionalInformation.SummerTemperature,
                        WinterTemperature = deviceAdditionalInformation.WinterTemperature,
                        WarmCapacity = deviceAdditionalInformation.WarmCapacity,
                        ColdCapacity = deviceAdditionalInformation.ColdCapacity,
                        FlowCapacity = deviceAdditionalInformation.FlowCapacity,
                        RateCapacity = deviceAdditionalInformation.RateCapacity,
                        MotorMode = deviceAdditionalInformation.MotorMode,
                        WaterPumpMode = deviceAdditionalInformation.WaterPumpMode,
                    };
                    break;
                case DeviceView.MetricsAndControl:
                    // TODO(zhangshuai.ustc): Loading it.
                    device.Metrics = new DeviceMetrics
                    {
                    };
                    device.Controls = new DeviceControls
                    {
                        DevicePower = deviceAdditionalInformation.DevicePower,
                        ExhaustPower = deviceAdditionalInformation.ExhaustPower,
                        HeatPumpAuto = deviceAdditionalInformation.HeatPumpAuto,
                        HeatPumpPower = deviceAdditionalInformation.HeatPumpPower,
                        HeatPumpFanOn = deviceAdditionalInformation.HeatPumpFanOn,
                        HeatPumpCompressorOn = deviceAdditionalInformation.HeatPumpCompressorOn,
                        HeatPumpFourWayReversingValue = deviceAdditionalInformation.HeatPumpFourWayReversingValue,
                    };
                    break;
                default:
                    throw new RpcException(new Status(
                        StatusCode.InvalidArgument,
                        "Invalid DeviceView: " + (int)request.View));
            }

            return Task.FromResult(device);
        }

        public override Task<Device> UpdateDevice(UpdateDeviceRequest request, ServerCallContext context)
        {
            var deviceOptions = this.serviceProvider.GetRequiredService<IOptionsSnapshot<DeviceOptions>>();
            var deviceBasicInformation = deviceOptions.Value.Devices.SingleOrDefault(d => d.ComputeIdBinary().SequenceEqual(request.Device.Id));
            if (deviceBasicInformation == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Id"));
            }

            var deviceStates = this.bjdireContext.DevicesDesiredStates.SingleOrDefault(d => d.Id.SequenceEqual(request.Device.Id));
            if (deviceStates == null)
            {
                deviceStates = new DeviceDesiredStates();
                this.bjdireContext.DevicesDesiredStates.Add(deviceStates);
            }

            if (request.UpdateMask.Paths.Count == 0)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid update_mask."));
            }

            foreach (var path in request.UpdateMask.Paths)
            {
                switch (path)
                {
                    case "working_mode":
                        deviceStates.WorkingMode = request.Device.WorkingMode;
                        break;
                    case "device_option":
                        deviceStates.SummerTemperature = request.Device.DeviceOption.SummerTemperature;
                        deviceStates.WinterTemperature = request.Device.DeviceOption.WinterTemperature;
                        deviceStates.WarmCapacity = request.Device.DeviceOption.WarmCapacity;
                        deviceStates.ColdCapacity = request.Device.DeviceOption.ColdCapacity;
                        deviceStates.FlowCapacity = request.Device.DeviceOption.FlowCapacity;
                        deviceStates.RateCapacity = request.Device.DeviceOption.RateCapacity;
                        deviceStates.MotorMode = request.Device.DeviceOption.MotorMode;
                        deviceStates.WaterPumpMode = request.Device.DeviceOption.WaterPumpMode;
                        break;
                    case "controls":
                        deviceStates.DevicePower = request.Device.Controls.DevicePower;
                        deviceStates.ExhaustPower = request.Device.Controls.ExhaustPower;
                        deviceStates.HeatPumpAuto = request.Device.Controls.HeatPumpAuto;
                        deviceStates.HeatPumpPower = request.Device.Controls.HeatPumpPower;
                        deviceStates.HeatPumpFanOn = request.Device.Controls.HeatPumpFanOn;
                        deviceStates.HeatPumpCompressorOn = request.Device.Controls.HeatPumpCompressorOn;
                        deviceStates.HeatPumpFourWayReversingValue = request.Device.Controls.HeatPumpFourWayReversingValue;
                        break;
                    default:
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid update_mask."));
                }
            }

            this.bjdireContext.SaveChanges();

            return base.UpdateDevice(request, context);
        }
    }
}
