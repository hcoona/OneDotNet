// <copyright file="MainWindow.xaml.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Buffers.Binary;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using GeothermalResearchInstitute.PlcV2;
using GeothermalResearchInstitute.v2;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using PlcClientList = System.Collections.Generic.List<GeothermalResearchInstitute.PlcV2.PlcClient>;

namespace ProtocolLab
{
    public partial class MainWindow : Window, IDisposable
    {
        private const int PORT = 8888;

        private readonly ILoggerFactory loggerFactory =
            LoggerFactory.Create(b =>
            {
                b.AddDebug();
                b.AddConsole(o => o.IncludeScopes = true);
                b.SetMinimumLevel(LogLevel.Debug);
            });

        private readonly object lockObject = new object();
        private readonly PlcClientList clients = new PlcClientList();

        private bool disposedValue;
        private PlcServer plcServer;
        private CancellationTokenSource cancellationTokenSource;
        private Task backgroundTask;

        private int fakePlcSequenceNumber;
        private TcpClient fakePlc;
        private CancellationTokenSource fakePlcCancellationTokenSource;
        private Task fakePlcBackgroundTask;

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private PlcClient FirstPlcClient
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.clients.FirstOrDefault();
                }
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
                this.loggerFactory.Dispose();
                this.cancellationTokenSource.Dispose();
                this.backgroundTask.Dispose();

                if (this.fakePlc != null)
                {
                    this.FakePlcDown_Click(this, null);
                }

