// <copyright file="Device.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using GeothermalResearchInstitute.v1;

namespace GeothermalResearchInstitute.ServerConsole.Model
{
    public class Device
    {
        [Key]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Performance", "CA1819:属性不应返回数组", Justification = "Disable for DTO.")]
        public byte[] Id { get; set; }

        public string Name { get; set; }

        public DeviceWorkingMode WorkingMode { get; set; }

        public float SummerTemperature { get; set; }

        public float WinterTemperature { get; set; }

        public float WarmCapacity { get; set; }

        public float ColdCapacity { get; set; }

        public float FlowCapacity { get; set; }

        public float RateCapacity { get; set; }

        public MotorMode MotorMode { get; set; }

        public WaterPumpMode WaterPumpMode { get; set; }

        public bool DevicePower { get; set; }

        public bool ExhaustPower { get; set; }

        public bool HeatPumpAuto { get; set; }

        public bool HeatPumpPower { get; set; }

        public bool HeatPumpFanOn { get; set; }

        public bool HeatPumpCompressorOn { get; set; }

        public bool HeatPumpFourWayReversingValue { get; set; }
    }
}
