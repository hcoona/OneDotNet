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
using GrpcDevice = GeothermalResearchInstitute.v1.Device;

namespace GeothermalResearchInstitute.ServerConsole.GrpcService
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Performance", "CA1812", Justification = "Instantiated with reflection.")]
    internal class DeviceServiceImpl : DeviceService.DeviceServiceBase
    {
        private readonly ILogger<DeviceServiceImpl> logger;
        private readonly IServiceProvider serviceProvider;

        public DeviceServiceImpl(
            ILogger<DeviceServiceImpl> logger,
            IServiceProvider serviceProvider)
        {
            this.logger = logger;
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
            response.Devices.Add(deviceOptions.Value.Devices.Select(d => new GrpcDevice
            {
                Id = ByteString.CopyFrom(d.ComputeIdBinary()),
                Name = d.Name,
            }));
            return Task.FromResult(response);
        }

        public override Task<GrpcDevice> GetDevice(GetDeviceRequest request, ServerCallContext context)
        {
            var deviceOptions = this.serviceProvider.GetRequiredService<IOptionsSnapshot<DeviceOptions>>();
            var deviceBasicInformation = deviceOptions.Value.Devices.SingleOrDefault(d => d.ComputeIdBinary().SequenceEqual(request.Id));
            if (deviceBasicInformation == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Id"));
            }

            var device = new GrpcDevice();
            switch (request.View)
            {
                case DeviceView.NameOnly:
                    device.Name = deviceBasicInformation.Name;
                    break;
                case DeviceView.WorkingModeOnly:
                    break;
                case DeviceView.DeviceOptionOnly:
                    break;
                case DeviceView.MetricsAndControl:
                    break;
                default:
                    throw new RpcException(new Status(
                        StatusCode.InvalidArgument,
                        "Invalid DeviceView: " + (int)request.View));
            }

            return Task.FromResult(device);
        }

        public override Task<GrpcDevice> UpdateDevice(UpdateDeviceRequest request, ServerCallContext context)
        {
            return base.UpdateDevice(request, context);
        }
    }
}
