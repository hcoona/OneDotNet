// <copyright file="DeviceMetricBoardView.xaml.cs" company="Shuai Zhang">
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
    public partial class DeviceMetricBoardView : UserControl
    {
        public DeviceMetricBoardView()
        {
            this.InitializeComponent();
            RegionContext.GetObservableContext(this).PropertyChanged += this.RegionContext_PropertyChanged;
        }

        private DeviceMetricBoardViewModel ViewModel => this.DataContext as DeviceMetricBoardViewModel;

        private void RegionContext_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var viewModelContext = (ViewModelContext)context.Value;
            this.ViewModel.ViewModelContext = viewModelContext;
        }

        private void DeviceMetricBoardView_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModelContext viewModelContext = this.ViewModel.ViewModelContext;
            viewModelContext.UserBarVisibility = Visibility.Visible;
            viewModelContext.BannerVisibility = Visibility.Visible;
            viewModelContext.Title = "实时数据";  // TODO: From resource.
        }
    }
}
