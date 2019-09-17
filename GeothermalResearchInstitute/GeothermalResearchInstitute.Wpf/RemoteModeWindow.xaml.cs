// <copyright file="RemoteModeWindow.xaml.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using GeothermalResearchInstitute.v1;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
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
            await this.load();

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += this.OnTimerEvent;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Start();
        }

        private async Task load()
        {
            try
            {
                var deviceRequest = new GetDeviceRequest()
                {
                    Id = this.peer.Id,
                    View = v1.DeviceView.MetricsAndControl,
                };
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

        private async void OnTimerEvent(object sender, EventArgs e)
        {
            this.logger.LogInformation("[RemoteModeWindow] Load_Data was raised");
            var deviceRequest = new GetDeviceRequest()
            {
                Id = this.peer.Id,
                View = v1.DeviceView.MetricsAndControl,
            };

            try
            {
                var device = await this.deviceServiceClient.GetDeviceAsync(deviceRequest);
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
            var updateDeviceRequest = new UpdateDeviceRequest()
            {
                Device = new Device()
                {
                    Id = this.peer.Id,
                    WorkingMode = this.TransformBackMode(this.LocalSelectedMode),
                },
                UpdateMask = FieldMask.FromString("working_mode"),
            };

            var device = await this.deviceServiceClient.UpdateDeviceAsync(updateDeviceRequest);
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

        private DeviceWorkingMode TransformBackMode(int mode)
        {
            switch (mode)
            {
                case 0:
                    return DeviceWorkingMode.MeasureTemperature;
                case 1:
                    return DeviceWorkingMode.KeepWarmCapacity;
                case 2:
                    return DeviceWorkingMode.KeepColdCapacity;
                case 3:
                    return DeviceWorkingMode.SummerCondition;
                case 4:
                    return DeviceWorkingMode.WinterCondition;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
