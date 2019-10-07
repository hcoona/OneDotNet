// <copyright file="App.xaml.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using PrismLab.Modules;
using PrismLab.Views;
using Unity;
using Unity.Microsoft.DependencyInjection;

namespace PrismLab
{
    public partial class App : PrismApplication
    {
        public IHost Host { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var container = this.Container.Resolve<IUnityContainer>();

            this.Host = new HostBuilder()
                .UseServiceProviderFactory<IServiceCollection>(new ServiceProviderFactory(container))
                .ConfigureHostConfiguration(builder => builder
                    .AddIniFile("appsettings.ini", optional: true)
                    .AddCommandLine(e.Args)).ConfigureAppConfiguration((context, builder) =>
                    {
                        IHostEnvironment env = context.HostingEnvironment;
                        builder
                            .AddIniFile("appsettings.ini", optional: true, reloadOnChange: true)
                            .AddIniFile($"appsettings.{env.EnvironmentName}.ini", optional: true, reloadOnChange: true)
                            .AddCommandLine(e.Args);
                    })
                .ConfigureLogging((context, builder) =>
                {
                    builder
                        .AddDebug()
                        .AddConfiguration(context.Configuration.GetSection("Logging"));
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

                    builder
                        .AddTransient<MainWindow>();
                })
                .Build();
        }

        protected override Window CreateShell()
        {
            return this.Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);
            moduleCatalog.AddModule<WelcomeModule>().AddModule<UserBarModule>();
        }
    }
}
