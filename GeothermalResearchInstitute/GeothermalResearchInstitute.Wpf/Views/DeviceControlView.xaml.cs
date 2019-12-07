// <copyright file="DeviceControlView.xaml.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GeothermalResearchInstitute.v2;
using GeothermalResearchInstitute.Wpf.ViewModels;
using Google.Protobuf.WellKnownTypes;
using Prism.Common;
using Prism.Regions;

namespace GeothermalResearchInstitute.Wpf.Views
{
    public partial class DeviceControlView : UserControl
    {
        public DeviceControlView()
        {
            this.InitializeComponent();
            RegionContext.GetObservableContext(this).PropertyChanged += this.RegionContext_PropertyChanged;
        }

        public object UpdateMask { get; private set; }

        private DeviceControlViewModel ViewModel => this.DataContext as DeviceControlViewModel;

        private void RegionContext_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var viewModelContext = (ViewModelContext)context.Value;
            this.ViewModel.ViewModelContext = viewModelContext;
        }

        private async void DeviceControlView_LoadedAsync(object sender, RoutedEventArgs e)
        {
            ViewModelContext viewModelContext = this.ViewModel.ViewModelContext;
            viewModelContext.UserBarVisibility = Visibility.Visible;
            viewModelContext.BannerVisibility = Visibility.Visible;
            viewModelContext.Title = "运行控制";  // TODO: From resource.

            await this.ViewModel.LoadMetricAsync().ConfigureAwait(true);
            await this.ViewModel.LoadSwitchAsync().ConfigureAwait(true);
        }

    }
}
