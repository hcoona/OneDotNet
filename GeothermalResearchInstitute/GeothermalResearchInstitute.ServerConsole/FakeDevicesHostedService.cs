// <copyright file="FakeDevicesHostedService.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using GeothermalResearchInstitute.ServerConsole.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace GeothermalResearchInstitute.ServerConsole
{
    [SuppressMessage("Microsoft.Performance", "CA1812", Justification = "Instantiated with reflection.")]
    internal class FakeDevicesHostedService : IHostedService, IDisposable
    {
        private readonly ICollection<DeviceOptionsEntry> devices;
        private readonly IServiceProvider serviceProvider;
        private Timer timer = null;
        private bool disposedValue = false;

        public FakeDevicesHostedService(
            IOptions<DeviceOptions> deviceOptions,
            IServiceProvider serviceProvider)
        {
            this.devices = deviceOptions.Value.Devices;
            this.serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.timer = new Timer(
                this.HearbeatEntryPoint,
                null,
                0,
                (long)TimeSpan.FromSeconds(1).TotalMilliseconds);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.timer.Dispose();
            this.timer = null;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    if (this.timer != null)
                    {
                        this.timer.Dispose();
                        this.timer = null;
                    }
                }

                this.disposedValue = true;
            }
        }

        [SuppressMessage("样式", "IDE0060:删除未使用的参数", Justification = "Required for callback delegate.")]
        private void HearbeatEntryPoint(object state)
        {
            foreach (DeviceOptionsEntry entry in this.devices)
            {
                // TODO(zhangshuai.ustc): Implement it.
                // 1. Get corresponding grpc client.
                // 2. Send heartbeat request according to corresponding states.
                // 3. Take action according to heatbeat response & record into states.
            }

            throw new NotImplementedException();
        }
    }
}
