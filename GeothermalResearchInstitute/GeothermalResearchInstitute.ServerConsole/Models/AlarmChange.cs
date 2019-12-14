// <copyright file="AlarmChange.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using GeothermalResearchInstitute.v2;

namespace GeothermalResearchInstitute.ServerConsole.Models
{
    public class AlarmChange
    {
        public string DeviceId { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public AlarmType Type { get; set; }

        public AlarmChangeDirection Direction { get; set; }
    }
}
