// <copyright file="RunningControlWindow.xaml.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Linq;
using System.Windows;
using GeothermalResearchInstitute.v1;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static GeothermalResearchInstitute.v1.AuthenticationService;
using static GeothermalResearchInstitute.v1.DeviceService;

namespace GeothermalResearchInstitute.Wpf
{
    /// <summary>
    /// RunningControlWindow.xaml 的交互逻辑.
    /// </summary>
    public partial class RunningControlWindow : Window
    {
        private UserIdentity User => (UserIdentity)Application.Current.FindResource("User");

        public static readonly DependencyProperty deviceViewProperty =
            DependencyProperty.Register(nameof(DeviceView), typeof(Device), typeof(Window));

        public Device DeviceView
        {
            get { return (Device)this.GetValue(deviceViewProperty); }
            set { this.SetValue(deviceViewProperty, value); }
        }

        public Device peer { get; internal set; }

        private readonly ILogger<LoginWindow> logger;
        private DeviceServiceClient deviceServiceClient;

        public RunningControlWindow(ILogger<LoginWindow> logger, DeviceServiceClient deviceServiceClient)
        {
            this.InitializeComponent();
            this.logger = logger;
            this.deviceServiceClient = deviceServiceClient;
        }

        private void HeatPumpFanOn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void HeatPumpCompressorOn_Click(object sender, RoutedEventArgs e)
        { }

        private void HeatPumpFourWayReversingValue_Click(object sender, RoutedEventArgs e)
        { }

        private void HeatPumpPower_Click(object sender, RoutedEventArgs e)
        { }

        private void DevicePower_Click(object sender, RoutedEventArgs e)
        { }

        private void ExhaustPower_Click(object sender, RoutedEventArgs e)
        { }

        private void HeatPumpAuto_Click(object sender, RoutedEventArgs e)
        { }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var deviceRequest = new GetDeviceRequest()
            {
                Id = this.peer.Id,
                View = v1.DeviceView.MetricsAndControl,
            };

            try
            {
                var deviceView = await this.deviceServiceClient.GetDeviceAsync(deviceRequest);

            }
            catch (RpcException ex)
            {
                this.logger.LogError("[ParameterSettingWindow] Window_Loaded error={}", ex);

            }

        }

    }
}
