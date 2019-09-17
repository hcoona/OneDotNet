// <copyright file="ConvertUtils.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using GeothermalResearchInstitute.ServerConsole.Models;
using GeothermalResearchInstitute.v1;
using GrpcDevice = GeothermalResearchInstitute.v1.Device;
using GrpcDeviceMetrics = GeothermalResearchInstitute.v1.DeviceMetrics;
using ModelDeviceMetrics = GeothermalResearchInstitute.ServerConsole.Models.DeviceMetrics;

namespace GeothermalResearchInstitute.ServerConsole.Utils
{
    internal static class ConvertUtils
    {
        public static GrpcDevice AssignNameFrom(this GrpcDevice device, DeviceOptionsEntry entry)
        {
            device.Name = entry.Name;
            return device;
        }

        public static GrpcDevice AssignWorkingModeFrom(this GrpcDevice device, DeviceStates states)
        {
            device.WorkingMode = states.WorkingMode;
            return device;
        }

        public static GrpcDevice AssignWorkingModeTo(this GrpcDevice device, DeviceStates states)
        {
            states.WorkingMode = device.WorkingMode;
            return device;
        }

        public static GrpcDevice AssignOptionFrom(this GrpcDevice device, DeviceStates states)
        {
            device.DeviceOption = new DeviceOption
            {
                SummerTemperature = states.SummerTemperature,
                WinterTemperature = states.WinterTemperature,
                WarmCapacity = states.WarmCapacity,
                ColdCapacity = states.ColdCapacity,
                FlowCapacity = states.FlowCapacity,
                RateCapacity = states.RateCapacity,
                MotorMode = states.MotorMode,
                WaterPumpMode = states.WaterPumpMode,
            };
            return device;
        }

        public static GrpcDevice AssignOptionTo(this GrpcDevice device, DeviceStates states)
        {
            states.SummerTemperature = device.DeviceOption.SummerTemperature;
            states.WinterTemperature = device.DeviceOption.WinterTemperature;
            states.WarmCapacity = device.DeviceOption.WarmCapacity;
            states.ColdCapacity = device.DeviceOption.ColdCapacity;
            states.FlowCapacity = device.DeviceOption.FlowCapacity;
            states.RateCapacity = device.DeviceOption.RateCapacity;
            states.MotorMode = device.DeviceOption.MotorMode;
            states.WaterPumpMode = device.DeviceOption.WaterPumpMode;
            return device;
        }

        public static GrpcDevice AssignControlsFrom(this GrpcDevice device, DeviceStates states)
        {
            device.Controls = new DeviceControls
            {
                DevicePower = states.DevicePower,
                ExhaustPower = states.ExhaustPower,
                HeatPumpAuto = states.HeatPumpAuto,
                HeatPumpPower = states.HeatPumpPower,
                HeatPumpFanOn = states.HeatPumpFanOn,
                HeatPumpCompressorOn = states.HeatPumpCompressorOn,
                HeatPumpFourWayReversingValue = states.HeatPumpFourWayReversingValue,
            };
            return device;
        }

        public static GrpcDevice AssignControlsTo(this GrpcDevice device, DeviceStates states)
        {
            states.DevicePower = device.Controls.DevicePower;
            states.ExhaustPower = device.Controls.ExhaustPower;
            states.HeatPumpAuto = device.Controls.HeatPumpAuto;
            states.HeatPumpPower = device.Controls.HeatPumpPower;
            states.HeatPumpFanOn = device.Controls.HeatPumpFanOn;
            states.HeatPumpCompressorOn = device.Controls.HeatPumpCompressorOn;
            states.HeatPumpFourWayReversingValue = device.Controls.HeatPumpFourWayReversingValue;
            return device;
        }

        public static GrpcDeviceMetrics AssignTo(this GrpcDeviceMetrics grpcDeviceMetrics, ModelDeviceMetrics metrics)
        {
            metrics.Timestamp = grpcDeviceMetrics.UpdateTimestamp.ToDateTimeOffset();
            metrics.WaterOutTemperature = grpcDeviceMetrics.WaterOutTemperature;
            metrics.WaterInTemperature = grpcDeviceMetrics.WaterInTemperature;
            metrics.HeaterWaterOutTemperature = grpcDeviceMetrics.HeaterWaterOutTemperature;
            metrics.EnvironmentTemperature = grpcDeviceMetrics.EnvironmentTemperature;
            metrics.WaterOutPressure = grpcDeviceMetrics.WaterOutPressure;
            metrics.WaterInPressure = grpcDeviceMetrics.WaterInPressure;
            metrics.HeaterPower = grpcDeviceMetrics.HeaterPower;
            metrics.FlowCapacity = grpcDeviceMetrics.FlowCapacity;
            return grpcDeviceMetrics;
        }

        public static GrpcDeviceMetrics AssignFrom(this GrpcDeviceMetrics grpcDeviceMetrics, ModelDeviceMetrics metrics)
        {
            grpcDeviceMetrics.UpdateTimestamp.Seconds = metrics.Timestamp.ToUniversalTime().ToUnixTimeSeconds();
            grpcDeviceMetrics.WaterOutTemperature = metrics.WaterOutTemperature;
            grpcDeviceMetrics.WaterInTemperature = metrics.WaterInTemperature;
            grpcDeviceMetrics.HeaterWaterOutTemperature = metrics.HeaterWaterOutTemperature;
            grpcDeviceMetrics.EnvironmentTemperature = metrics.EnvironmentTemperature;
            grpcDeviceMetrics.WaterOutPressure = metrics.WaterOutPressure;
            grpcDeviceMetrics.WaterInPressure = metrics.WaterInPressure;
            grpcDeviceMetrics.HeaterPower = metrics.HeaterPower;
            grpcDeviceMetrics.FlowCapacity = metrics.FlowCapacity;
            return grpcDeviceMetrics;
        }
    }
}
