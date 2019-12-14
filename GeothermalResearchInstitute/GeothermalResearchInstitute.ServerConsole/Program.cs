// <copyright file="Program.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net;
using GeothermalResearchInstitute.PlcV2;
using GeothermalResearchInstitute.ServerConsole.GrpcServices;
using GeothermalResearchInstitute.ServerConsole.Models;
using GeothermalResearchInstitute.ServerConsole.Options;
using GeothermalResearchInstitute.v2;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace GeothermalResearchInstitute.ServerConsole
{
    internal class Program
    {
        internal static readonly Action<DbContextOptionsBuilder> DbContextOptionsBuilderAction =
            builder => builder.UseSqlite("Data Source=bjdire.sqlite;");

        private static void Main(string[] args)
        {
            IHost host = new HostBuilder()
                .ConfigureHostConfiguration(builder => builder
                    .AddIniFile("appsettings.ini", optional: true, reloadOnChange: false)
                    .AddCommandLine(args))
                .ConfigureAppConfiguration((context, builder) =>
                {
                    IHostEnvironment env = context.HostingEnvironment;
                    builder
                        .AddIniFile("appsettings.ini", optional: false, reloadOnChange: true)
                        .AddIniFile($"appsettings.{env.EnvironmentName}.ini", optional: true, reloadOnChange: true)
                        .AddCommandLine(args);
                })
                .ConfigureLogging((context, builder) =>
                {
                    IHostEnvironment env = context.HostingEnvironment;
                    if (env.IsDevelopment() || env.IsStaging())
                    {
                        builder.AddDebug();
                    }

                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(context.Configuration)
                        .CreateLogger();
                    builder.AddSerilog(dispose: true);

                    builder.AddConfiguration(context.Configuration.GetSection("Logging"));
                })
                .ConfigureServices((context, builder) =>
                {
                    IHostEnvironment env = context.HostingEnvironment;
                    IConfiguration config = context.Configuration;

                    // Configuration options.
                    builder.Configure<AuthenticationOptions>(config);
                    builder.Configure<CoreOptions>(config.GetSection("core"));
                    builder.Configure<DeviceOptions>(config);
                    builder.Configure<TasksOptions>(config.GetSection("tasks"));

                    // Database.
                    if (env.IsDevelopment())
                    {
                        // TODO(zhangshuai.ustc): Add fake data.
                        builder.AddDbContext<BjdireContext>(
                            options => options.UseInMemoryDatabase("bjdire"),
                            ServiceLifetime.Transient,
                            ServiceLifetime.Transient);
                    }
                    else
                    {
                        builder.AddDbContext<BjdireContext>(
                            DbContextOptionsBuilderAction,
                            ServiceLifetime.Transient,
                            ServiceLifetime.Transient);
                    }

                    // PLC server.
                    builder.AddSingleton(provider => new PlcServer(
                        provider.GetRequiredService<ILoggerFactory>(),
                        IPAddress.Any,
                        provider.GetRequiredService<IOptions<CoreOptions>>().Value.TcpPort));
                    builder.AddSingleton<PlcManager>();

                    // gRPC services.
                    builder.AddSingleton(serviceProvider =>
                    {
                        return new GrpcLoggerAdapater.GrpcLoggerAdapter(
                            serviceProvider.GetRequiredService<ILoggerFactory>(),
                            serviceProvider.GetRequiredService<ILogger<Server>>());
                    });
                    builder.AddSingleton<DeviceServiceImpl>();
                    builder.AddSingleton(serviceProvider =>
                    {
                        GrpcEnvironment.SetLogger(serviceProvider.GetRequiredService<GrpcLoggerAdapater.GrpcLoggerAdapter>());
                        return new Server
                        {
                            Services =
                            {
                                DeviceService.BindService(serviceProvider.GetRequiredService<DeviceServiceImpl>()),
                            },
                            Ports =
                            {
                                new ServerPort(
                                    "0.0.0.0",
                                    serviceProvider.GetRequiredService<IOptions<CoreOptions>>().Value.GrpcPort,
                                    ServerCredentials.Insecure),
                            },
                        };
                    });

                    builder.AddHostedService<GrpcHostedService>();
                    builder.AddHostedService<PlcHostedService>();

                    if (env.IsDevelopment())
                    {
                        builder.AddHostedService<FakeDevicesHostedService>();
                    }
                })
                .UseConsoleLifetime()
                .Build();

            using (host)
            {
                host.Run();
            }
        }
    }
}
