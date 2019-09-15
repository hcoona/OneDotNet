using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GeothermalResearchInstitute.v1;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Core.Testing;

namespace GeothermalResearchInstitute.Wpf.FakeClients
{
    internal class FakeDeviceServiceClient : DeviceService.DeviceServiceClient
    {
        static Device device1 = new Device()
        {
            Id = ByteString.CopyFromUtf8("111"),
            Name = "Test1",
            WorkingMode = DeviceWorkingMode.KeepWarmCapacity,
            DeviceOption = new DeviceOption()
                        {
                            SummerTemperature = 1.1F,
                            WinterTemperature = 1.2F,
                            WarmCapacity = 1.3F,
                            ColdCapacity = 1.4F,
                            FlowCapacity = 1.5F,
                            RateCapacity = 1.6F,
                            MotorMode = MotorMode.VariableFrequency,
                            WaterPumpMode = WaterPumpMode.B,
                        },
            Metrics = new DeviceMetrics()
                        {
                            WaterOutTemperature = 1.1F,
                            WaterInTemperature = 1.2F,
                            HeaterWaterOutTemperature = 1.3F,
                            EnvironmentTemperature = 1.4F,
                            WaterOutPressure = 1.5F,
                            WaterInPressure = 1.6F,
                            HeaterPower = 1.7F,
                            FlowCapacity = 1.8F,
                        },
            Controls = new DeviceControls()
            {
                            DevicePower = true,
                            ExhaustPower = true,
                            HeatPumpAuto = true,
                            HeatPumpPower = true,
                            HeatPumpFanOn = true,
                            HeatPumpCompressorOn = true,
                            HeatPumpFourWayReversingValue = true,
            },
        };

        static Device device2 = new Device()
        {
            Id = ByteString.CopyFromUtf8("222"),
            Name = "Test2",
            WorkingMode = DeviceWorkingMode.KeepWarmCapacity,
            DeviceOption = new DeviceOption()
            {
                SummerTemperature = 2.1F,
                WinterTemperature = 2.2F,
                WarmCapacity = 2.3F,
                ColdCapacity = 2.4F,
                FlowCapacity = 2.5F,
                RateCapacity = 2.6F,
                MotorMode = MotorMode.VariableFrequency,
                WaterPumpMode = WaterPumpMode.B,
            },
            Metrics = new DeviceMetrics()
            {
                WaterOutTemperature = 2.1F,
                WaterInTemperature = 2.2F,
                HeaterWaterOutTemperature = 2.3F,
                EnvironmentTemperature = 2.4F,
                WaterOutPressure = 2.5F,
                WaterInPressure = 2.6F,
                HeaterPower = 2.7F,
                FlowCapacity = 2.8F,
            },
            Controls = new DeviceControls()
            {
                DevicePower = true,
                ExhaustPower = true,
                HeatPumpAuto = true,
                HeatPumpPower = true,
                HeatPumpFanOn = true,
                HeatPumpCompressorOn = true,
                HeatPumpFourWayReversingValue = true,
            },
        };

        public override AsyncUnaryCall<ListDevicesResponse> ListDevicesAsync(
            ListDevicesRequest request,
            Metadata headers = null,
            DateTime? deadline = null,
            CancellationToken cancellationToken = default)
        {
            var response = new ListDevicesResponse();
            response.Devices.Add(new Device()
            {
                Id = device1.Id,
                Name = device1.Name,
            });
            response.Devices.Add(new Device()
            {
                Id = device2.Id,
                Name = device2.Name,
            });

            return TestCalls.AsyncUnaryCall(Task.FromResult(response), Task.FromResult(new Metadata()),
                    () => Status.DefaultSuccess,
                    () => new Metadata(),
                    () => { });
        }

        public override AsyncUnaryCall<Device> GetDeviceAsync(GetDeviceRequest request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
        {
            if (request.Id.Equals(ByteString.CopyFromUtf8("111")))
            {
                return TestCalls.AsyncUnaryCall(
                    Task.FromResult(device1),
                    Task.FromResult(new Metadata()),
                    () => Status.DefaultSuccess,
                    () => new Metadata(),
                    () => { });
            }
            else if (request.Id.Equals(ByteString.CopyFromUtf8("222")))
            {
                return TestCalls.AsyncUnaryCall(
                    Task.FromResult(device2),
                    Task.FromResult(new Metadata()),
                    () => Status.DefaultSuccess,
                    () => new Metadata(),
                    () => { });
            }
            else
            {
                return TestCalls.AsyncUnaryCall(
                    Task.FromResult(new Device()),
                    Task.FromResult(new Metadata()),
                    () => Status.DefaultSuccess,
                    () => new Metadata(),
                    () => { });
            }
        }

        public override AsyncUnaryCall<Device> UpdateDeviceAsync(UpdateDeviceRequest request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
        {
            if (request.Device.Id.Equals(device1.Id))
            {
                // TODO: FieldMask
                return TestCalls.AsyncUnaryCall(
                    Task.FromResult(device1),
                    Task.FromResult(new Metadata()),
                    () => Status.DefaultSuccess,
                    () => new Metadata(),
                    () => { });
            }
            else if (request.Device.Id.Equals(device2.Id))
            {
                // TODO: FieldMask

                return TestCalls.AsyncUnaryCall(
                    Task.FromResult(device2),
                    Task.FromResult(new Metadata()),
                    () => Status.DefaultSuccess,
                    () => new Metadata(),
                    () => { });
            }
            else {
                return TestCalls.AsyncUnaryCall(
                    Task.FromResult(new Device()),
                    Task.FromResult(new Metadata()),
                    () => Status.DefaultSuccess,
                    () => new Metadata(),
                    () => { });
            }
        }
    }


}
