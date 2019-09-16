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
using GeothermalResearchInstitute.v1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GeothermalResearchInstitute.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ILogger<MainWindow> logger;
        private readonly IServiceProvider serviceProvider;

        public MainWindow(ILogger<MainWindow> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
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
            var selectPeerWindow = this.serviceProvider.GetService<SelectPeerWindow>();
            selectPeerWindow.Owner = this;

            if (selectPeerWindow.ShowDialog() == true)
            {
                var peer = (Device)selectPeerWindow.lbPeer.SelectedItem;
                var controlWindow = this.serviceProvider.GetService<ControlWindow>();
                controlWindow.Owner = this;
                controlWindow.Peer = peer;
                controlWindow.ShowDialog();
            }
        }

        private void BtnFeedback_Click(object sender, RoutedEventArgs e)
        {
            new RemoteModeWindow { Owner = this }.ShowDialog();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = this.serviceProvider.GetService<LoginWindow>();
            loginWindow.Owner = this;
            loginWindow.ShowDialog();

            // if (loginWindow.ShowDialog() == true)
            // {
            //    this.User.Username = "刘冰";
            //    this.User.Role = "管理员";
            // }
        }
    }
}
