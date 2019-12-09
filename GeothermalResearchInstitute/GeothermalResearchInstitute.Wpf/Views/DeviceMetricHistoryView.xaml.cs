// <copyright file="DeviceMetricHistoryView.xaml.cs" company="Shuai Zhang">
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
    public partial class DeviceMetricHistoryView : UserControl
    {
        public DeviceMetricHistoryView()
        {
            this.InitializeComponent();
            RegionContext.GetObservableContext(this).PropertyChanged += this.RegionContext_PropertyChanged;
        }

        private DeviceMetricHistoryViewModel ViewModel => this.DataContext as DeviceMetricHistoryViewModel;

        private void RegionContext_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var viewModelContext = (ViewModelContext)context.Value;
            this.ViewModel.ViewModelContext = viewModelContext;
        }

        private async void DeviceMetricHistoryView_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModelContext viewModelContext = this.ViewModel.ViewModelContext;
            viewModelContext.UserBarVisibility = Visibility.Visible;
            viewModelContext.BannerVisibility = Visibility.Visible;
            viewModelContext.Title = "历史数据";  // TODO: From resource.

            await this.ViewModel.LoadAsync().ConfigureAwait(true);
        }

        private async void Scroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.OriginalSource is ScrollViewer scrollViewer)
            {
                double verticalOffsetPercentage = e.VerticalOffset / scrollViewer.ScrollableHeight;
                if (verticalOffsetPercentage > 0.75)
                {
                    await this.ViewModel.LoadAsync().ConfigureAwait(true);
                }
            }
        }
    }
}
