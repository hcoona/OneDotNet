// <copyright file="PlcHostedService.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace GeothermalResearchInstitute.ServerConsole
{
    public class PlcHostedService : IHostedService
    {
        private readonly PlcManager plcManager;

        public PlcHostedService(PlcManager plcManager)
        {
            this.plcManager = plcManager ?? throw new ArgumentNullException(nameof(plcManager));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return this.plcManager.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return this.plcManager.StopAsync();
        }
    }
}
