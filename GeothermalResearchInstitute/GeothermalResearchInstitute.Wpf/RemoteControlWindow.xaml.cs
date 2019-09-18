// <copyright file="RunningControlWindow.xaml.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using GeothermalResearchInstitute.v1;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using ToggleSwitch;
using static GeothermalResearchInstitute.v1.AuthenticationService;
using static GeothermalResearchInstitute.v1.DeviceService;

namespace GeothermalResearchInstitute.Wpf
{
    /// <summary>
    /// RunningControlWindow.xaml 的交互逻辑.
    /// </summary>
    public partial class RemoteControlWindow : Window
    {
        private UserIdentity User => (UserIdentity)Application.Current.FindResource("User");

        public static readonly DependencyProperty deviceViewProperty =
            DependencyProperty.Register(nameof(DeviceView), typeof(Device), typeof(Window));

        public static readonly DependencyProperty deviceViewLocalProperty =
           DependencyProperty.Register(nameof(DeviceViewLocal), typeof(Device), typeof(Window));

        public Device DeviceView
        {
            get { return (Device)this.GetValue(deviceViewProperty); }
            set { this.SetValue(deviceViewProperty, value); }
        }

        public Device DeviceViewLocal
        {
            get { return (Device)this.GetValue(deviceViewLocalProperty); }
            set { this.SetValue(deviceViewLocalProperty, value); }
        }

        public Device peer { get; internal set; }

        private readonly ILogger<LoginWindow> logger;
        private DeviceServiceClient deviceServiceClient;

        public RemoteControlWindow(ILogger<LoginWindow> logger, DeviceServiceClient deviceServiceClient)
        {
            this.InitializeComponent();
            this.logger = logger;
            this.deviceServiceClient = deviceServiceClient;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await this.load();

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += this.OnTimerEvent;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Start();

        }

        private async Task load()
        {
            var deviceRequest = new GetDeviceRequest()
            {
                Id = this.peer.Id,
                View = v1.DeviceView.MetricsAndControl,
            };

            try
            {
                this.DeviceView = await this.deviceServiceClient.GetDeviceAsync(deviceRequest);
                this.DeviceViewLocal = this.DeviceView;
            }
            catch (RpcException ex)
            {
                this.logger.LogError("[RemoteControlWindow] Window_Loaded error={}", ex);

            }
        }

        private async void OnTimerEvent(object sender, EventArgs e)
        {
            this.logger.LogInformation("[RemoteControlWindow] Load_Data was raised");
            var deviceRequest = new GetDeviceRequest()
            {
                Id = this.peer.Id,
                View = v1.DeviceView.MetricsAndControl,
            };

            try
            {
                this.DeviceView = await this.deviceServiceClient.GetDeviceAsync(deviceRequest);
            }
            catch (RpcException ex)
            {
                this.logger.LogError("[RemoteControlWindow] Window_Loaded error={}", ex);

            }
        }

        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private async void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            var updateDeviceRequest = new UpdateDeviceRequest()
            {
                Device = new Device()
                {
                    Id = this.peer.Id,
                    Controls = this.DeviceViewLocal.Controls,
                },
                UpdateMask = FieldMask.FromString("controls"),
            };

            var device = await this.deviceServiceClient.UpdateDeviceAsync(updateDeviceRequest);
        }

    }
}
