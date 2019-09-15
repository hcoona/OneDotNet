// <copyright file="ControlWindow.xaml.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GeothermalResearchInstitute.Wpf
{
    /// <summary>
    /// ControlWindow.xaml 的交互逻辑.
    /// </summary>
    public partial class ControlWindow : Window
    {
        private readonly ILogger<ControlWindow> logger;
        private readonly IServiceProvider serviceProvider;

        public Device Peer { get; internal set; }

        public ControlWindow(ILogger<ControlWindow> logger, IServiceProvider serviceProvider)
        {
            this.InitializeComponent();
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void BtnRemoteControl_Click(object sender, RoutedEventArgs e)
        {
            RunningControlWindow runningControlWindow = this.serviceProvider.GetService<RunningControlWindow>();
            runningControlWindow.peer = this.Peer;
            runningControlWindow.ShowDialog();
        }

        private void BtnRemoteMode_Click(object sender, RoutedEventArgs e)
        {
            RemoteModeWindow remoteModeWindow = this.serviceProvider.GetService<RemoteModeWindow>();
            remoteModeWindow.peer = this.Peer;
            remoteModeWindow.ShowDialog();
        }

        private void BtnParameterSetting_Click(object sender, RoutedEventArgs e)
        {
            ParameterSettingWindow parameterSettingWindow = this.serviceProvider.GetService<ParameterSettingWindow>();
            parameterSettingWindow.peer = this.Peer;
            parameterSettingWindow.ShowDialog();
        }
    }
}
