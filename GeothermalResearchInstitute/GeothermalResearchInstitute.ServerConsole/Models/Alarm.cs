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

        public bool LowFlowRate { get; set; }

        public bool HighHeaterPressure { get; set; }

        public bool LowHeaterPressure { get; set; }

        public bool NoPower { get; set; }

        public bool HeaterOverloadedBroken { get; set; }

        public bool ElectricalHeaterBorken { get; set; }
    }
}
