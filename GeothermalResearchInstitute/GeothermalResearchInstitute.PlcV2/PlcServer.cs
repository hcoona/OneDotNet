// <copyright file="PlcServer.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GeothermalResearchInstitute.PlcV2
{
    public class PlcServer
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly TcpListener tcpListener;

        public PlcServer(ILoggerFactory loggerFactory, IPAddress ipAddress, int port)
        {
            this.loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            this.tcpListener = new TcpListener(ipAddress, port);
        }

        public EndPoint LocalEndPoint => this.tcpListener.LocalEndpoint;

        public void Start()
        {
            this.tcpListener.Start(20);
        }

        public void Stop()
        {
            this.tcpListener.Stop();
        }

        public async Task<PlcClient> AcceptAsync()
        {
            TcpClient tcpClient = await this.tcpListener
                .AcceptTcpClientAsync()
                .ConfigureAwait(false);
            return new PlcClient(this.loggerFactory.CreateLogger<PlcClient>(), tcpClient);
        }
    }
}
