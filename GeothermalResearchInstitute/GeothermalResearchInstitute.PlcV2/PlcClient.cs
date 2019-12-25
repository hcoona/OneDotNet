// <copyright file="PlcClient.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Logging;

namespace GeothermalResearchInstitute.PlcV2
{
    public partial class PlcClient : IDisposable
    {
        private readonly ILogger logger;

        private readonly TcpClient tcpClient;

        private readonly BufferBlock<PlcRequestContext> requestContextSendingBufferBlock =
            new BufferBlock<PlcRequestContext>(new DataflowBlockOptions
            {
                BoundedCapacity = 100,
                EnsureOrdered = false,
                TaskScheduler = TaskScheduler.Default,
            });

        private readonly Task sendingBackgroundTask;
        private readonly Task receivingBackgroundTask;
        private readonly Task deadlineBackgroundTask;
        private readonly SemaphoreSlim mutex = new SemaphoreSlim(1);
        private readonly CancellationTokenSource closingCancellationTokenSource = new CancellationTokenSource();

        private readonly ConcurrentDictionary<int, PlcRequestContext> requestContextReceivingDictionary =
            new ConcurrentDictionary<int, PlcRequestContext>();

        private int sequenceNumberGenerator = 0;
        private bool disposedValue = false;

        public PlcClient(ILogger<PlcClient> logger, TcpClient tcpClient)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.tcpClient = tcpClient ?? throw new ArgumentNullException(nameof(tcpClient));
            this.RemoteEndPoint = tcpClient.Client.RemoteEndPoint;

            this.sendingBackgroundTask = Task.Factory.StartNew(
                this.SendingBackgroundTaskEntryPoint,
                CancellationToken.None,
                TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default);
            this.receivingBackgroundTask = Task.Factory.StartNew(
                this.ReceivingBackgroundTaskEntryPoint,
                CancellationToken.None,
                TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default);
            this.deadlineBackgroundTask = Task.Factory.StartNew(
                this.DeadlineBackgroundTaskEntryPoint,
                CancellationToken.None,
                TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default);
        }

        ~PlcClient()
        {
            this.Dispose(false);
        }

        public event EventHandler OnClosed;

        public event EventHandler<byte[]> OnDebugSending;

        public event EventHandler<byte[]> OnDebugReceiving;

        public EndPoint RemoteEndPoint { get; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task Close()
        {
            await this.mutex.WaitAsync().ConfigureAwait(false);
            try
            {
                if (!this.closingCancellationTokenSource.IsCancellationRequested)
                {
                    this.closingCancellationTokenSource.Cancel();
                    this.requestContextSendingBufferBlock.Complete();

                    await this.sendingBackgroundTask.ConfigureAwait(false);
                    await this.receivingBackgroundTask.ConfigureAwait(false);
                    await this.deadlineBackgroundTask.ConfigureAwait(false);

                    this.tcpClient.Close();
                    this.OnClosed?.Invoke(this, EventArgs.Empty);
                }
            }
            finally
            {
                this.mutex.Release();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.Close().GetAwaiter().GetResult();

                    this.sendingBackgroundTask.Dispose();
                    this.receivingBackgroundTask.Dispose();
                    this.deadlineBackgroundTask.Dispose();

                    this.tcpClient.Dispose();
                    this.closingCancellationTokenSource.Dispose();
                    this.mutex.Dispose();
                }

                this.disposedValue = true;
            }
        }
    }
}
