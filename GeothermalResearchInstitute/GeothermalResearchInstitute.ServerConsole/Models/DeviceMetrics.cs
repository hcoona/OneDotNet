// <copyright file="DeviceMetrics.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;

namespace GeothermalResearchInstitute.ServerConsole.Models
{
    [SuppressMessage("Performance", "CA1819:属性不应返回数组", Justification = "Disable for DTO.")]
    public class DeviceMetrics
    {
        public byte[] Id { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        // 出水温度
        public float WaterOutTemperature { get; set; }

        // 回水温度
        public float WaterInTemperature { get; set; }

        // 加热器出水温度
        public float HeaterWaterOutTemperature { get; set; }

        // 环境温度
        public float EnvironmentTemperature { get; set; }

        // 出水压力
        public float WaterOutPressure { get; set; }

        // 回水压力
        public float WaterInPressure { get; set; }

        // 加热器加热功率
        public float HeaterPower { get; set; }

        // 流量
        public float FlowCapacity { get; set; }
    }
}
