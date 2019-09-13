// <copyright file="Program.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using GeothermalResearchInstitute.v1;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GeothermalResearchInstitute.ServerConsole
{
    internal class Program
    {
        private const int Port = 50051;

        private static void Main(string[] args)
        {
            // TODO(zhangshuai.ustc): Hook grpc logger.
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
                .ConfigureServices((context, builder) =>
                {
                    IHostingEnvironment env = context.HostingEnvironment;
                    IConfiguration config = context.Configuration;

                    if (env.IsDevelopment())
                    {
                        // TODO(zhangshuai.ds): Add fake clients.
                    }
                    else
                    {
                        // TODO(zhangshuai.ds): Add real clients.
                    }

                    builder.AddSingleton<AuthenticationServiceImpl>();
                    builder.AddSingleton<DeviceServiceImpl>();
                    builder.AddSingleton(serviceProvider =>
                    {
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
                .UseConsoleLifetime()
                .Build();

            using (host)
            {
                host.Run();
            }
        }
    }
}
