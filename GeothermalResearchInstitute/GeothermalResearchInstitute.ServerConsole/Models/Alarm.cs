// <copyright file="Alarm.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace GeothermalResearchInstitute.ServerConsole.Models
{
    public class Alarm
    {
        public string DeviceId { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        // 流量低
        public bool LowFlowRate { get; set; }

        // 热泵压力高
        public bool HighHeaterPressure { get; set; }

        // 热泵压力低
        public bool LowHeaterPressure { get; set; }

        // 电源断相或相序错
        public bool NoPower { get; set; }

        // 热泵过载故障（热继电器）
        public bool HeaterOverloadedBroken { get; set; }

        // 电加热器故障
        public bool ElectricalHeaterBroken { get; set; }

        // 系统缺水故障
        public bool NoWater { get; set; }

        // 电源电压过高
        public bool HighVoltage { get; set; }

        // 电源电压过低
        public bool LowVoltage { get; set; }

        // 急停开关被按下
        public bool EmergencyStopped { get; set; }
    }
}
