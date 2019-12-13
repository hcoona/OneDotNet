// <copyright file="Metric.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace GeothermalResearchInstitute.ServerConsole.Models
{
    public class Metric
    {
        public string DeviceId { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        // 出水温度
        public float OutputWaterCelsiusDegree { get; set; }

        // 回水温度
        public float InputWaterCelsiusDegree { get; set; }

        // 加热器出水温度
        public float HeaterOutputWaterCelsiusDegree { get; set; }

        // 环境温度
        public float EnvironmentCelsiusDegree { get; set; }

        // 出水压力
        public float OutputWaterPressureMeter { get; set; }

        // 回水压力
        public float InputWaterPressureMeter { get; set; }

        // 加热器加热功率
        public float HeaterPowerKilowatt { get; set; }

        // 流量
        public float WaterPumpFlowRateCubicMeterPerHour { get; set; }
    }
}
