// <copyright file="App.xaml.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GeothermalResearchInstitute.Wpf.FakeClients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static GeothermalResearchInstitute.v1.AuthenticationService;

namespace GeothermalResearchInstitute.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IHost Host { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            // TODO(zhangshuai.ustc): Hook grpc logger.
            // TODO(zhangshuai.ustc): Add wpf-base host lifetime.
            // (https://github.com/aspnet/Hosting/blob/master/samples/GenericHostSample/ServiceBaseLifetime.cs)
            this.Host = new HostBuilder()
                .ConfigureHostConfiguration(builder => builder
                    .AddEnvironmentVariables()
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddCommandLine(e.Args))
                .ConfigureAppConfiguration((context, builder) =>
                {
                    IHostingEnvironment env = context.HostingEnvironment;
                    builder
                        .AddEnvironmentVariables()
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                        .AddCommandLine(e.Args);
                })
                .ConfigureServices((context, builder) =>
                {
                    if (context.HostingEnvironment.IsDevelopment())
                    {
                        // TODO(zhangshuai.ds): Add fake clients.
                        builder.AddSingleton<AuthenticationServiceClient, FakeAuthenticationServiceClient>();
                    }
                    else
                    {
                        // TODO(zhangshuai.ds): Add real clients.
                        builder.AddSingleton<AuthenticationServiceClient>();
                    }

                    builder
                        .AddTransient<MainWindow>()
                        .AddTransient<LoginWindow>();
                })
                .ConfigureLogging((context, builder) =>
                {
                    builder
                        .AddDebug()
                        .AddConfiguration(context.Configuration.GetSection("Logging"));
                })
                .Build();
            this.Host.Start();

            var logger = this.Host.Services.GetRequiredService<ILogger<App>>();
            logger.LogWarning(
                "Current HostEnvironment is {0}",
                this.Host.Services.GetRequiredService<IHostingEnvironment>().EnvironmentName);
            var mainWindow = this.Host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            this.Host.StopAsync().GetAwaiter().GetResult();
            this.Host.Dispose();
            this.Host = null;

            base.OnExit(e);
        }
    }
}
