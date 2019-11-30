// <copyright file="DeviceControlView.xaml.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using GeothermalResearchInstitute.v2;
using GeothermalResearchInstitute.Wpf.ViewModels;
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

        private DeviceControlViewModel ViewModel => this.DataContext as DeviceControlViewModel;

        private void RegionContext_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var viewModelContext = (ViewModelContext)context.Value;
            this.ViewModel.ViewModelContext = viewModelContext;
        }

        private void DeviceControlView_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModelContext viewModelContext = this.ViewModel.ViewModelContext;
            viewModelContext.UserBarVisibility = Visibility.Visible;
            viewModelContext.BannerVisibility = Visibility.Visible;
            viewModelContext.Title = "运行控制";  // TODO: From resource.

            Switch switchObj = new Switch()
            {
                HeaterFanOn = false,
                HeaterPowerOn = true,
                HeaterCompressorOn = true,
            };
            this.ViewModel.Switch = switchObj;
            Switch switchInfo = this.ViewModel.Switch;
        }

        //for test
        private void On_Click(object sender, RoutedEventArgs e)
        {
            Switch switchObj = new Switch()
            {
                HeaterFanOn = true,
                HeaterPowerOn = true,
                HeaterCompressorOn = true,
            };
            this.ViewModel.Switch = switchObj;
            Switch switchInfo = this.ViewModel.Switch;
        }

        private void Off_Click(object sender, RoutedEventArgs e)
        {
            Switch switchObj = new Switch()
            {
                HeaterFanOn = false,
                HeaterPowerOn = false,
                HeaterCompressorOn = false,
            };
            this.ViewModel.Switch = switchObj;
            Switch switchInfo = this.ViewModel.Switch;
        }
    }
}
