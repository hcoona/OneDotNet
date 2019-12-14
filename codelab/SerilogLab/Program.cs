// <copyright file="Program.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace SerilogLab
{
    public static class Program
    {
        internal static void Main(string[] args)
        {
            IHost host = new HostBuilder()
                .ConfigureHostConfiguration(builder => builder
                    .AddCommandLine(args))
                .ConfigureAppConfiguration((context, builder) =>
                {
                    IHostEnvironment env = context.HostingEnvironment;
                    builder
                        .AddEnvironmentVariables()
                        .AddIniFile("appsettings.ini", optional: true, reloadOnChange: true)
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
                    builder.AddHostedService<KeepLoggingHostedService>();
                })
                .UseConsoleLifetime()
                .Build();

            host.Run();
        }
    }
}
