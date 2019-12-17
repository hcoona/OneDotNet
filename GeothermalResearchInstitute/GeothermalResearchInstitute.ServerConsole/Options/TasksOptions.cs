// <copyright file="TasksOptions.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Text.Json;

namespace GeothermalResearchInstitute.ServerConsole.Options
{
    public class TasksOptions
    {
        public int CollectAlarmIntervalMillis { get; set; }

        public int CollectMetricIntervalMillis { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
