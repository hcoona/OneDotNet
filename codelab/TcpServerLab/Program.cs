// <copyright file="Program.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Globalization;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GeothermalResearchInstitute.Plc;
using GeothermalResearchInstitute.v2;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Overby.Extensions.AsyncBinaryReaderWriter;

namespace TcpServerLab
{
    internal class Program
    {
        private static async Task Main()
        {
            using ILoggerFactory loggerFactory = LoggerFactory.Create(b => b
                .AddConsole(opt =>
                {
                    opt.LogToStandardErrorThreshold = LogLevel.Debug;
                    opt.IncludeScopes = true;
                })
                .SetMinimumLevel(LogLevel.Debug));
            ILogger<Program> logger = loggerFactory.CreateLogger<Program>();

            var listener = TcpListener.Create(8888);
            listener.Start(20);
            logger.LogInformation("TCP Server is listening on port 8888");

#if DEBUG
            var t1 = new Thread(async () => await StartPlcAsync(logger, "127.0.0.1", 8888).ConfigureAwait(false));
            t1.Start();
            var t2 = new Thread(async () => await StartPlcAsync(logger, "127.0.0.1", 8888).ConfigureAwait(false));
            t2.Start();
#endif

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                logger.LogInformation("TCP client connected: {0}", client.Client.RemoteEndPoint);

                using var plcClient = new PlcClient(loggerFactory.CreateLogger<PlcClient>(), client);
                await plcClient.StartAsync().ConfigureAwait(false);

                var request = new TestRequest()
                {
                    A = 42,
                    B = 3.1415926F,
                    C = "Hello World!",
                    D = Timestamp.FromDateTimeOffset(DateTimeOffset.Parse("2019-10-29T21:42:13.00000+8:00", CultureInfo.InvariantCulture)),
                };
                logger.LogInformation("Request sending: {0}", request);

                try
                {
                    TestResponse response = await plcClient.TestAsync(request, deadline: DateTime.Now.AddMilliseconds(3000)).ConfigureAwait(false);
                    logger.LogInformation("Response received: {0}", response);
                }
                catch (RpcException e)
                {
                    logger.LogError(e, "Failed to send/receive test");
                }
            }
        }

        private static async Task<TcpClient> StartPlcAsync(ILogger logger, string hostname, int port)
        {
            var fakePlc = new TcpClient(hostname, port);
            logger.LogInformation("Fake PLC connected.");

            NetworkStream networkStream = fakePlc.GetStream();
            using var reader = new AsyncBinaryReader(networkStream, Encoding.ASCII, true);
            using var writer = new AsyncBinaryWriter(networkStream, Encoding.ASCII, true);

            logger.LogDebug("Receiving TestRequest frame...");
            byte[] receivedBytes = await reader.ReadBytesAsync(0x53).ConfigureAwait(false);
            logger.LogInformation("Received {0} bytes.", 0x53);

            byte[] sendingPayload = new UnifiedFrameContent
            {
                Header = new Header { Status = 0 },
                Payload = new TestResponse
                {
                    A = 42,
                    B = 3.1415926F,
                    C = "Hello World!",
                    D = Timestamp.FromDateTimeOffset(DateTimeOffset.Parse("2019-10-29T21:42:13.00000+8:00", CultureInfo.InvariantCulture)),
                }.ToByteString(),
            }.ToByteArray();
            var sendingHeader = new FrameHeader(
                version: 1,
                type: 2,
                sequenceNumber: 0,
                streamId: 1,
                contentOffset: 20,
                contentLength: (ushort)sendingPayload.Length,
                contentChecksum: Crc32C.Crc32CAlgorithm.Compute(sendingPayload));

            logger.LogDebug("Sending TestResponse frame header...");
            await sendingHeader.WriteTo(writer).ConfigureAwait(false);
            logger.LogDebug("Sending TestResponse frame body...");
            await writer.WriteAsync(sendingPayload).ConfigureAwait(false);
            logger.LogInformation("Sent TestResponse.");

            return fakePlc;
        }
    }
}
