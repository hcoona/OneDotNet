// <copyright file="PlcHostedService.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using GeothermalResearchInstitute.PlcV2;
using GeothermalResearchInstitute.v2;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GeothermalResearchInstitute.ServerConsole
{
    [SuppressMessage(
        "Design",
        "CA1001:具有可释放字段的类型应该是可释放的",
        Justification = "Disposed in StopAsync, ensured by framework.")]
    public class PlcHostedService : IHostedService
    {
        private readonly ILogger<PlcHostedService> logger;
        private readonly PlcServer plcServer;

        private readonly ConcurrentDictionary<ByteString, PlcClient> plcDictionary =
            new ConcurrentDictionary<ByteString, PlcClient>();

        private CancellationTokenSource cancellationTokenSource;
        private Task backgroundTask;

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

            this.plcDictionary.Clear();
            this.cancellationTokenSource = new CancellationTokenSource();
            this.backgroundTask = Task.Factory.StartNew(
                this.BackgroundTaskEntryPoint,
                this.cancellationTokenSource.Token,
                TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default);

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            this.cancellationTokenSource.Cancel();
            await this.backgroundTask.ConfigureAwait(false);

            foreach (ByteString id in this.plcDictionary.Keys)
            {
                if (this.plcDictionary.TryRemove(id, out PlcClient client))
                {
                    client.Close();
                    client.Dispose();
                }
            }

            this.plcServer.Stop();

            this.cancellationTokenSource.Dispose();
            this.cancellationTokenSource = null;
            this.backgroundTask.Dispose();
            this.backgroundTask = null;
        }

        private async void BackgroundTaskEntryPoint()
        {
            while (!this.cancellationTokenSource.IsCancellationRequested)
            {
                PlcClient client = await this.plcServer.AcceptAsync().ConfigureAwait(false);

                ConnectResponse response;
                try
                {
                    response = await client
                        .ConnectAsync(new ConnectRequest(), DateTime.UtcNow.AddSeconds(10))
                        .ConfigureAwait(false);
                }
                catch (RpcException e)
                {
                    this.logger.LogWarning(
                        e,
                        "Failed to send ConnectRequest to newly PLC {0}",
                        client.RemoteEndPoint);
                    continue;
                }

                if (this.plcDictionary.TryAdd(response.Id, client))
                {
                    this.logger.LogInformation(
                        "Client(MAC={0}, EndPoint={1}) connected.",
                        BitConverter.ToString(response.Id.ToByteArray()),
                        client.RemoteEndPoint);
                    client.OnClosed += (sender, args) =>
                    {
                        this.logger.LogInformation(
                            "Client(MAC={0}, EndPoint={1}) disconnected.",
                            BitConverter.ToString(response.Id.ToByteArray()),
                            client.RemoteEndPoint);
                        this.plcDictionary.TryRemove(response.Id, out PlcClient _);
                    };
                }
                else
                {
                    this.logger.LogWarning(
                        "Failed to add the client(MAC={0}, EndPoint={1}) into dictionary.",
                        BitConverter.ToString(response.Id.ToByteArray()),
                        client.RemoteEndPoint);
                    client.Close();
                    client.Dispose();
                }
            }
        }
    }
}
