// <copyright file="ConvertUtils.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using GeothermalResearchInstitute.ServerConsole.Models;
using GeothermalResearchInstitute.v1;

namespace GeothermalResearchInstitute.ServerConsole.Utils
{
    internal static class ConvertUtils
    {
        public static Device AssignNameFrom(this Device device, DeviceOptionsEntry entry)
        {
            device.Name = entry.Name;
            return device;
        }

        public static Device AssignWorkingModeFrom(this Device device, DeviceStates states)
        {
            device.WorkingMode = states.WorkingMode;
            return device;
        }

        public static Device AssignWorkingModeTo(this Device device, DeviceStates states)
        {
            states.WorkingMode = device.WorkingMode;
            return device;
        }

        public static Device AssignOptionFrom(this Device device, DeviceStates states)
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

        public static Device AssignOptionTo(this Device device, DeviceStates states)
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

        public static Device AssignControlsFrom(this Device device, DeviceStates states)
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

        public static Device AssignControlsTo(this Device device, DeviceStates states)
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
    }
}
