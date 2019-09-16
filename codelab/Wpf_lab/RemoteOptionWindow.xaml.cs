// <copyright file="RemoteOptionWindow.xaml.cs" company="Shuai Zhang">
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
    /// RemoteOptionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RemoteOptionWindow : Window
    {
        public RemoteOptionWindow()
        {
            this.InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
        }

        private async void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(1000);
        }

        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
