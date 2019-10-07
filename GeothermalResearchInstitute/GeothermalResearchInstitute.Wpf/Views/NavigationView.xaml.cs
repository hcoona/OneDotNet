// <copyright file="NavigationView.xaml.cs" company="Shuai Zhang">
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
    public partial class NavigationView : UserControl
    {
        public NavigationView()
        {
            this.InitializeComponent();
            RegionContext.GetObservableContext(this).PropertyChanged += this.RegionContext_PropertyChanged;
        }

        private NavigationViewModel ViewModel => this.DataContext as NavigationViewModel;

        private void RegionContext_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var viewModelContext = (ViewModelContext)context.Value;
            this.ViewModel.ViewModelContext = viewModelContext;
        }

        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModelContext viewModelContext = this.ViewModel.ViewModelContext;
            viewModelContext.UserBarVisibility = Visibility.Visible;
            viewModelContext.BannerVisibility = Visibility.Visible;
            viewModelContext.Title = "功能选择";  // TODO: From resource.
            viewModelContext.NavigateBackTarget = nameof(WelcomeView);
        }
    }
}
