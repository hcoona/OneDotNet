using System;
using System.Collections.Generic;
using System.Text;
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
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static GeothermalResearchInstitute.v1.DeviceService;

namespace GeothermalResearchInstitute.Wpf
{
    /// <summary>
    /// ParameterSettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RemoteOptionWindow : Window
    {
        public static readonly DependencyProperty ParameterViewProperty =
     DependencyProperty.Register(nameof(ParameterView), typeof(Device), typeof(Window));

        private readonly ILogger<RemoteOptionWindow> logger;
        private readonly DeviceServiceClient deviceServiceClient;

        public Device peer { get; internal set; }

        public Device ParameterView
        {
            get { return (Device)this.GetValue(ParameterViewProperty); }
            set { this.SetValue(ParameterViewProperty, value); }
        }

        public RemoteOptionWindow(ILogger<RemoteOptionWindow> logger, DeviceServiceClient deviceServiceClient)
        {
            this.InitializeComponent();
            this.logger = logger;
            this.deviceServiceClient = deviceServiceClient;
        }

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += this.OnTimerEvent;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Start();

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
                this.ParameterView = await this.deviceServiceClient.GetDeviceAsync(deviceRequest);

            }
            catch (RpcException ex)
            {
                this.logger.LogError("[RemoteOptionWindow] Window_Loaded error={}", ex);

            }

        }

        private async void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            var updateDeviceRequest = new UpdateDeviceRequest()
            {
                Device = new Device()
                {
                    Id = this.peer.Id,
                    DeviceOption = this.ParameterView.DeviceOption,
                },
                UpdateMask = FieldMask.FromString("device_option"),
            };

            var device = await this.deviceServiceClient.UpdateDeviceAsync(updateDeviceRequest);

        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }



    }
}
