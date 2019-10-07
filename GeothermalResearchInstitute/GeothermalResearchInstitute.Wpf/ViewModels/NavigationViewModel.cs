// <copyright file="NavigationViewModel.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using GeothermalResearchInstitute.Wpf.Common;
using GeothermalResearchInstitute.Wpf.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace GeothermalResearchInstitute.Wpf.ViewModels
{
    public class NavigationViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;
        private ViewModelContext viewModelContext;

        public NavigationViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));
            this.NavigateToDeviceControlView =
                new DelegateCommand(this.ExecuteNavigateToDeviceControlView, this.CanNavigateToDeviceControlView);
            this.NavigateToDeviceWorkingModeView =
                new DelegateCommand(this.ExecuteNavigateToDeviceWorkingModeView, this.CanNavigateToDeviceWorkingModeView);
            this.NavigateToDeviceRunningParameterView =
                new DelegateCommand(this.ExecuteNavigateToDeviceRunningParameterView, this.CanNavigateToDeviceRunningParameterView);
        }

        public ViewModelContext ViewModelContext
        {
            get => this.viewModelContext;
            set
            {
                this.SetProperty(ref this.viewModelContext, value);
                this.RaiseCommandsCanExecuteChanged();
                this.viewModelContext.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(this.ViewModelContext.SelectedDevice))
                    {
                        this.RaiseCommandsCanExecuteChanged();
                    }
                };
            }
        }

        public DelegateCommand NavigateToDeviceControlView { get; }

        public DelegateCommand NavigateToDeviceWorkingModeView { get; }

        public DelegateCommand NavigateToDeviceRunningParameterView { get; }

        private bool IsDeviceConnected =>
            this.ViewModelContext?.SelectedDevice?.Status == v2.DeviceStatus.Healthy
            || this.ViewModelContext?.SelectedDevice?.Status == v2.DeviceStatus.Unhealthy;

        private void RaiseCommandsCanExecuteChanged()
        {
            this.NavigateToDeviceControlView.RaiseCanExecuteChanged();
            this.NavigateToDeviceWorkingModeView.RaiseCanExecuteChanged();
            this.NavigateToDeviceRunningParameterView.RaiseCanExecuteChanged();
        }

        private bool CanNavigateToDeviceControlView() => this.IsDeviceConnected;

        private void ExecuteNavigateToDeviceControlView()
        {
            this.regionManager.RequestNavigate(Constants.ContentRegion, nameof(DeviceControlView));
            this.ViewModelContext.NavigateBackTarget = nameof(NavigationView);
        }

        private bool CanNavigateToDeviceWorkingModeView() => this.IsDeviceConnected;

        private void ExecuteNavigateToDeviceWorkingModeView()
        {
            this.regionManager.RequestNavigate(Constants.ContentRegion, nameof(DeviceWorkingModeView));
            this.ViewModelContext.NavigateBackTarget = nameof(NavigationView);
        }

        private bool CanNavigateToDeviceRunningParameterView() => this.IsDeviceConnected;

        private void ExecuteNavigateToDeviceRunningParameterView()
        {
            this.regionManager.RequestNavigate(Constants.ContentRegion, nameof(DeviceRunningParameterView));
            this.ViewModelContext.NavigateBackTarget = nameof(NavigationView);
        }
    }
}
