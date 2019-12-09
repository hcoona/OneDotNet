// <copyright file="PlcHostedService.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using GeothermalResearchInstitute.PlcV2;
using Google.Protobuf;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GeothermalResearchInstitute.ServerConsole
{
    public class PlcHostedService : IHostedService
    {
        private readonly ILogger<PlcHostedService> logger;
        private readonly PlcServer plcServer;

        private readonly ConcurrentDictionary<ByteString, PlcClient> plcDictionary =
            new ConcurrentDictionary<ByteString, PlcClient>();

        public PlcHostedService(ILogger<PlcHostedService> logger, PlcServer plcServer)
        {
            this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            this.plcServer = plcServer ?? throw new System.ArgumentNullException(nameof(plcServer));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled(cancellationToken);
            }

            this.plcServer.Start();
            this.logger.LogInformation("PLC server is listening on {0}", this.plcServer.LocalEndPoint);

            // TODO: Run background task
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.plcServer.Stop();
            return Task.CompletedTask;
        }
    }
}
