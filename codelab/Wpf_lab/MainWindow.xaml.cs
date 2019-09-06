// <copyright file="MainWindow.xaml.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Windows;

namespace Wpf_lab
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private UserIdentity User
        {
            get { return (UserIdentity)Application.Current.FindResource("User"); }
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow
            {
                Owner = this,
            };

            if (loginWindow.ShowDialog() == true)
            {
                this.User.Username = "刘冰";
                this.User.Role = "管理员";
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            this.User.Reset();
        }

        private void BtnEnter_Click(object sender, RoutedEventArgs e)
        {
            var selectPeerWindow = new SelectPeerWindow
            {
                Owner = this,
            };

            if (selectPeerWindow.ShowDialog() == true)
            {
                var peer = (string)selectPeerWindow.lbPeer.SelectedItem;
                var controlWindow = new ControlWindow { Owner = this };
                controlWindow.ShowDialog();
            }
        }

        private void BtnFeedback_Click(object sender, RoutedEventArgs e)
        {
            new RemoteLogWindow { Owner = this }.ShowDialog();
        }
    }
}
