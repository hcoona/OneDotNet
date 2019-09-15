// <copyright file="SelectPeerWindow.xaml.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GeothermalResearchInstitute.v1;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static GeothermalResearchInstitute.v1.DeviceService;

namespace GeothermalResearchInstitute.Wpf
{
    /// <summary>
    /// SelectPeerWindow.xaml 的交互逻辑.
    /// </summary>
    public partial class SelectPeerWindow : Window
    {
        public SelectPeerWindow(ILogger<LoginWindow> logger, DeviceServiceClient deviceServiceClient)
        {
            this.InitializeComponent();
            this.Logger = logger;
            this.DeviceServiceClient = deviceServiceClient;
        }

        public ILogger<LoginWindow> Logger { get; }

        public DeviceServiceClient DeviceServiceClient { get; }

        public static readonly DependencyProperty PeerNodesProperty =
            DependencyProperty.Register(nameof(PeerNodes), typeof(IList<Device>), typeof(Window));

        public IList<Device> PeerNodes
        {
            get { return (IList<Device>)this.GetValue(PeerNodesProperty); }
            set { this.SetValue(PeerNodesProperty, value); }
        }


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var listDevicesRequest = new ListDevicesRequest() { };
            try
            {
                var response = await this.DeviceServiceClient.ListDevicesAsync(listDevicesRequest);
                this.PeerNodes = response.Devices.ToList();
            } catch (RpcException ex)
            {
                this.Logger.LogError("[SelectPeerWindow]Window_LoadedAsync error={}", ex);
            }

        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {

            this.DialogResult = true;
        }
    }
}
