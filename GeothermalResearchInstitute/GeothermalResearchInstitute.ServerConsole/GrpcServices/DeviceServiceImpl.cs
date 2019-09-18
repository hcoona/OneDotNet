// <copyright file="DeviceServiceImpl.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GeothermalResearchInstitute.ServerConsole.Models;
using GeothermalResearchInstitute.ServerConsole.Utils;
using GeothermalResearchInstitute.v1;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static Google.Protobuf.WellKnownTypes.FieldMask;
using GrpcDevice = GeothermalResearchInstitute.v1.Device;
using GrpcDeviceMetrics = GeothermalResearchInstitute.v1.DeviceMetrics;
using ModelDeviceMetrics = GeothermalResearchInstitute.ServerConsole.Models.DeviceMetrics;

namespace GeothermalResearchInstitute.ServerConsole.GrpcServices
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Performance", "CA1812", Justification = "Instantiated with reflection.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design", "CA1062:验证公共方法的参数", Justification = "由Grpc框架保证.")]
    public class DeviceServiceImpl : DeviceService.DeviceServiceBase
    {
        private readonly ILogger<DeviceServiceImpl> logger;
        private readonly BjdireContext bjdireContext;
        private readonly IServiceProvider serviceProvider;
        private readonly ConcurrentDictionary<ByteString, GrpcDeviceMetrics> metricsMap;

        public DeviceServiceImpl(
            ILogger<DeviceServiceImpl> logger,
            BjdireContext bjdireContext,
            IServiceProvider serviceProvider)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.bjdireContext = bjdireContext ?? throw new ArgumentNullException(nameof(bjdireContext));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.metricsMap = new ConcurrentDictionary<ByteString, GrpcDeviceMetrics>();

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
                throw new RpcException(new Status(StatusCode.NotFound, "Device Id is not configured."));
            }

            var deviceAdditionalInformation = this.bjdireContext.DevicesActualStates.Find(new object[]
            {
                request.Id.ToByteArray(),
            });
            if (deviceAdditionalInformation == null)
            {
                deviceAdditionalInformation = new DeviceActualStates();
            }

            var device = new GrpcDevice
            {
                Id = request.Id,
            };
            switch (request.View)
            {
                case DeviceView.NameOnly:
                    device.AssignNameFrom(deviceBasicInformation);
                    break;
                case DeviceView.WorkingModeOnly:
                    device.AssignWorkingModeFrom(deviceAdditionalInformation);
                    break;
                case DeviceView.DeviceOptionOnly:
                    device.AssignOptionFrom(deviceAdditionalInformation);
                    break;
                case DeviceView.MetricsAndControl:
                    if (this.metricsMap.TryGetValue(request.Id, out var metrics))
                    {
                        device.Metrics = new GrpcDeviceMetrics(metrics);
                    }
                    else
                    {
                        device.Metrics = new GrpcDeviceMetrics();
                    }

                    device.AssignControlsFrom(deviceAdditionalInformation);
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
            var deviceOptions = this.serviceProvider.GetRequiredService<IOptionsSnapshot<DeviceOptions>>();
            var deviceBasicInformation = deviceOptions.Value.Devices.SingleOrDefault(d => d.ComputeIdBinary().SequenceEqual(request.Device.Id));
            if (deviceBasicInformation == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Device Id is not configured."));
            }

            var deviceStates = this.bjdireContext.DevicesDesiredStates.Find(new object[]
            {
                request.Device.Id.ToByteArray(),
            });
            if (deviceStates == null)
            {
                deviceStates = new DeviceDesiredStates
                {
                    Id = request.Device.Id.ToByteArray(),
                };
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
                        request.Device.AssignWorkingModeTo(deviceStates);
                        break;
                    case "device_option":
                        request.Device.AssignOptionTo(deviceStates);
                        break;
                    case "controls":
                        request.Device.AssignControlsTo(deviceStates);
                        break;
                    default:
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid update_mask."));
                }
            }

            this.bjdireContext.SaveChanges();

            var device = new GrpcDevice
            {
                Id = request.Device.Id,
            };
            request.UpdateMask.Merge(request.Device, device, new MergeOptions());

            return Task.FromResult(device);
        }

        public override Task<HeartbeatResponse> Heartbeat(HeartbeatRequest request, ServerCallContext context)
        {
            if (request.Device.Ipv4Address.IsEmpty)
            {
                request.Device.Ipv4Address = ByteString.CopyFrom(IPAddress.Parse(context.Peer).MapToIPv4().GetAddressBytes());
            }

            var deviceOptions = this.serviceProvider.GetRequiredService<IOptionsSnapshot<DeviceOptions>>();
            var deviceBasicInformation = deviceOptions.Value.Devices.SingleOrDefault(d => d.ComputeIdBinary().SequenceEqual(request.Device.Id));
            if (deviceBasicInformation == null)
            {
                this.logger.LogWarning(
                    "Received heartbeat from a not configured device: mac={0}, ipv4={1}",
                    string.Join(string.Empty, request.Device.Id.Select(b => b.ToString("X2", CultureInfo.InvariantCulture))),
                    new IPAddress(request.Device.Ipv4Address.ToByteArray()).ToString());
                throw new RpcException(new Status(StatusCode.NotFound, "Device Id is not configured."));
            }

            var actualStates = this.bjdireContext.DevicesActualStates.Find(new object[]
            {
                request.Device.Id.ToByteArray(),
            });
            if (actualStates == null)
            {
                actualStates = new DeviceActualStates
                {
                    Id = request.Device.Id.ToByteArray(),
                };
                this.bjdireContext.DevicesActualStates.Add(actualStates);
            }

            actualStates.IPAddress = request.Device.Ipv4Address.ToByteArray();
            request.Device
                .AssignWorkingModeTo(actualStates)
                .AssignOptionFrom(actualStates)
                .AssignControlsFrom(actualStates);

            this.metricsMap.AddOrUpdate(request.Device.Id, _ => request.Device.Metrics, (_, __) => request.Device.Metrics);
            foreach (var m in request.HistoryMetrics.Concat(new[] { request.Device.Metrics }))
            {
                var currentMetrics = this.bjdireContext.DevicesMetrics.Find(new
                {
                    Id = request.Device.Id.ToByteArray(),
                    Timestamp = request.Device.Metrics.UpdateTimestamp.ToDateTimeOffset(),
                });
                if (currentMetrics == null)
                {
                    currentMetrics = new ModelDeviceMetrics
                    {
                        Id = request.Device.Id.ToByteArray(),
                    };
                    this.bjdireContext.DevicesMetrics.Add(currentMetrics);
                }

                request.Device.Metrics.AssignTo(currentMetrics);
            }

            this.bjdireContext.SaveChanges();

            var desiredStates = this.bjdireContext.DevicesDesiredStates.Find(new object[]
            {
                request.Device.Id.ToByteArray(),
            });
            if (actualStates == null)
            {
                desiredStates = new DeviceDesiredStates();
            }

            var device = new GrpcDevice
            {
                Id = request.Device.Id,
            };

            device
                .AssignWorkingModeFrom(desiredStates)
                .AssignOptionFrom(desiredStates)
                .AssignControlsFrom(desiredStates);

            return Task.FromResult(new HeartbeatResponse
            {
                Device = device,
            });
        }
    }
}
