// <copyright file="PlcManager.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using GeothermalResearchInstitute.PlcV2;
using GeothermalResearchInstitute.v2;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GeothermalResearchInstitute.ServerConsole
{
    [SuppressMessage(
        "Design",
        "CA1001:具有可释放字段的类型应该是可释放的",
        Justification = "Disposed in StopAsync, ensured by framework.")]
    public class PlcManager
    {
        private readonly ILogger logger;
        private readonly PlcServer plcServer;
        private CancellationTokenSource cancellationTokenSource;
        private Task backgroundTask;

        public PlcManager(ILogger<PlcManager> logger, PlcServer plcServer)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.plcServer = plcServer ?? throw new ArgumentNullException(nameof(plcServer));
        }

        public ConcurrentDictionary<ByteString, PlcClient> PlcDictionary { get; } =
            new ConcurrentDictionary<ByteString, PlcClient>();

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled(cancellationToken);
            }

            this.plcServer.Start();
            this.logger.LogInformation("PLC server is listening on {0}", this.plcServer.LocalEndPoint);

            this.PlcDictionary.Clear();
            this.cancellationTokenSource = new CancellationTokenSource();
            this.backgroundTask = Task.Factory.StartNew(
                this.BackgroundTaskEntryPoint,
                this.cancellationTokenSource.Token,
                TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default);

            return Task.CompletedTask;
        }

        public async Task StopAsync()
        {
            this.cancellationTokenSource.Cancel();
            await this.backgroundTask.ConfigureAwait(true);

            foreach (ByteString id in this.PlcDictionary.Keys)
            {
                if (this.PlcDictionary.TryRemove(id, out PlcClient client))
                {
                    await client.Close().ConfigureAwait(true);
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
                PlcClient client;
                try
                {
                    client = await this.plcServer.AcceptAsync().ConfigureAwait(true);
                }
                catch (SocketException e)
                {
                    this.logger.LogError(e, "Failed to accept PLC.");
                    continue;
                }

                ConnectResponse response;
                try
                {
                    response = await client
                        .ConnectAsync(new ConnectRequest(), DateTime.UtcNow.AddSeconds(10))
                        .ConfigureAwait(true);
                }
                catch (RpcException e)
                {
                    this.logger.LogWarning(
                        e,
                        "Failed to send ConnectRequest to newly PLC {0}",
                        client.RemoteEndPoint);
                    continue;
                }

                if (this.PlcDictionary.TryAdd(response.Id, client))
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
                        this.PlcDictionary.TryRemove(response.Id, out PlcClient _);
                    };
                }
                else
                {
                    this.logger.LogWarning(
                        "Failed to add the client(MAC={0}, EndPoint={1}) into dictionary.",
                        BitConverter.ToString(response.Id.ToByteArray()),
                        client.RemoteEndPoint);
                    await client.Close().ConfigureAwait(true);
                    client.Dispose();
                }
            }
        }
    }
}
