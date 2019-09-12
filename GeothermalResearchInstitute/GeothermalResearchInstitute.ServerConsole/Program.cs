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
                    .AddJsonFile("appsettings.json", true)
                    .AddCommandLine(args))
                .ConfigureAppConfiguration((context, builder) =>
                {
                    IHostingEnvironment env = context.HostingEnvironment;
                    builder
                        .AddEnvironmentVariables()
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                        .AddCommandLine(args);
                })
                .ConfigureServices((context, builder) =>
                {
                    if (context.HostingEnvironment.IsDevelopment())
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
                                new ServerPort("0.0.0.0", Port, ServerCredentials.Insecure),
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
