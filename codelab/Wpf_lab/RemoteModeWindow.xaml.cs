// <copyright file="RemoteModeWindow.xaml.cs" company="Shuai Zhang">
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
    /// RemoteModeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RemoteModeWindow : Window
    {
        public RemoteModeWindow()
        {
            this.InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.LocalSelectedMode = this.RemoteSelectedMode = 2;
        }

        public static readonly DependencyProperty LocalSelectedModeProperty =
            DependencyProperty.Register(nameof(LocalSelectedMode), typeof(int), typeof(Window));

        public static readonly DependencyProperty RemoteSelectedModeProperty =
            DependencyProperty.Register(nameof(RemoteSelectedMode), typeof(int), typeof(Window));

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

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.LocalSelectedMode = this.RemoteSelectedMode;
        }

        private async void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(1000);
            this.RemoteSelectedMode = this.LocalSelectedMode;
        }

        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
