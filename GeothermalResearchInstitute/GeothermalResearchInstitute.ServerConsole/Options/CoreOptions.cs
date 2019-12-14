// <copyright file="CoreOptions.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace GeothermalResearchInstitute.ServerConsole.Options
{
    public class CoreOptions
    {
        public int GrpcPort { get; set; }

        public int TcpPort { get; set; }

        public int DefaultReadTimeoutMillis { get; set; }

        public int MaxFakeDeviceNum { get; set; }
    }
}
