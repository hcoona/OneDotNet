// <copyright file="FakeDeviceServiceClient.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using GeothermalResearchInstitute.v2;
using GeothermalResearchInstitute.Wpf.Common;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Testing;

namespace GeothermalResearchInstitute.Wpf.FakeClients
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design", "CA1062:验证公共方法的参数", Justification = "Guaranteed by framework.")]
    public class FakeDeviceServiceClient : DeviceService.DeviceServiceClient
    {
        public static readonly ByteString Id1 = ByteString.CopyFrom(new byte[] { 0x10, 0xBF, 0x48, 0x79, 0xB2, 0xA4 });
        public static readonly ByteString Id2 = ByteString.CopyFrom(new byte[] { 0xBC, 0x96, 0x80, 0xE6, 0x70, 0x16 });
        public static readonly ByteString Id3 = ByteString.CopyFrom(new byte[] { 0xBC, 0x96, 0x80, 0xE6, 0x70, 0x17 });

        public static readonly Dictionary<ByteString, Device> Devices = new Dictionary<ByteString, Device>
        {
            {
                Id1,
                new Device
                {
                    Id = Id1,
                    Ipv4Address = ByteString.CopyFromUtf8("10.0.0.1"),
                    Name = "测试设备1",
                    Status = DeviceStatus.Healthy,
                }
            },
            {
                Id2,
                new Device
                {
                    Id = Id2,
                    Ipv4Address = ByteString.CopyFromUtf8("10.0.0.2"),
                    Name = "测试设备2",
                    Status = DeviceStatus.Unhealthy,
                }
            },
            {
                Id3,
                new Device
                {
                    Id = Id3,
                    Ipv4Address = ByteString.CopyFromUtf8("10.0.0.3"),
                    Name = "测试设备3",
                    Status = DeviceStatus.Disconnected,
                }
            },
        };

        private static readonly Dictionary<ByteString, WorkingMode> WorkingModes = new Dictionary<ByteString, WorkingMode>
        {
            {
                Id1,
                new WorkingMode
                {
                    DeviceWorkingMode = DeviceWorkingMode.SummerSituation,
                    DeviceFlowRateControlMode = DeviceFlowRateControlMode.VariableFrequency,
                    WaterPumpWorkingMode = WaterPumpWorkingMode.FixedFrequency,
                }
            },
            {
                Id2,
                new WorkingMode
                {
                    DeviceWorkingMode = DeviceWorkingMode.WinterSituation,
                    DeviceFlowRateControlMode = DeviceFlowRateControlMode.VariableFrequency,
                    WaterPumpWorkingMode = WaterPumpWorkingMode.FixedFrequency,
                }
            },
            {
                Id3,
                new WorkingMode
                {
                    DeviceWorkingMode = DeviceWorkingMode.TemperatureDetermination,
                    DeviceFlowRateControlMode = DeviceFlowRateControlMode.VariableFrequency,
                    WaterPumpWorkingMode = WaterPumpWorkingMode.FixedFrequency,
                }
            },
        };

        private static readonly Dictionary<ByteString, RunningParameter> RunningParameters = new Dictionary<ByteString, RunningParameter>
        {
            {
                Id1,
                new RunningParameter
                {
                    SummerHeaterCelsiusDegree = 13.7F,
                    WinterHeaterCelsiusDegree = 8.4F,
                    ColdPowerKilowatt = 4F,
                    WarmPowerKilowatt = 8F,
                    WaterPumpFlowRateCubicMeterPerHour = 28.4F,
                    WaterPumpFrequencyHertz = 20.8F,
                }
            },
        };

        private static readonly Dictionary<ByteString, Switch> Switches = new Dictionary<ByteString, Switch>
        {
            {
                Id1,
                new Switch
                {
                    DevicePowerOn = true,
                    ExhausterPowerOn = true,
                    HeaterPowerOn = true,
                    HeaterFourWayReversingOn = true,
                    HeaterFanOn = true,
                    HeaterCompressorOn = true,
                    HeaterAutoOn = true,
                }
            },
            {
                Id2,
                new Switch
                {
                    DevicePowerOn = true,
                    ExhausterPowerOn = true,
                    HeaterPowerOn = true,
                    HeaterFourWayReversingOn = true,
                    HeaterFanOn = false,
                    HeaterCompressorOn = true,
                    HeaterAutoOn = true,
                }
            },
        };

        public override AsyncUnaryCall<AuthenticateResponse> AuthenticateAsync(
            AuthenticateRequest request,
            Metadata headers = null,
            DateTime? deadline = null,
            CancellationToken cancellationToken = default)
        {
            if (request.Username == "user" && request.Password == "user")
            {
                return TestCalls.AsyncUnaryCall(
                    Task.FromResult(
                        new AuthenticateResponse()
                        {
                            Nickname = "用户1",
                            Role = UserRole.User,
                        }),
                    Task.FromResult(new Metadata()),
                    () => Status.DefaultSuccess,
                    () => new Metadata(),
                    () => { });
            }
            else if (request.Username == "admin" && request.Password == "admin")
            {
                return TestCalls.AsyncUnaryCall(
                    Task.FromResult(
                        new AuthenticateResponse()
                        {
                            Nickname = "管理员1",
                            Role = UserRole.Administrator,
                        }),
                    Task.FromResult(new Metadata()),
                    () => Status.DefaultSuccess,
                    () => new Metadata(),
                    () => { });
            }
            else
            {
                var status = new Status(StatusCode.Unauthenticated, "Invalid username or password.");
                return TestCalls.AsyncUnaryCall(
                    Task.FromException<AuthenticateResponse>(new RpcException(status)),
                    Task.FromResult(new Metadata()),
                    () => status,
                    () => new Metadata(),
                    () => { });
            }
        }

        public override AsyncUnaryCall<ListDevicesResponse> ListDevicesAsync(ListDevicesRequest request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
        {
            var response = new ListDevicesResponse();
            response.Devices.Add(Devices.Values);
            return TestCalls.AsyncUnaryCall(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });
        }

        public override AsyncUnaryCall<WorkingMode> GetWorkingModeAsync(GetWorkingModeRequest request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
        {
            return TestCalls.AsyncUnaryCall(
                Task.FromResult(WorkingModes[request.DeviceId]),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });
        }

        public override AsyncUnaryCall<WorkingMode> UpdateWorkingModeAsync(UpdateWorkingModeRequest request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
        {
            WorkingMode workingMode = WorkingModes[request.DeviceId];
            if (request.UpdateMask == null)
            {
                workingMode.MergeFrom(request.WorkingMode);
            }
            else
            {
                request.UpdateMask.Merge(request.WorkingMode, workingMode);
            }

            return TestCalls.AsyncUnaryCall(
                Task.FromResult(workingMode),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });
        }

        public override AsyncUnaryCall<RunningParameter> GetRunningParameterAsync(GetRunningParameterRequest request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
        {
            return TestCalls.AsyncUnaryCall(
                Task.FromResult(RunningParameters[request.DeviceId]),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });
        }

        public override AsyncUnaryCall<RunningParameter> UpdateRunningParameterAsync(UpdateRunningParameterRequest request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
        {
            RunningParameter runningParameter = RunningParameters[request.DeviceId];
            if (request.UpdateMask == null)
            {
                runningParameter.MergeFrom(request.RunningParameter);
            }
            else
            {
                request.UpdateMask.Merge(request.RunningParameter, runningParameter);
            }

            return TestCalls.AsyncUnaryCall(
                Task.FromResult(runningParameter),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });
        }

        public override AsyncUnaryCall<ListMetricsResponse> ListMetricsAsync(ListMetricsRequest request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
        {
            DateTime? startDateTime = request.StartTime?.ToDateTime();
            DateTime endDateTime;
            if (string.IsNullOrEmpty(request.PageToken))
            {
                endDateTime = request.EndTime?.ToDateTime() ?? DateTime.UtcNow;
            }
            else
            {
                endDateTime = DateTime.Parse(request.PageToken, CultureInfo.InvariantCulture);
            }

            endDateTime = endDateTime.ToUniversalTime();

            var metrics = new List<Metric>(request.PageSize);
            for (int i = 0; i < request.PageSize; i++)
            {
                endDateTime = endDateTime.Subtract(TimeSpan.FromSeconds(5));
                if (startDateTime.HasValue && startDateTime > endDateTime)
                {
                    break;
                }

                metrics.Add(new Metric
                {
                    CreateTime = Timestamp.FromDateTime(endDateTime),
                    InputWaterCelsiusDegree = RandomUtils.NextFloat(10, 20),
                    OutputWaterCelsiusDegree = RandomUtils.NextFloat(10, 20),
                    HeaterOutputWaterCelsiusDegree = RandomUtils.NextFloat(10, 20),
                    EnvironmentCelsiusDegree = RandomUtils.NextFloat(10, 20),
                    HeaterPowerKilowatt = RandomUtils.NextFloat(0, 12),
                    WaterPumpFlowRateCubicMeterPerHour = RandomUtils.NextFloat(1, 3),
                });
            }

            var response = new ListMetricsResponse
            {
                NextPageToken = endDateTime.ToString(CultureInfo.InvariantCulture),
            };
            response.Metrics.AddRange(metrics);

            return TestCalls.AsyncUnaryCall(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });
        }

        public override AsyncUnaryCall<Metric> GetMetricAsync(GetMetricRequest request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
        {
            return TestCalls.AsyncUnaryCall(
                Task.FromResult(new Metric
                {
                    InputWaterCelsiusDegree = RandomUtils.NextFloat(10, 20),
                    OutputWaterCelsiusDegree = RandomUtils.NextFloat(10, 20),
                    HeaterOutputWaterCelsiusDegree = RandomUtils.NextFloat(10, 20),
                    EnvironmentCelsiusDegree = RandomUtils.NextFloat(10, 20),
                    HeaterPowerKilowatt = RandomUtils.NextFloat(0, 12),
                    WaterPumpFlowRateCubicMeterPerHour = RandomUtils.NextFloat(1, 3),
                }),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });
        }

        public override AsyncUnaryCall<Switch> GetSwitchAsync(GetSwitchRequest request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
        {
            Switch switchInfo = Switches[request.DeviceId];

            return TestCalls.AsyncUnaryCall(
                Task.FromResult(switchInfo),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });
        }

        public override AsyncUnaryCall<Switch> UpdateSwitchAsync(UpdateSwitchRequest request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
        {
            Switch switchInfo = Switches[request.DeviceId] ?? new Switch();

            if (request.UpdateMask == null)
            {
                switchInfo.MergeFrom(request.Switch);
            }
            else
            {
                request.UpdateMask.Merge(request.Switch, switchInfo);
            }

            Switches[request.DeviceId] = switchInfo;

            return TestCalls.AsyncUnaryCall(
                Task.FromResult(switchInfo),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });
        }
    }
}
