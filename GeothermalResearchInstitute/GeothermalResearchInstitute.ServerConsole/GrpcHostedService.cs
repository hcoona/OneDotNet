// <copyright file="GrpcHostedService.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GeothermalResearchInstitute.ServerConsole
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Performance", "CA1812", Justification = "Instantiated with reflection.")]
    public class GrpcHostedService : IHostedService
    {
        private readonly ILogger<GrpcHostedService> logger;
        private readonly Server server;

        public GrpcHostedService(ILogger<GrpcHostedService> logger, Server server)
        {
            this.logger = logger;
            this.server = server;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled(cancellationToken);
            }

            this.server.Start();
            if (this.logger.IsEnabled(LogLevel.Information))
            {
                this.logger.LogInformation(
                    "Grpc services are listening on {}",
                    string.Join(",", this.server.Ports.Select(p => $"{p.Host}:{p.Port}")));
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Killing grpc server...");
            return this.server.KillAsync();
        }
    }
}
