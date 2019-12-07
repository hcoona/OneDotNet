// <copyright file="MainWindow.xaml.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using GeothermalResearchInstitute.Plc;
using GeothermalResearchInstitute.v2;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using PlcClientList = System.Collections.Generic.List<GeothermalResearchInstitute.Plc.PlcClient>;

namespace ProtocolLab
{
    public partial class MainWindow : Window, IDisposable
    {
        private const int PORT = 8888;

        private readonly object lockObject = new object();
        private readonly PlcClientList clients = new PlcClientList();
        private readonly ILoggerFactory loggerFactory = LoggerFactory.Create(b => { });
        private bool disposedValue;
        private TcpListener listener;
        private CancellationTokenSource cancellationTokenSource;
        private Task backgroundTask;

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
                    return this.clients.First();
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
            this.listener = TcpListener.Create(PORT);
            this.listener.Start(20);
            this.LogMessage(string.Format(
                CultureInfo.CurrentCulture,
                "正在监听 {0} 端口，等待 PLC 连接，PLC 连接上来后会在这里收到通知。",
                PORT));

            this.cancellationTokenSource = new CancellationTokenSource();
            this.backgroundTask = Task.Run(this.BackgroundTaskEntryPoint, this.cancellationTokenSource.Token);
        }

        private async void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            this.listener.Stop();
            this.cancellationTokenSource.Cancel();
            await this.backgroundTask.ConfigureAwait(true);
        }

        private async void Connect_Click(object sender, RoutedEventArgs e)
        {
            PlcClient c = this.FirstPlcClient;

            this.LogMessage("Sending ConnectRequest");
            TestResponse response = await c.TestAsync(
                new TestRequest
                {
                    A = 42,
                    B = 3.1415926F,
                    C = "Hello World!",
                    D = Timestamp.FromDateTimeOffset(DateTimeOffset.Parse("2019-10-29T21:42:13.00000+8:00", CultureInfo.InvariantCulture)),
                },
                deadline: DateTime.Now.AddSeconds(30))
            .ConfigureAwait(true);
            this.LogMessage("Received ConnectResponse");
        }

        private void GetMetric_Click(object sender, RoutedEventArgs e)
        {
            this.LogMessage("GetMetric");
        }

        private void UpdateSwitch_Click(object sender, RoutedEventArgs e)
        {
            this.LogMessage("UpdateSwitch");
        }

        private async void BackgroundTaskEntryPoint()
        {
            while (!this.cancellationTokenSource.IsCancellationRequested)
            {
                TcpClient client = await this.listener.AcceptTcpClientAsync().ConfigureAwait(false);
                var plcClient = new PlcClient(this.loggerFactory.CreateLogger<PlcClient>(), client);
                await plcClient.StartAsync().ConfigureAwait(false);
                plcClient.ConnectionClosed += (sender, _) =>
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
                    this.clients.Add(plcClient);
                }

                this.Dispatcher.Invoke(() =>
                {
                    this.LogMessage(string.Format(
                        CultureInfo.CurrentCulture,
                        "PLC {0} 已连接，点击按钮向 PLC 发送消息，鼠标悬停在按钮上有相关说明。",
                        client.Client.RemoteEndPoint));
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
    }
}
