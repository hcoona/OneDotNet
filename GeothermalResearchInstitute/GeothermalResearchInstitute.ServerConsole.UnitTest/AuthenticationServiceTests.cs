// <copyright file="AuthenticationServiceTests.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GeothermalResearchInstitute.ServerConsole.GrpcServices;
using GeothermalResearchInstitute.ServerConsole.Options;
using GeothermalResearchInstitute.v1;
using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeothermalResearchInstitute.ServerConsole.UnitTest
{
    [TestClass]
    public class AuthenticationServiceTests
    {
        private static IHost Host { get; set; }

        [ClassInitialize]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Required by MSTest Framework.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "样式", "IDE0060:删除未使用的参数", Justification = "Required by MSTest Framework.")]
        public static void Init(TestContext testContext)
        {
            Host = new HostBuilder()
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
                        { "credentials:0:nickname", "测试用户0" },
                        { "credentials:0:username", "user0_username" },
                        { "credentials:0:password", "user0_password" },
                        { "credentials:0:role", "User" },
                        { "credentials:1:nickname", "测试用户1" },
                        { "credentials:1:username", "user1_username" },
                        { "credentials:1:password", "user1_password" },
                        { "credentials:1:role", "Administrator" },
                    });
                })
                .ConfigureLogging((context, builder) =>
                {
                    builder.AddProvider(new MsTestLoggerProvider());
                })
                .ConfigureServices((context, builder) =>
                {
                    IConfiguration config = context.Configuration;

                    // Configuration options.
                    builder.Configure<AuthenticationOptions>(context.Configuration);

                    // Grpc services.
                    builder.AddSingleton(serviceProvider =>
                    {
                        return new GrpcLoggerAdapater.GrpcLoggerAdapter(
                            serviceProvider.GetRequiredService<ILoggerFactory>(),
                            serviceProvider.GetRequiredService<ILogger<Server>>());
                    });
                    builder.AddTransient<AuthenticationServiceImpl>();
                })
                .Build();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            Host.Dispose();
            Host = null;
        }

        [TestMethod]
        public async Task LoginUser0Success()
        {
            var service = Host.Services.GetRequiredService<AuthenticationServiceImpl>();
            var fakeServerCallContext = TestServerCallContext.Create(
                nameof(service.Authenticate),
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

            var response = await service.Authenticate(
                new AuthenticateRequest
                {
                    Username = "user0_username",
                    Password = "user0_password",
                },
                fakeServerCallContext).ConfigureAwait(false);
            Assert.AreEqual(
                new AuthenticateResponse
                {
                    Nickname = "测试用户0",
                    Role = UserRole.User,
                },
                response);
        }

        [TestMethod]
        public async Task LoginUser1Success()
        {
            var service = Host.Services.GetRequiredService<AuthenticationServiceImpl>();
            var fakeServerCallContext = TestServerCallContext.Create(
                nameof(service.Authenticate),
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

            var response = await service.Authenticate(
                new AuthenticateRequest
                {
                    Username = "user1_username",
                    Password = "user1_password",
                },
                fakeServerCallContext).ConfigureAwait(false);
            Assert.AreEqual(
                new AuthenticateResponse
                {
                    Nickname = "测试用户1",
                    Role = UserRole.Administrator,
                },
                response);
        }

        [TestMethod]
        public async Task LoginUser1Failed()
        {
            var service = Host.Services.GetRequiredService<AuthenticationServiceImpl>();
            var fakeServerCallContext = TestServerCallContext.Create(
                nameof(service.Authenticate),
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

            var rpcException = await Assert.ThrowsExceptionAsync<RpcException>(() => service.Authenticate(
                new AuthenticateRequest
                {
                    Username = "user1_username",
                },
                fakeServerCallContext)).ConfigureAwait(false);
            Assert.AreEqual(StatusCode.Unauthenticated, rpcException.StatusCode);
        }

        [TestMethod]
        public async Task LoginFailed()
        {
            var service = Host.Services.GetRequiredService<AuthenticationServiceImpl>();
            var fakeServerCallContext = TestServerCallContext.Create(
                nameof(service.Authenticate),
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

            var rpcException = await Assert.ThrowsExceptionAsync<RpcException>(() => service.Authenticate(
                new AuthenticateRequest
                {
                },
                fakeServerCallContext)).ConfigureAwait(false);
            Assert.AreEqual(StatusCode.Unauthenticated, rpcException.StatusCode);
        }
    }
}
