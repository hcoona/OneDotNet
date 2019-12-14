// <copyright file="KeepLoggingHostedService.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SerilogLab
{
    public class KeepLoggingHostedService : IHostedService
    {
        private readonly ILogger<KeepLoggingHostedService> logger;
        private readonly IConfiguration configuration;

        public KeepLoggingHostedService(
            ILogger<KeepLoggingHostedService> logger,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Hello World!");
            this.logger.LogInformation("{0}", this.configuration.GetValue<string>("Serilog:Using:0"));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Goodbye World!");
            return Task.CompletedTask;
        }
    }
}
