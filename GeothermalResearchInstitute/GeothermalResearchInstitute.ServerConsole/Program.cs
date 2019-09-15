// <copyright file="Program.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using GeothermalResearchInstitute.ServerConsole.GrpcService;
using GeothermalResearchInstitute.ServerConsole.Model;
using GeothermalResearchInstitute.v1;
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
            var host = new HostBuilder()
                .ConfigureHostConfiguration(builder => builder
                    .AddEnvironmentVariables()
                    .AddIniFile("appsettings.ini", true)
                    .AddCommandLine(args))
                .ConfigureAppConfiguration((context, builder) =>
                {
                    IHostingEnvironment env = context.HostingEnvironment;
                    builder
                        .AddEnvironmentVariables()
                        .AddIniFile("appsettings.ini", optional: false, reloadOnChange: true)
                        .AddIniFile($"appsettings.{env.EnvironmentName}.ini", optional: true, reloadOnChange: true)
                        .AddCommandLine(args);
                })
                .ConfigureLogging((context, builder) =>
                {
                    IHostingEnvironment env = context.HostingEnvironment;
                    if (env.IsDevelopment())
                    {
                        builder.AddDebug();
                        builder.AddConsole();
                    }

                    builder.AddConfiguration(context.Configuration.GetSection("Logging"));
                })
                .ConfigureServices((context, builder) =>
                {
                    IHostingEnvironment env = context.HostingEnvironment;
                    IConfiguration config = context.Configuration;

                    if (env.IsDevelopment())
                    {
                        // TODO(zhangshuai.ds): Add fake data.
                        builder.AddDbContext<BjdireContext>(options => options.UseInMemoryDatabase("bjdire"));
                    }
                    else
                    {
                        // Database.
                        builder.AddDbContext<BjdireContext>(DbContextOptionsBuilderAction);
                    }

                    // Configuration options.
                    builder.Configure<AuthenticationOptions>(context.Configuration);
                    builder.Configure<DeviceOptions>(context.Configuration);

                    // Grpc services.
                    builder.AddSingleton(serviceProvider =>
                    {
                        return new GrpcLoggerAdapater.GrpcLoggerAdapter(
                            serviceProvider.GetRequiredService<ILoggerFactory>(),
                            serviceProvider.GetRequiredService<ILogger<Server>>());
                    });
                    builder.AddSingleton<AuthenticationServiceImpl>();
                    builder.AddSingleton<DeviceServiceImpl>();
                    builder.AddSingleton(serviceProvider =>
                    {
                        GrpcEnvironment.SetLogger(serviceProvider.GetRequiredService<GrpcLoggerAdapater.GrpcLoggerAdapter>());
                        return new Server
                        {
                            Services =
                            {
                                AuthenticationService.BindService(serviceProvider.GetRequiredService<AuthenticationServiceImpl>()),
                                DeviceService.BindService(serviceProvider.GetRequiredService<DeviceServiceImpl>()),
                            },
                            Ports =
                            {
                                new ServerPort(
                                    "0.0.0.0",
                                    config.GetValue<int>("core.port"),
                                    ServerCredentials.Insecure),
                            },
                        };
                    });

                    builder.AddHostedService<GrpcHostedService>();
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
