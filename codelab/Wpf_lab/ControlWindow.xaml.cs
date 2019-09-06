// <copyright file="ControlWindow.xaml.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Wpf_lab
{
    /// <summary>
    /// ControlWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ControlWindow : Window
    {
        public ControlWindow()
        {
            this.InitializeComponent();
        }

        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void BtnRemoteControl_Click(object sender, RoutedEventArgs e)
        {
            RemoteControlWindow remoteControlWindow = new RemoteControlWindow
            {
                Owner = this,
            };
            remoteControlWindow.ShowDialog();
        }

        private void BtnRemoteMode_Click(object sender, RoutedEventArgs e)
        {
            RemoteModeWindow remoteModeWindow = new RemoteModeWindow
            {
                Owner = this,
            };
            remoteModeWindow.ShowDialog();
        }

        private void BtnRemoteOption_Click(object sender, RoutedEventArgs e)
        {
            new RemoteOptionWindow { Owner = this }.ShowDialog();
        }

        private void BtnHistoryData_Click(object sender, RoutedEventArgs e)
        {
            new HistoryDataWindow { Owner = this }.ShowDialog();
        }

        private void BtnCurrentData_Click(object sender, RoutedEventArgs e)
        {
            new CurrentDataWindow { Owner = this }.ShowDialog();
        }

        private void BtnRemoteLog_Click(object sender, RoutedEventArgs e)
        {
            new RemoteLogWindow { Owner = this }.ShowDialog();
        }
    }
}
