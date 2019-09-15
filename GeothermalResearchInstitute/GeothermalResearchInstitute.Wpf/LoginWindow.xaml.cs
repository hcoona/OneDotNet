// <copyright file="LoginWindow.xaml.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using GeothermalResearchInstitute.v1;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static GeothermalResearchInstitute.v1.AuthenticationService;

namespace GeothermalResearchInstitute.Wpf
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑.
    /// </summary>
    public partial class LoginWindow : Window
    {
        public static readonly DependencyProperty ErrorMsgProperty =
            DependencyProperty.Register(nameof(ErrorMsg), typeof(string), typeof(Window));

        public string ErrorMsg
        {
            get { return (string)this.GetValue(ErrorMsgProperty); }
            set { this.SetValue(ErrorMsgProperty, value); }
        }

        private readonly ILogger<LoginWindow> logger;
        private AuthenticationServiceClient authClient;

        public LoginWindow(ILogger<LoginWindow> logger, AuthenticationServiceClient client)
        {
            this.InitializeComponent();
            this.logger = logger;
            this.authClient = client;

            this.ErrorMsg = string.Empty;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.logger.LogInformation("Hello World!");
            this.ErrorMsg = string.Empty;
        }

        private UserIdentity User => (UserIdentity)Application.Current.FindResource("User");

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private async void BtnConfirm_ClickAsync(object sender, RoutedEventArgs e)
        {
            var authRequest = new AuthenticateRequest()
            {
                Username = this.userName.Text,
                Password = this.userPsd.Password,
            };

            try
            {
                var response = await this.authClient.AuthenticateAsync(authRequest);
                this.User.Role = this.TransUserRole(response.Role);
                this.User.Username = response.Nickname;
                this.DialogResult = true;
            }
            catch (RpcException ex)
            {
                this.ErrorMsg = "login error.Please try agarin latter.";
            }
            catch (NotImplementedException ex)
            {
                this.ErrorMsg = "login error.";
            }
            catch (Exception ex)
            {
                this.ErrorMsg = ex.Message;
                this.logger.LogError("[LoginWindow]BtnConfirm_ClickAsync error={}", ex);
            }
        }

        private string TransUserRole(UserRole role)
        {
            switch (role)
            {
                case UserRole.User:
                    return "用户";
                case UserRole.Administrator:
                    return "管理员";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
