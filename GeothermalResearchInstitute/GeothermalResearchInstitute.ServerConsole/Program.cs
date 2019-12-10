// <copyright file="Program.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net;
using GeothermalResearchInstitute.PlcV2;
using GeothermalResearchInstitute.ServerConsole.GrpcServices;
using GeothermalResearchInstitute.ServerConsole.Models;
using GeothermalResearchInstitute.v2;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GeothermalResearchInstitute.ServerConsole
{
    internal class Program
    {
        internal static readonly Action<DbContextOptionsBuilder> DbContextOptionsBuilderAction =
            builder => builder.UseSqlite("Data Source=bjdire.sqlite");

        private static void Main(string[] args)
        {
            IHost host = new HostBuilder()
                .ConfigureHostConfiguration(builder => builder
                    .AddIniFile("appsettings.ini", true)
                    .AddCommandLine(args))
                .ConfigureAppConfiguration((context, builder) =>
                {
                    IHostEnvironment env = context.HostingEnvironment;
                    builder
                        .AddEnvironmentVariables()
                        .AddIniFile("appsettings.ini", optional: false, reloadOnChange: true)
                        .AddIniFile($"appsettings.{env.EnvironmentName}.ini", optional: true, reloadOnChange: true)
                        .AddCommandLine(args);
                })
                .ConfigureLogging((context, builder) =>
                {
                    IHostEnvironment env = context.HostingEnvironment;
                    if (env.IsDevelopment())
                    {
                        builder.AddDebug();
                        builder.AddConsole();
                    }

                    builder.AddConfiguration(context.Configuration.GetSection("Logging"));
                })
                .ConfigureServices((context, builder) =>
                {
                    IHostEnvironment env = context.HostingEnvironment;
                    IConfiguration config = context.Configuration;

                    // Database.
                    if (env.IsDevelopment())
                    {
                        // TODO(zhangshuai.ds): Add fake data.
                        builder.AddDbContext<BjdireContext>(options => options.UseInMemoryDatabase("bjdire"));
                    }
                    else
                    {
                        builder.AddDbContext<BjdireContext>(DbContextOptionsBuilderAction);
                    }

                    // Configuration options.
                    builder.Configure<AuthenticationOptions>(context.Configuration);
                    builder.Configure<DeviceOptions>(context.Configuration);

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
                                    config.GetValue<int>("core:grpc_port"),
                                    ServerCredentials.Insecure),
                            },
                        };
                    });

                    // PLC server.
                    builder.AddSingleton(provider => new PlcServer(
                        provider.GetRequiredService<ILoggerFactory>(),
                        IPAddress.Any,
                        config.GetValue<int>("core:plc_port")));
                    builder.AddSingleton<PlcHostedService>();

                    builder.AddHostedService<GrpcHostedService>();
                    builder.AddHostedService<PlcHostedService>();
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
