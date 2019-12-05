// <copyright file="PlcClient.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using GeothermalResearchInstitute.v2;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GeothermalResearchInstitute.Plc
{
    public partial class PlcClient : IDisposable
    {
        private readonly ILogger logger;
        private readonly TcpClient client;
        private readonly ConcurrentDictionary<int, RequestContext> receivingContexts =
            new ConcurrentDictionary<int, RequestContext>();

        private volatile bool started = false;
        private volatile bool stopped = false;
        private ManualResetEvent stoppedEvent;
        private EndPoint remoteEndPoint;
        private BufferBlock<RequestContext> requestQueue;
        private int counter;
        private Task backgroundSendingTask;
        private Task backgroundReceivingTask;
        private Task backgroundDeadlineCollectionTask;

        private bool disposedValue = false;

        public PlcClient(ILogger<PlcClient> logger, TcpClient client)
        {
            this.logger = logger;
            this.client = client;
            this.ConnectionClosed += async (_, __) =>
            {
                await this.StopAsync().ConfigureAwait(false);
            };
        }

        public event EventHandler ConnectionClosed;

        public EndPoint RemoteEndPoint => this.remoteEndPoint;

        public Task StartAsync()
        {
            if (this.started)
            {
                return Task.CompletedTask;
            }

            if (!this.client.Connected)
            {
                return Task.FromException(new InvalidOperationException("Client disconnected"));
            }

            this.remoteEndPoint = this.client.Client?.RemoteEndPoint;
            this.stoppedEvent = new ManualResetEvent(false);
            this.requestQueue = new BufferBlock<RequestContext>(new DataflowBlockOptions
            {
                BoundedCapacity = 20,
                EnsureOrdered = false,
                TaskScheduler = TaskScheduler.Default,
            });
            this.counter = 0;

            this.backgroundSendingTask = Task.Factory.StartNew(
                this.BackgroundSendingTaskEntryPoint,
                Task.Factory.CancellationToken,
                TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default);
            this.backgroundReceivingTask = Task.Factory.StartNew(
                this.BackgroundReceivingTaskEntryPoint,
                Task.Factory.CancellationToken,
                TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default);
            this.backgroundDeadlineCollectionTask = Task.Factory.StartNew(
                this.BackgroundDeadlineCollectionTaskEntryPoint,
                Task.Factory.CancellationToken,
                TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default);

            this.started = true;
            return Task.CompletedTask;
        }

        public async Task StopAsync()
        {
            if (this.started && !this.stopped)
            {
                this.stoppedEvent.Set();
                this.requestQueue.Complete();

                await this.backgroundSendingTask.ConfigureAwait(false);
                await this.backgroundReceivingTask.ConfigureAwait(false);

                this.stopped = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposedValue)
            {
                return;
            }

            if (disposing)
            {
                if (this.started)
                {
                    if (!this.stopped)
                    {
                        this.StopAsync().GetAwaiter().GetResult();
                    }

                    this.stoppedEvent.Dispose();
                    this.backgroundSendingTask.Dispose();
                    this.backgroundReceivingTask.Dispose();
                    this.client.Dispose();
                }
            }

            this.disposedValue = true;
        }

        private async Task<TResponse> InvokeAsync<TRequest, TResponse>(
            string path,
            TRequest request,
            MessageParser<TResponse> responseParser,
            DateTime? deadline = null,
            CancellationToken cancellationToken = default)
            where TRequest : IMessage<TRequest>
            where TResponse : IMessage<TResponse>
        {
            var promise = new TaskCompletionSource<UnifiedFrameContent>();

            await this.requestQueue
                .SendAsync(
                    new RequestContext
                    {
                        Path = path,
                        Message = request,
                        Deadline = deadline,
                        CancellationToken = cancellationToken,
                        TaskCompletionSource = promise,
                    },
                    cancellationToken)
                .ConfigureAwait(false);
            return await promise.Task
                .ContinueWith(
                    future =>
                    {
                        if (future.IsFaulted)
                        {
                            throw new RpcException(new Status(
                                StatusCode.Internal, future.Exception.ToString()));
                        }

                        if (future.IsCanceled)
                        {
                            throw new RpcException(Status.DefaultCancelled);
                        }

                        UnifiedFrameContent frame = future.Result;
                        if (frame.Header.Status == 0)
                        {
                            return responseParser.ParseFrom(frame.Payload);
                        }
                        else
                        {
                            throw new RpcException(new Status(
                                (StatusCode)frame.Header.Status,
                                frame.Header.StatusMessage));
                        }
                    },
                    cancellationToken,
                    TaskContinuationOptions.DenyChildAttach,
                    TaskScheduler.Default)
                .ConfigureAwait(false);
        }

        private void BackgroundDeadlineCollectionTaskEntryPoint()
        {
            while (!this.stoppedEvent.WaitOne(100))
            {
                DateTime now = DateTime.Now;
                foreach (KeyValuePair<int, RequestContext> p in this.receivingContexts)
                {
                    if (p.Value.CancellationToken.IsCancellationRequested
                        || now > p.Value.Deadline)
                    {
                        if (this.receivingContexts.TryRemove(p.Key, out var _))
                        {
                            p.Value.TaskCompletionSource.SetResult(new UnifiedFrameContent
                            {
                                Header = new Header
                                {
                                    Status = (int)StatusCode.DeadlineExceeded,
                                    StatusMessage = "Didn't receive message before deadline",
                                },
                            });
                        }
                    }
                }
            }
        }
    }
}
