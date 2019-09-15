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
using GeothermalResearchInstitute.v1;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static GeothermalResearchInstitute.v1.DeviceService;

namespace GeothermalResearchInstitute.Wpf
{
    /// <summary>
    /// ParameterSettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ParameterSettingWindow : Window
    {
        public static readonly DependencyProperty ParameterViewProperty =
     DependencyProperty.Register(nameof(ParameterView), typeof(Device), typeof(Window));

        private readonly ILogger<ParameterSettingWindow> logger;
        private readonly DeviceServiceClient deviceServiceClient;

        public Device ParameterView
        {
            get { return (Device)this.GetValue(ParameterViewProperty); }
            set { this.SetValue(ParameterViewProperty, value); }
        }

        public ParameterSettingWindow(ILogger<ParameterSettingWindow> logger, DeviceServiceClient deviceServiceClient)
        {
            this.InitializeComponent();
            this.logger = logger;
            this.deviceServiceClient = deviceServiceClient;
        }

        public Device peer { get; internal set; }

        public async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var deviceRequest = new GetDeviceRequest()
            {
                Id = this.peer.Id,
                View = v1.DeviceView.MetricsAndControl,
            };

            try
            {
                Device ParameterView = await this.deviceServiceClient.GetDeviceAsync(deviceRequest);

            }
            catch (RpcException ex)
            {
                this.logger.LogError("[ParameterSettingWindow] Window_Loaded error={}", ex);

            }
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            // TODO:

        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            // TODO:
            this.DialogResult = true;
        }



    }
}
