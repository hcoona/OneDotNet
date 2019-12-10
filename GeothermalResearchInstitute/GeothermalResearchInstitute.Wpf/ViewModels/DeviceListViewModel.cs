// <copyright file="DeviceListViewModel.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GeothermalResearchInstitute.v2;
using GeothermalResearchInstitute.Wpf.Common;
using GeothermalResearchInstitute.Wpf.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace GeothermalResearchInstitute.Wpf.ViewModels
{
    public class DeviceListViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;
        private readonly DeviceService.DeviceServiceClient client;
        private ViewModelContext viewModelContext;

        public DeviceListViewModel(IRegionManager regionManager, DeviceService.DeviceServiceClient client)
        {
            this.regionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.ConfirmCommand = new DelegateCommand(this.ExecuteConfirmCommand);
        }

        public ViewModelContext ViewModelContext
        {
            get => this.viewModelContext;
            set
            {
                this.SetProperty(ref this.viewModelContext, value);
                this.viewModelContext.PropertyChanged += (s, e) =>
                {
                    this.RaisePropertyChanged(e.PropertyName);
                };
            }
        }

        public ObservableCollection<Device> Devices { get; } = new ObservableCollection<Device>();

        public Device SelectedDevice
        {
            get => this.ViewModelContext?.SelectedDevice;
            set => this.ViewModelContext.SelectedDevice = value;
        }

        public DelegateCommand ConfirmCommand { get; }

        public async Task LoadDevicesAsync()
        {
            ListDevicesResponse response = await this.client.ListDevicesAsync(
                new ListDevicesRequest(),
                deadline: DateTime.UtcNow.AddMilliseconds(500));
            this.Devices.Clear();
            this.Devices.AddRange(response.Devices);
            this.SelectedDevice = this.Devices.FirstOrDefault();
        }

        private void ExecuteConfirmCommand()
        {
            if (this.SelectedDevice == null)
            {
                MessageBox.Show("必须选择一个设备才能继续");
            }
            else
            {
                this.regionManager.RequestNavigate(Constants.ContentRegion, nameof(NavigationView));
                this.ViewModelContext.NavigateBackTarget = nameof(WelcomeView);
            }
        }
    }
}
