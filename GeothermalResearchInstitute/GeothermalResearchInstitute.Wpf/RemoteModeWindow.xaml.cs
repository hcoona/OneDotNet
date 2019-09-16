// <copyright file="RemoteModeWindow.xaml.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GeothermalResearchInstitute.v1;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static GeothermalResearchInstitute.v1.DeviceService;

namespace GeothermalResearchInstitute.Wpf
{
    /// <summary>
    /// RemoteModeWindow.xaml 的交互逻辑.
    /// </summary>
    public partial class RemoteModeWindow : Window
    {
        public RemoteModeWindow(ILogger<RemoteModeWindow> logger, DeviceServiceClient deviceServiceClient)
        {
            this.InitializeComponent();
            this.logger = logger;
            this.deviceServiceClient = deviceServiceClient;
        }

        public static readonly DependencyProperty LocalSelectedModeProperty =
            DependencyProperty.Register(nameof(LocalSelectedMode), typeof(int), typeof(Window));

        public static readonly DependencyProperty RemoteSelectedModeProperty =
            DependencyProperty.Register(nameof(RemoteSelectedMode), typeof(int), typeof(Window));

        private readonly ILogger<RemoteModeWindow> logger;
        private readonly DeviceServiceClient deviceServiceClient;

        public int LocalSelectedMode
        {
            get { return (int)this.GetValue(LocalSelectedModeProperty); }
            set { this.SetValue(LocalSelectedModeProperty, value); }
        }

        public int RemoteSelectedMode
        {
            get { return (int)this.GetValue(RemoteSelectedModeProperty); }
            set { this.SetValue(RemoteSelectedModeProperty, value); }
        }

        public Device peer { get; internal set; }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var deviceRequest = new GetDeviceRequest()
            {
                Id = this.peer.Id,
                View = v1.DeviceView.MetricsAndControl,
            };

            try
            {
                var device = await this.deviceServiceClient.GetDeviceAsync(deviceRequest);
                this.LocalSelectedMode = this.TransformMode(device.WorkingMode);
                this.RemoteSelectedMode = this.TransformMode(device.WorkingMode);
            }
            catch (RpcException ex)
            {
                this.logger.LogError("[ParameterSettingWindow] Window_Loaded error={}", ex);
            }
            catch (NotImplementedException ex)
            {
                // TODO:
            }
        } 

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.LocalSelectedMode = this.RemoteSelectedMode;
        }

        private async void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(600);
            this.RemoteSelectedMode = this.LocalSelectedMode;
        }

        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private int TransformMode(DeviceWorkingMode mode)
        {
            switch (mode)
            {
                case DeviceWorkingMode.MeasureTemperature:
                    return 0;
                case DeviceWorkingMode.KeepWarmCapacity:
                    return 1;
                case DeviceWorkingMode.KeepColdCapacity:
                    return 2;
                case DeviceWorkingMode.SummerCondition:
                    return 3;
                case DeviceWorkingMode.WinterCondition:
                    return 4;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
