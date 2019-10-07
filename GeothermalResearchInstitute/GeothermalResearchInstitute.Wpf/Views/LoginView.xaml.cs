// <copyright file="LoginView.xaml.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using GeothermalResearchInstitute.Wpf.ViewModels;
using Prism.Common;
using Prism.Regions;

namespace GeothermalResearchInstitute.Wpf.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            this.InitializeComponent();
            RegionContext.GetObservableContext(this).PropertyChanged += this.RegionContext_PropertyChanged;
        }

        private void RegionContext_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var viewModelContext = (ViewModelContext)context.Value;
            (this.DataContext as LoginViewModel).ViewModelContext = viewModelContext;
        }

        private void LoginView_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModelContext viewModelContext = (this.DataContext as LoginViewModel).ViewModelContext;
            viewModelContext.UserBarVisibility = Visibility.Collapsed;
            viewModelContext.BannerVisibility = Visibility.Visible;
            viewModelContext.Title = "用户登录";  // TODO: From resource.
        }
    }
}
