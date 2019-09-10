// <copyright file="MainWindow.xaml.cs" company="Shuai Zhang">
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Extensions.Logging;

namespace GeothermalResearchInstitute.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ILogger<MainWindow> logger;

        public MainWindow(ILogger<MainWindow> logger)
        {
            this.logger = logger;
            this.InitializeComponent();
        }

        private UserIdentity User
        {
            get { return (UserIdentity)Application.Current.FindResource("User"); }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.logger.LogInformation("Hello World!");
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
            new RemoteModeWindow { Owner = this }.ShowDialog();
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
    }
}
