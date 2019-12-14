// <copyright file="DeviceServiceImpl.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using GeothermalResearchInstitute.PlcV2;
using GeothermalResearchInstitute.ServerConsole.Models;
using GeothermalResearchInstitute.ServerConsole.Options;
using GeothermalResearchInstitute.ServerConsole.Utils;
using GeothermalResearchInstitute.v2;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GrpcAlarm = GeothermalResearchInstitute.v2.Alarm;
using GrpcAlarmChange = GeothermalResearchInstitute.v2.AlarmChange;
using GrpcMetric = GeothermalResearchInstitute.v2.Metric;
using ModelAlarm = GeothermalResearchInstitute.ServerConsole.Models.Alarm;
using ModelAlarmChange = GeothermalResearchInstitute.ServerConsole.Models.AlarmChange;
using ModelMetric = GeothermalResearchInstitute.ServerConsole.Models.Metric;

namespace GeothermalResearchInstitute.ServerConsole.GrpcServices
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design", "CA1062:验证公共方法的参数", Justification = "由Grpc框架保证.")]
    public class DeviceServiceImpl : DeviceService.DeviceServiceBase, IDisposable
    {
        private readonly ILogger<DeviceServiceImpl> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly PlcManager plcManager;
        private readonly Timer askAlarmTimer;
        private readonly Timer askMetricTimer;
        private bool disposedValue = false;

        public DeviceServiceImpl(
            ILogger<DeviceServiceImpl> logger,
            IOptions<TasksOptions> tasksOptions,
            IServiceProvider serviceProvider,
            PlcManager plcManager)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.plcManager = plcManager ?? throw new ArgumentNullException(nameof(plcManager));
            this.askAlarmTimer = new Timer(
                this.AskPersistDeviceAlarm,
                null,
                TimeSpan.FromMilliseconds(tasksOptions.Value.CollectAlarmIntervalMillis),
                TimeSpan.FromMilliseconds(tasksOptions.Value.CollectAlarmIntervalMillis));
            this.askMetricTimer = new Timer(
                this.AskPersistDeviceMetric,
                null,
                TimeSpan.FromMilliseconds(tasksOptions.Value.CollectMetricIntervalMillis),
                TimeSpan.FromMilliseconds(tasksOptions.Value.CollectMetricIntervalMillis));

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

        ~DeviceServiceImpl()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
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

        public override Task<TestResponse> Test(TestRequest request, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, "Not supported."));
        }

        public override Task<ConnectResponse> Connect(ConnectRequest request, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, "Not supported."));
        }

        public override Task<ListDevicesResponse> ListDevices(ListDevicesRequest request, ServerCallContext context)
        {
            IOptionsSnapshot<DeviceOptions> deviceOptions =
                this.serviceProvider.GetRequiredService<IOptionsSnapshot<DeviceOptions>>();

            var response = new ListDevicesResponse();
            response.Devices.Add(
                from d in deviceOptions.Value.Devices
                let id = ByteString.CopyFrom(d.ComputeIdBinary())
                join e in this.plcManager.PlcDictionary.AsEnumerable()
                on id equals e.Key into g
                from e in g.DefaultIfEmpty()
                select new Device
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

        public override Task<GrpcMetric> GetMetric(GetMetricRequest request, ServerCallContext context)
        {
            return this.Invoke(
                (client, request, deadline) => client.GetMetricAsync(request, deadline),
                request.DeviceId,
                request,
                context);
        }

        public override Task<Switch> GetSwitch(GetSwitchRequest request, ServerCallContext context)
        {
            return this.Invoke(
                (client, request, deadline) => client.GetSwitchAsync(request, deadline),
                request.DeviceId,
                request,
                context);
        }

        public override Task<Switch> UpdateSwitch(UpdateSwitchRequest request, ServerCallContext context)
        {
            return this.Invoke(
                (client, request, deadline) => client.UpdateSwitchAsync(request, deadline),
                request.DeviceId,
                request,
                context);
        }

        public override Task<ListMetricsResponse> ListMetrics(ListMetricsRequest request, ServerCallContext context)
        {
            string id = BitConverter.ToString(request.DeviceId.ToByteArray());

            DateTimeOffset endDateTime;
            if (string.IsNullOrEmpty(request.PageToken))
            {
                endDateTime = request.EndTime?.ToDateTimeOffset() ?? DateTimeOffset.UtcNow;
            }
            else
            {
                endDateTime = DateTime.Parse(request.PageToken, CultureInfo.InvariantCulture);
            }

            endDateTime = endDateTime.ToUniversalTime();
            DateTimeOffset? startDateTime = request.StartTime?.ToDateTimeOffset().ToUniversalTime();

            var response = new ListMetricsResponse();
            using (BjdireContext db = this.serviceProvider.GetRequiredService<BjdireContext>())
            {
                var metrics = (from m in db.Metrics
                               where m.DeviceId == id
                                   && (startDateTime == null || startDateTime <= m.Timestamp)
                                   && m.Timestamp <= endDateTime
                               orderby m.Timestamp descending
                               select m)
                    .Take(request.PageSize)
                    .ToList();
                response.Metrics.AddRange(metrics.Select(metric =>
                {
                    var m = new GrpcMetric();
                    m.AssignFrom(metric);
                    return m;
                }));

                if (metrics.Count == request.PageSize && metrics.Last().Timestamp > startDateTime)
                {
                    response.NextPageToken = metrics.Last().Timestamp
                            .ToUniversalTime()
                            .ToString(CultureInfo.InvariantCulture);
                }
            }

            return Task.FromResult(response);
        }

        public override Task<WorkingMode> GetWorkingMode(GetWorkingModeRequest request, ServerCallContext context)
        {
            return this.Invoke(
                (client, request, deadline) => client.GetWorkingModeAsync(request, deadline),
                request.DeviceId,
                request,
                context);
        }

        public override Task<WorkingMode> UpdateWorkingMode(
            UpdateWorkingModeRequest request,
            ServerCallContext context)
        {
            return this.Invoke(
                (client, request, deadline) => client.UpdateWorkingModeAsync(request, deadline),
                request.DeviceId,
                request,
                context);
        }

        public override Task<RunningParameter> GetRunningParameter(
            GetRunningParameterRequest request,
            ServerCallContext context)
        {
            return this.Invoke(
                (client, request, deadline) => client.GetRunningParameterAsync(request, deadline),
                request.DeviceId,
                request,
                context);
        }

        public override Task<RunningParameter> UpdateRunningParameter(
            UpdateRunningParameterRequest request,
            ServerCallContext context)
        {
            return this.Invoke(
                (client, request, deadline) => client.UpdateRunningParameterAsync(request, deadline),
                request.DeviceId,
                request,
                context);
        }

        public override Task<GrpcAlarm> GetAlarm(GetAlarmRequest request, ServerCallContext context)
        {
            return this.Invoke(
                (client, request, deadline) => client.GetAlarmAsync(request, deadline),
                request.DeviceId,
                request,
                context);
        }

        public override Task<ListAlarmChangesResponse> ListAlarmChanges(
            ListAlarmChangesRequest request,
            ServerCallContext context)
        {
            string id = BitConverter.ToString(request.DeviceId.ToByteArray());

            DateTimeOffset endDateTime;
            if (string.IsNullOrEmpty(request.PageToken))
            {
                endDateTime = request.EndTime?.ToDateTimeOffset() ?? DateTimeOffset.UtcNow;
            }
            else
            {
                endDateTime = DateTime.Parse(request.PageToken, CultureInfo.InvariantCulture);
            }

            endDateTime = endDateTime.ToUniversalTime();
            DateTimeOffset? startDateTime = request.StartTime?.ToDateTimeOffset().ToUniversalTime();

            var response = new ListAlarmChangesResponse();
            using (BjdireContext db = this.serviceProvider.GetRequiredService<BjdireContext>())
            {
                var alarmChanges = (from m in db.AlarmChanges
                                    where m.DeviceId == id
                                        && (startDateTime == null || startDateTime <= m.Timestamp)
                                        && m.Timestamp <= endDateTime
                                    orderby m.Timestamp descending
                                    select m)
                    .Take(request.PageSize)
                    .ToList();
                response.AlarmChanges.AddRange(alarmChanges.Select(alarmChange =>
                {
                    var m = new GrpcAlarmChange();
                    m.AssignFrom(alarmChange);
                    return m;
                }));

                if (alarmChanges.Count == request.PageSize && alarmChanges.Last().Timestamp > startDateTime)
                {
                    response.NextPageToken = alarmChanges.Last().Timestamp
                            .ToUniversalTime()
                            .ToString(CultureInfo.InvariantCulture);
                }
            }

            return Task.FromResult(response);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.askAlarmTimer.Dispose();
                    this.askMetricTimer.Dispose();
                }

                this.disposedValue = true;
            }
        }

        private static IEnumerable<ModelAlarmChange> ComputeAlarmChanges(
            string deviceId,
            ModelAlarm lastKnownAlarm,
            ModelAlarm currentAlarm)
        {
            if (lastKnownAlarm.LowFlowRate != currentAlarm.LowFlowRate)
            {
                yield return new ModelAlarmChange
                {
                    DeviceId = deviceId,
                    Timestamp = currentAlarm.Timestamp,
                    Type = AlarmType.LowFlowRate,
                    Direction = currentAlarm.LowFlowRate
                        ? AlarmChangeDirection.Appearance
                        : AlarmChangeDirection.Disappearance,
                };
            }

            if (lastKnownAlarm.HighHeaterPressure != currentAlarm.HighHeaterPressure)
            {
                yield return new ModelAlarmChange
                {
                    DeviceId = deviceId,
                    Timestamp = currentAlarm.Timestamp,
                    Type = AlarmType.HighHeaterPressure,
                    Direction = currentAlarm.HighHeaterPressure
                        ? AlarmChangeDirection.Appearance
                        : AlarmChangeDirection.Disappearance,
                };
            }

            if (lastKnownAlarm.LowHeaterPressure != currentAlarm.LowHeaterPressure)
            {
                yield return new ModelAlarmChange
                {
                    DeviceId = deviceId,
                    Timestamp = currentAlarm.Timestamp,
                    Type = AlarmType.LowHeaterPressure,
                    Direction = currentAlarm.LowHeaterPressure
                        ? AlarmChangeDirection.Appearance
                        : AlarmChangeDirection.Disappearance,
                };
            }

            if (lastKnownAlarm.NoPower != currentAlarm.NoPower)
            {
                yield return new ModelAlarmChange
                {
                    DeviceId = deviceId,
                    Timestamp = currentAlarm.Timestamp,
                    Type = AlarmType.NoPower,
                    Direction = currentAlarm.NoPower
                        ? AlarmChangeDirection.Appearance
                        : AlarmChangeDirection.Disappearance,
                };
            }

            if (lastKnownAlarm.HeaterOverloadedBroken != currentAlarm.HeaterOverloadedBroken)
            {
                yield return new ModelAlarmChange
                {
                    DeviceId = deviceId,
                    Timestamp = currentAlarm.Timestamp,
                    Type = AlarmType.HeaterOverloadedBroken,
                    Direction = currentAlarm.HeaterOverloadedBroken
                        ? AlarmChangeDirection.Appearance
                        : AlarmChangeDirection.Disappearance,
                };
            }

            if (lastKnownAlarm.ElectricalHeaterBroken != currentAlarm.ElectricalHeaterBroken)
            {
                yield return new ModelAlarmChange
                {
                    DeviceId = deviceId,
                    Timestamp = currentAlarm.Timestamp,
                    Type = AlarmType.ElectricalHeaterBroken,
                    Direction = currentAlarm.ElectricalHeaterBroken
                        ? AlarmChangeDirection.Appearance
                        : AlarmChangeDirection.Disappearance,
                };
            }
        }

        private Task<TResponse> Invoke<TRequest, TResponse>(
            Func<PlcClient, TRequest, DateTime, Task<TResponse>> stub,
            ByteString deviceId,
            TRequest request,
            ServerCallContext context)
        {
            if (deviceId.IsEmpty)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Must specify device_id."));
            }

            if (this.plcManager.PlcDictionary.TryGetValue(deviceId, out PlcClient client))
            {
                return stub.Invoke(client, request, context.Deadline);
            }
            else
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Device is currently offline."));
            }
        }

        private async void AskPersistDeviceAlarm(object state)
        {
            IOptionsSnapshot<CoreOptions> coreOptions =
                this.serviceProvider.GetRequiredService<IOptionsSnapshot<CoreOptions>>();
            IOptionsSnapshot<DeviceOptions> deviceOptions =
                this.serviceProvider.GetRequiredService<IOptionsSnapshot<DeviceOptions>>();

            using BjdireContext db = this.serviceProvider.GetRequiredService<BjdireContext>();

            foreach (DeviceOptionsEntry d in deviceOptions.Value.Devices)
            {
                try
                {
                    byte[] id = d.ComputeIdBinary();
                    string deviceId = BitConverter.ToString(id);
                    if (this.plcManager.PlcDictionary.TryGetValue(ByteString.CopyFrom(id), out PlcClient client))
                    {
                        this.logger.LogInformation("Ask alarm for {0}({1})", d.Id, d.Name);

                        GrpcAlarm alarm = await client
                            .GetAlarmAsync(
                                new GetAlarmRequest(),
                                DateTime.UtcNow.AddMilliseconds(
                                    coreOptions.Value.DefaultReadTimeoutMillis))
                            .ConfigureAwait(true);

                        var m = new ModelAlarm
                        {
                            DeviceId = deviceId,
                        };
                        alarm.AssignTo(m);

                        ModelAlarm lastKnownAlarmInfo = (
                            from mm in db.Alarms
                            where mm.DeviceId == deviceId
                            orderby mm.Timestamp descending
                            select mm)
                            .FirstOrDefault();

                        if (lastKnownAlarmInfo == null)
                        {
                            lastKnownAlarmInfo = new ModelAlarm();
                        }

                        db.AlarmChanges.AddRange(ComputeAlarmChanges(deviceId, lastKnownAlarmInfo, m));

                        db.Alarms.Add(m);
                    }
                    else
                    {
                        this.logger.LogWarning(
                            "Failed to ask alarm for {0}({1}), currently offline.",
                            d.Id,
                            d.Name);
                    }
                }
                catch (RpcException e)
                {
                    this.logger.LogWarning(e, "Failed to ask alarm for {0}({1})", d.Id, d.Name);
                }
            }

            db.SaveChanges();
        }

        private async void AskPersistDeviceMetric(object state)
        {
            IOptionsSnapshot<CoreOptions> coreOptions =
                this.serviceProvider.GetRequiredService<IOptionsSnapshot<CoreOptions>>();
            IOptionsSnapshot<DeviceOptions> deviceOptions =
                this.serviceProvider.GetRequiredService<IOptionsSnapshot<DeviceOptions>>();

            using BjdireContext db = this.serviceProvider.GetRequiredService<BjdireContext>();

            foreach (DeviceOptionsEntry d in deviceOptions.Value.Devices)
            {
                try
                {
                    byte[] id = d.ComputeIdBinary();
                    if (this.plcManager.PlcDictionary.TryGetValue(ByteString.CopyFrom(id), out PlcClient client))
                    {
                        this.logger.LogInformation("Ask metric for {0}({1})", d.Id, d.Name);

                        GrpcMetric metric = await client
                            .GetMetricAsync(
                                new GetMetricRequest(),
                                DateTime.UtcNow.AddMilliseconds(
                                    coreOptions.Value.DefaultReadTimeoutMillis))
                            .ConfigureAwait(true);

                        var m = new ModelMetric
                        {
                            DeviceId = BitConverter.ToString(id),
                        };
                        metric.AssignTo(m);

                        db.Metrics.Add(m);
                    }
                    else
                    {
                        this.logger.LogWarning(
                            "Failed to ask metric for {0}({1}), currently offline.",
                            d.Id,
                            d.Name);
                    }
                }
                catch (RpcException e)
                {
                    this.logger.LogWarning(e, "Failed to ask metric for {0}({1})", d.Id, d.Name);
                }
            }

            db.SaveChanges();
        }
    }
}
