// <copyright file="CoreOptions.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace GeothermalResearchInstitute.Wpf.Options
{
    public class CoreOptions
    {
        public string ServerGrpcAddress { get; set; }

        public int ServerGrpcPort { get; set; }

        public int DefaultReadTimeoutMillis { get; set; }

        public int DefaultWriteTimeoutMillis { get; set; }

        public int DefaultPageSize { get; set; } = 50;

        public int DefaultRefreshIntervalMillis { get; set; } = 1000;

        public int MaxErrorToleranceNum { get; set; } = 5;
    }
}
