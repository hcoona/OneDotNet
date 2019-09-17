// <copyright file="DeviceServiceTests.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GeothermalResearchInstitute.ServerConsole.GrpcServices;
using GeothermalResearchInstitute.ServerConsole.Models;
using GeothermalResearchInstitute.v1;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeothermalResearchInstitute.ServerConsole.UnitTest
{
    [TestClass]
    public class DeviceServiceTests
    {
        private IHost Host { get; set; }

        [TestInitialize]
        public void Init()
        {
            this.Host = new HostBuilder()
                .ConfigureHostConfiguration(builder =>
                {
                    builder.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "Environment", "UnitTest" },
                    });
                })
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "devices:0:id", "10:BF:48:79:B2:A4" },
                        { "devices:0:name", "测试设备0" },
                        { "devices:1:id", "BC:96:80:E6:70:16" },
                        { "devices:1:name", "测试设备1" },
                    });
                })
                .ConfigureLogging((context, builder) =>
                {
                    builder.AddProvider(new MsTestLoggerProvider());
                })
                .ConfigureServices((context, builder) =>
                {
                    IConfiguration config = context.Configuration;

                    builder.AddDbContext<BjdireContext>(options => options.UseInMemoryDatabase("bjdire"));

                    // Configuration options.
                    builder.Configure<DeviceOptions>(context.Configuration);

                    // Grpc services.
                    builder.AddSingleton(serviceProvider =>
                    {
                        return new GrpcLoggerAdapater.GrpcLoggerAdapter(
                            serviceProvider.GetRequiredService<ILoggerFactory>(),
                            serviceProvider.GetRequiredService<ILogger<Server>>());
                    });
                    builder.AddSingleton<DeviceServiceImpl>();
                })
                .Build();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.Host.Dispose();
            this.Host = null;
        }

        [TestMethod]
        public async Task ListDevicesTest()
        {
            var service = this.Host.Services.GetRequiredService<DeviceServiceImpl>();
            var fakeServerCallContext = TestServerCallContext.Create(
                nameof(service.ListDevices),
                null,
                DateTime.UtcNow.AddHours(1),
                new Metadata(),
                CancellationToken.None,
                null,
                null,
                null,
                (metadata) => Task.CompletedTask,
                () => new WriteOptions(),
                (writeOptions) => { });
            var response = await service.ListDevices(new ListDevicesRequest(), fakeServerCallContext).ConfigureAwait(false);

            CollectionAssert.AreEqual(
                new List<Device>
                {
                    new Device
                    {
                        Id = ByteString.CopyFrom(new byte[] { 0x10, 0xBF, 0x48, 0x79, 0xB2, 0xA4 }),
                        Name = "测试设备0",
                    },
                    new Device
                    {
                        Id = ByteString.CopyFrom(new byte[] { 0xBC, 0x96, 0x80, 0xE6, 0x70, 0x16 }),
                        Name = "测试设备1",
                    },
                },
                response.Devices);
        }
    }
}
