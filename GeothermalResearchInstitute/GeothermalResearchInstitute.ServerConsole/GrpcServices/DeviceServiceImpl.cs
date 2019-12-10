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
using GeothermalResearchInstitute.v2;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GrpcDevice = GeothermalResearchInstitute.v2.Device;
using GrpcDeviceMetrics = GeothermalResearchInstitute.v2.Metric;

namespace GeothermalResearchInstitute.ServerConsole.GrpcServices
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Performance", "CA1812", Justification = "Instantiated with reflection.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design", "CA1062:验证公共方法的参数", Justification = "由Grpc框架保证.")]
    public class DeviceServiceImpl : DeviceService.DeviceServiceBase
    {
        private readonly ILogger<DeviceServiceImpl> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly BjdireContext bjdireContext;
        private readonly PlcHostedService plcHostedService;
        private readonly ConcurrentDictionary<ByteString, GrpcDeviceMetrics> metricsMap =
            new ConcurrentDictionary<ByteString, GrpcDeviceMetrics>();

        public DeviceServiceImpl(
            ILogger<DeviceServiceImpl> logger,
            IServiceProvider serviceProvider,
            BjdireContext bjdireContext,
            PlcHostedService plcHostedService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.bjdireContext = bjdireContext ?? throw new ArgumentNullException(nameof(bjdireContext));
            this.plcHostedService = plcHostedService ?? throw new ArgumentNullException(nameof(plcHostedService));

            if (this.logger.IsEnabled(LogLevel.Debug))
            {
                IOptionsSnapshot<DeviceOptions> deviceOptions = this.serviceProvider.GetRequiredService<IOptionsSnapshot<DeviceOptions>>();
                foreach (DeviceOptionsEntry d in deviceOptions.Value.Devices)
                {
                    this.logger.LogDebug(
                        "{0}={1}",
                        string.Join(string.Empty, d.ComputeIdBinary().Select(b => b.ToString("X2", CultureInfo.InvariantCulture))),
                        d.Name);
                }

                IOptionsSnapshot<AuthenticationOptions> authenticationOptions = this.serviceProvider.GetRequiredService<IOptionsSnapshot<AuthenticationOptions>>();
                foreach (Credential c in authenticationOptions.Value.Credentials)
                {
                    this.logger.LogDebug(c.ToString());
                }
            }
        }

        public override Task<AuthenticateResponse> Authenticate(
            AuthenticateRequest request, ServerCallContext context)
        {
            IOptionsSnapshot<AuthenticationOptions> authenticationOptions = this.serviceProvider.GetRequiredService<IOptionsSnapshot<AuthenticationOptions>>();
            Credential credential = authenticationOptions.Value.Credentials.SingleOrDefault(
                c => string.Equals(c.Username, request.Username, StringComparison.Ordinal) &&
                     string.Equals(c.Password, request.Password, StringComparison.Ordinal));
            if (credential == null)
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid username or password."));
            }
            else
            {
                return Task.FromResult(new AuthenticateResponse()
                {
                    Nickname = credential.Nickname,
                    Role = credential.Role,
                });
            }
        }

        public override Task<ListDevicesResponse> ListDevices(ListDevicesRequest request, ServerCallContext context)
        {
            IOptionsSnapshot<DeviceOptions> deviceOptions =
                this.serviceProvider.GetRequiredService<IOptionsSnapshot<DeviceOptions>>();

            var response = new ListDevicesResponse();
            response.Devices.Add(
                from d in deviceOptions.Value.Devices
                let id = ByteString.CopyFrom(d.ComputeIdBinary())
                join e in this.plcHostedService.PlcDictionary.AsEnumerable()
                on id equals e.Key into g
                from e in g.DefaultIfEmpty()
                select new GrpcDevice
                {
                    Id = id,
                    Name = d.Name,
                    Ipv4Address = e.Value == null
                        ? ByteString.Empty
                        : ByteString.CopyFrom(((IPEndPoint)e.Value.RemoteEndPoint).Address.GetAddressBytes()),
                    Status = e.Value == null
                        ? DeviceStatus.Disconnected
                        : DeviceStatus.Healthy,
                });

            return Task.FromResult(response);
        }
    }
}
