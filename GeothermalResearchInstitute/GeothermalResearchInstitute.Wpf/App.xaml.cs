// <copyright file="App.xaml.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;
using System.Windows;
using GeothermalResearchInstitute.v2;
using GeothermalResearchInstitute.Wpf.FakeClients;
using GeothermalResearchInstitute.Wpf.Modules;
using GeothermalResearchInstitute.Wpf.ViewModels;
using GeothermalResearchInstitute.Wpf.Views;
using Grpc.Core;
using GrpcLoggerAdapater;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using Prism.Unity.Ioc;
using Unity;
using Unity.Microsoft.DependencyInjection;

namespace GeothermalResearchInstitute.Wpf
{
    public partial class App : PrismApplication
    {
        private IUnityContainer UnityContainer { get; set; }

        private IHost Host { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            this.SetupExceptionHandling();

            this.UnityContainer = new UnityContainer();

            this.Host = new HostBuilder()
                .UseServiceProviderFactory<IServiceCollection>(new ServiceProviderFactory(this.UnityContainer))
                .ConfigureHostConfiguration(builder => builder
                    .AddCommandLine(e.Args))
                .ConfigureAppConfiguration((context, builder) =>
                    {
                        IHostEnvironment env = context.HostingEnvironment;
                        builder
                            .SetBasePath(Environment.CurrentDirectory)
                            .AddIniFile("appsettings.ini", optional: false, reloadOnChange: true)
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
                    builder.AddSingleton(serviceProvider =>
                    {
                        return new GrpcLoggerAdapter(
                            serviceProvider.GetRequiredService<ILoggerFactory>(),
                            serviceProvider.GetRequiredService<ILogger<ClientBase>>());
                    });
                    if (context.HostingEnvironment.IsDevelopment())
                    {
                        builder.AddSingleton<DeviceService.DeviceServiceClient, FakeDeviceServiceClient>();
                    }
                    else
                    {
                        string hostname = context.Configuration.GetValue<string>("core:server.hostname");
                        int port = context.Configuration.GetValue<int>("core:server.port");

                        var channel = new Channel(hostname, port, ChannelCredentials.Insecure);
                        var deviceServiceClient = new DeviceService.DeviceServiceClient(channel);

                        builder.AddSingleton(channel);
                        builder.AddSingleton(deviceServiceClient);
                    }
                })
                .Build();

            base.OnStartup(e);
        }

        protected override Window CreateShell()
        {
            return this.Container.Resolve<MainWindow>();
        }

        protected override IContainerExtension CreateContainerExtension()
        {
            return new UnityContainerExtension(this.Host.Services.GetRequiredService<IUnityContainer>());
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance(new ViewModelContext
            {
                Principal = null,
                UserBarVisibility = Visibility.Visible,
                BannerVisibility = Visibility.Collapsed,
                NavigateBackTarget = null,
            });
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);
            moduleCatalog
                .AddModule<BannerModule>()
                .AddModule<UserBarModule>()
                .AddModule<WelcomeModule>()
                .AddModule<LoginModule>()
                .AddModule<DeviceListModule>()
                .AddModule<NavigationModule>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            this.Host.Dispose();

            base.OnExit(e);
        }

        private static void MessageBoxShowException(Exception e, string caption)
        {
            MessageBox.Show(e.ToString(), caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void SetupExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                MessageBoxShowException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            this.Dispatcher.UnhandledException += (s, e) =>
                MessageBoxShowException(e.Exception, "Application.Current.DispatcherUnhandledException");

            TaskScheduler.UnobservedTaskException += (s, e) =>
                MessageBoxShowException(e.Exception, "TaskScheduler.UnobservedTaskException");
        }
    }
}