                lock (this.lockObject)
                {
                    foreach (PlcClient c in this.clients)
                    {
                        c.Dispose();
                    }

                    this.clients.Clear();
                }
            }

            this.disposedValue = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.plcServer = new PlcServer(this.loggerFactory, IPAddress.Any, PORT);
            this.plcServer.Start();
            this.LogMessage(string.Format(
                CultureInfo.CurrentCulture,
                "正在监听 {0} 端口，等待 PLC 连接，PLC 连接上来后会在这里收到通知。",
                PORT));

            this.cancellationTokenSource = new CancellationTokenSource();
            this.backgroundTask = Task.Run(this.BackgroundTaskEntryPoint, this.cancellationTokenSource.Token);
        }

        private async void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            this.plcServer.Stop();
            this.cancellationTokenSource.Cancel();
            await this.backgroundTask.ConfigureAwait(true);
        }

        private async void Connect_Click(object sender, RoutedEventArgs e)
        {
            PlcClient c = this.FirstPlcClient;
            if (c == null)
            {
                this.LogMessage("目前没有 PLC 连接。");
                return;
            }

            this.LogMessage("Sending ConnectRequest");
            this.SendingDocument.Blocks.Clear();
            this.ReceivingDocument.Blocks.Clear();
            try
            {
                ConnectResponse response = await c.ConnectAsync(
                    new ConnectRequest(),
                    deadline: DateTime.Now.AddSeconds(30))
                .ConfigureAwait(true);
                this.LogMessage("Received ConnectResponse " + response.ToString());
            }
            catch (RpcException ex)
            {
                this.LogMessage("Failed to receive ConnectResponse " + ex.ToString());
            }
        }

        private async void GetMetric_Click(object sender, RoutedEventArgs e)
        {
            PlcClient c = this.FirstPlcClient;
            if (c == null)
            {
                this.LogMessage("目前没有 PLC 连接。");
                return;
            }

            this.LogMessage("Sending GetMetricRequest");
            this.SendingDocument.Blocks.Clear();
            this.ReceivingDocument.Blocks.Clear();
            try
            {
                Metric response = await c.GetMetricAsync(
                    new GetMetricRequest(),
                    deadline: DateTime.Now.AddSeconds(30))
                .ConfigureAwait(true);
                this.LogMessage("Received GetMetricResponse " + response.ToString());
            }
            catch (RpcException ex)
            {
                this.LogMessage("Failed to receive GetMetricResponse " + ex.ToString());
            }
        }

        private async void UpdateSwitch_Click(object sender, RoutedEventArgs e)
        {
            PlcClient c = this.FirstPlcClient;
            if (c == null)
            {
                this.LogMessage("目前没有 PLC 连接。");
                return;
            }

            this.LogMessage("Sending UpdateSwitchRequest");
            this.SendingDocument.Blocks.Clear();
            this.ReceivingDocument.Blocks.Clear();
            try
            {
                Switch response = await c.UpdateSwitchAsync(
                    new UpdateSwitchRequest
                    {
                        Switch = new Switch
                        {
                            HeaterAutoOn = true,
                        },
                        UpdateMask = FieldMask.FromString<Switch>("heater_auto_on"),
                    },
                    deadline: DateTime.Now.AddSeconds(30))
                .ConfigureAwait(true);
                this.LogMessage("Received UpdateSwitchResponse " + response.ToString());
            }
            catch (RpcException ex)
            {
                this.LogMessage("Failed to receive UpdateSwitchResponse " + ex.ToString());
            }
        }

        private void FakePlcUp_Click(object sender, RoutedEventArgs e)
        {
            if (this.fakePlc != null)
            {
                this.LogMessage("伪装 PLC 已经上线");
                return;
            }

            this.fakePlc = new TcpClient("127.0.0.1", PORT);
            this.fakePlcSequenceNumber = 0;
            this.fakePlcCancellationTokenSource = new CancellationTokenSource();
            this.fakePlcBackgroundTask = Task.Run(async () =>
            {
                CancellationToken token = this.fakePlcCancellationTokenSource.Token;
                while (!token.IsCancellationRequested)
                {
                    using var stream = new MemoryStream();
                    await this.fakePlc.GetStream().CopyToAsync(stream, token).ConfigureAwait(false);
                    stream.Seek(0, SeekOrigin.Begin);
                }
            });
        }

        private void FakePlcDown_Click(object sender, RoutedEventArgs e)
        {
            if (this.fakePlc == null)
            {
                this.LogMessage("伪装 PLC 未上线");
                return;
            }

            this.fakePlcCancellationTokenSource.Cancel();
            try
            {
                this.fakePlcBackgroundTask.ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
            }

            this.fakePlc.Close();
            this.fakePlc.Dispose();

            this.fakePlcCancellationTokenSource.Dispose();
            this.fakePlcBackgroundTask.Dispose();

            this.fakePlc = null;
            this.LogMessage("伪装 PLC 已下线");
        }

        private void FakePlcReplyConnect_Click(object sender, RoutedEventArgs e)
        {
            if (this.fakePlc == null)
            {
                this.LogMessage("伪装 PLC 未上线");
                return;
            }

            var frame = PlcFrame.Create(
                PlcMessageType.ConnectResponse,
                ByteString.CopyFrom(this.GetPhysicalAddress().GetAddressBytes()));
            frame.FrameHeader.SequenceNumber = (uint)Interlocked.Increment(ref this.fakePlcSequenceNumber);
            frame.WriteTo(this.fakePlc.GetStream());
        }

        private void FakePlcReplyGetMetric_Click(object sender, RoutedEventArgs e)
        {
            if (this.fakePlc == null)
            {
                this.LogMessage("伪装 PLC 未上线");
                return;
            }

            byte[] bytes = new byte[0x20];
            using var writer = new BinaryWriter(new MemoryStream(bytes));
            writer.Write(60F);
            writer.Write(23F);
            writer.Write(79.9F);
            writer.Write(20F);
            writer.Write(10F);
            writer.Write(4F);
            writer.Write(42F);
            writer.Write(17F);
            var frame = PlcFrame.Create(
                PlcMessageType.GetMetricResponse,
                ByteString.CopyFrom(bytes));
            frame.FrameHeader.SequenceNumber = (uint)Interlocked.Increment(ref this.fakePlcSequenceNumber);
            frame.WriteTo(this.fakePlc.GetStream());
        }

        private PhysicalAddress GetPhysicalAddress()
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }

                return adapter.GetPhysicalAddress();
            }

            return PhysicalAddress.None;
        }

        private void FakePlcReplyUpdateSwitch_Click(object sender, RoutedEventArgs e)
        {
            if (this.fakePlc == null)
            {
                this.LogMessage("伪装 PLC 未上线");
                return;
            }

            byte[] bytes = new byte[0x07];
            using var writer = new BinaryWriter(new MemoryStream(bytes));
            writer.Write((byte)1);
            writer.Write((byte)0);
            writer.Write((byte)1);
            writer.Write((byte)0);
            writer.Write((byte)1);
            writer.Write((byte)0);
            writer.Write((byte)1);

            var frame = PlcFrame.Create(
                PlcMessageType.GetSwitchResponse,
                ByteString.CopyFrom(bytes));
            frame.FrameHeader.SequenceNumber = (uint)Interlocked.Increment(ref this.fakePlcSequenceNumber);
            frame.WriteTo(this.fakePlc.GetStream());
        }

        private async void BackgroundTaskEntryPoint()
        {
            while (!this.cancellationTokenSource.IsCancellationRequested)
            {
                PlcClient client = await this.plcServer.AcceptAsync().ConfigureAwait(false);
                client.OnClosed += (sender, _) =>
                {
                    var c = (PlcClient)sender;
                    lock (this.lockObject)
                    {
                        this.clients.Remove(c);
                    }

                    this.Dispatcher.Invoke(() =>
                    {
                        this.LogMessage(string.Format(
                            CultureInfo.CurrentCulture,
                            "PLC {0} 已断开连接。",
                            c.RemoteEndPoint));
                    });
                };

                lock (this.lockObject)
                {
                    if (this.clients.Count == 0)
                    {
                        client.OnDebugSending += (sender, bytes) =>
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                this.SendingDocument.Blocks.Clear();
                                this.SendingDocument.Blocks.Add(new Paragraph(new Run(HexUtils.Dump(
                                    bytes, bytesPerLine: 4, showHeader: false))));
                            });
                        };
                        client.OnDebugReceiving += (sender, bytes) =>
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                this.ReceivingDocument.Blocks.Clear();
                                this.ReceivingDocument.Blocks.Add(new Paragraph(new Run(HexUtils.Dump(
                                    bytes, bytesPerLine: 4, showHeader: false))));
                            });
                        };

                        this.clients.Add(client);
                    }
                    else
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            this.LogMessage("测试程序只允许一台 PLC 连接。");
                        });
                        client.Close();
                    }
                }

                this.Dispatcher.Invoke(() =>
                {
                    this.LogMessage(string.Format(
                        CultureInfo.CurrentCulture,
                        "PLC {0} 已连接，点击按钮向 PLC 发送消息，鼠标悬停在按钮上有相关说明。",
                        client.RemoteEndPoint));
                });
            }
        }

        private void LogMessage(string message)
        {
            if (this.LogDocument.Blocks.FirstBlock == null)
            {
                this.LogDocument.Blocks.Add(new Paragraph(new Run(message)));
            }
            else
            {
                this.LogDocument.Blocks.InsertBefore(
                    this.LogDocument.Blocks.FirstBlock,
                    new Paragraph(new Run(message)));
            }
        }

        private void LocalDebugToggle_Checked(object sender, RoutedEventArgs e)
        {
            this.LocalDebugGroup.Visibility = Visibility.Visible;
        }

        private void LocalDebugToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            this.LocalDebugGroup.Visibility = Visibility.Collapsed;
        }
    }
}
