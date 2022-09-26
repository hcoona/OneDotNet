// <copyright file="App.xaml.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;
using System.Windows;
using GeothermalResearchInstitute.v2;
using GeothermalResearchInstitute.Wpf.Modules;
using GeothermalResearchInstitute.Wpf.Options;
using GeothermalResearchInstitute.Wpf.ViewModels;
using GeothermalResearchInstitute.Wpf.Views;
using Grpc.Core;
using HCOONa.MicrosoftExtensions.Logging.GrpcAdapater;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using Prism.Unity.Ioc;
using Serilog;
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
                    .SetBasePath(Environment.CurrentDirectory)
                    .AddIniFile("appsettings.ini", optional: false, reloadOnChange: true)
                    .AddCommandLine(e.Args))
                .ConfigureAppConfiguration((context, builder) =>
                    {
                        IHostEnvironment env = context.HostingEnvironment;
                        builder
                            .SetBasePath(Environment.CurrentDirectory)
                            .AddIniFile("appsettings.ini", optional: false, reloadOnChange: true)
                            .AddIniFile($"appsettings.{env.EnvironmentName}.ini", optional: false, reloadOnChange: true)
                            .AddCommandLine(e.Args);
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

                    builder.Configure<CoreOptions>(config.GetSection("core"));

                    builder.AddSingleton(serviceProvider =>
                    {
                        return new GrpcLogger(
                            serviceProvider.GetRequiredService<ILoggerFactory>(),
                            serviceProvider.GetRequiredService<ILogger<ClientBase>>());
                    });

                    if (env.IsDevelopment())
                    {
#if DEBUG
                        builder.AddSingleton<
                            DeviceService.DeviceServiceClient,
                            FakeClients.FakeDeviceServiceClient>();
#endif
                    }
                    else
                    {
                        builder.AddSingleton(provider =>
                        {
                            IOptions<CoreOptions> coreOptions =
                                provider.GetRequiredService<IOptions<CoreOptions>>();
                            return new Channel(
                                coreOptions.Value.ServerGrpcAddress,
                                coreOptions.Value.ServerGrpcPort,
                                ChannelCredentials.Insecure);
                        });
                        builder.AddSingleton(provider =>
                            new DeviceService.DeviceServiceClient(provider.GetRequiredService<Channel>()));
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
