// <copyright file="NavigationViewModel.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;
using GeothermalResearchInstitute.Wpf.Common;
using GeothermalResearchInstitute.Wpf.Views;
using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace GeothermalResearchInstitute.Wpf.ViewModels
{
    public class NavigationViewModel : BindableBase
    {
        private readonly ILogger<NavigationViewModel> logger;
        private readonly IRegionManager regionManager;
        private ViewModelContext viewModelContext;

        public NavigationViewModel(ILogger<NavigationViewModel> logger, IRegionManager regionManager)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.regionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));

            this.NavigateToDeviceControlView =
                new DelegateCommand(this.ExecuteNavigateToDeviceControlView, this.CanNavigateToDeviceControlView);
            this.NavigateToDeviceWorkingModeView =
                new DelegateCommand(this.ExecuteNavigateToDeviceWorkingModeView, this.CanNavigateToDeviceWorkingModeView);
            this.NavigateToDeviceRunningParameterView =
                new DelegateCommand(this.ExecuteNavigateToDeviceRunningParameterView, this.CanNavigateToDeviceRunningParameterView);
            this.NavigateToDeviceMetricHistoryView =
                 new DelegateCommand(this.ExecuteNavigateToDeviceMetricHistoryView);
            this.NavigateToDeviceMetricBoardView =
                new DelegateCommand(this.ExecuteNavigateToDeviceMetricBoardView, this.CanNavigateToDeviceMetricBoardView);
            this.NavigateToDeviceMetricHistoryExportView =
                 new DelegateCommand(this.ExecuteNavigateToDeviceMetricHistoryExportView);
            this.NavigateToDeviceMetricHistoryPlotView =
                 new DelegateCommand(this.ExecuteNavigateToDeviceMetricHistoryPlotView);
            this.NavigateToDeviceAlarmHistoryView = this.NavigateToDeviceAlarmHistoryView =
                 new DelegateCommand(this.ExecuteNavigateToDeviceAlarmHistoryView);
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

        public DelegateCommand NavigateToDeviceMetricHistoryView { get; }

        public DelegateCommand NavigateToDeviceMetricBoardView { get; }

        public DelegateCommand NavigateToDeviceMetricHistoryExportView { get; }

        public DelegateCommand NavigateToDeviceMetricHistoryPlotView { get; }

        public DelegateCommand NavigateToDeviceAlarmHistoryView { get; }

        private bool IsDeviceConnected =>
            this.ViewModelContext?.SelectedDevice?.Status == v2.DeviceStatus.Healthy
            || this.ViewModelContext?.SelectedDevice?.Status == v2.DeviceStatus.Unhealthy;

        private void RaiseCommandsCanExecuteChanged()
        {
            this.NavigateToDeviceControlView.RaiseCanExecuteChanged();
            this.NavigateToDeviceWorkingModeView.RaiseCanExecuteChanged();
            this.NavigateToDeviceRunningParameterView.RaiseCanExecuteChanged();
            this.NavigateToDeviceMetricBoardView.RaiseCanExecuteChanged();
            this.NavigateToDeviceAlarmHistoryView.RaiseCanExecuteChanged();
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

        private void ExecuteNavigateToDeviceMetricHistoryView()
        {
            this.regionManager.RequestNavigate(Constants.ContentRegion, nameof(DeviceMetricHistoryView));
            this.ViewModelContext.NavigateBackTarget = nameof(NavigationView);
        }

        private bool CanNavigateToDeviceMetricBoardView() => this.IsDeviceConnected;

        private void ExecuteNavigateToDeviceMetricBoardView()
        {
            this.regionManager.RequestNavigate(Constants.ContentRegion, nameof(DeviceMetricBoardView));
            this.ViewModelContext.NavigateBackTarget = nameof(NavigationView);
        }

        private void ExecuteNavigateToDeviceMetricHistoryExportView()
        {
            try
            {
                this.regionManager.RequestNavigate(Constants.ContentRegion, nameof(DeviceMetricHistoryExportView));
                this.ViewModelContext.NavigateBackTarget = nameof(NavigationView);
            }
            catch (IOException e)
            {
                this.logger.LogError(e, "Failed to navigate to device metric history export view.");
            }
        }

        private void ExecuteNavigateToDeviceMetricHistoryPlotView()
        {
            try
            {
                this.regionManager.RequestNavigate(Constants.ContentRegion, nameof(DeviceMetricHistoryPlotView));
                this.ViewModelContext.NavigateBackTarget = nameof(NavigationView);
            }
            catch (IOException e)
            {
                this.logger.LogError(e, "Failed to navigate to device metric history plot view.");
            }
        }

        private bool CanNavigateToDeviceAlarmHistoryView() => this.IsDeviceConnected;

        private void ExecuteNavigateToDeviceAlarmHistoryView()
        {
            this.regionManager.RequestNavigate(Constants.ContentRegion, nameof(DeviceAlarmHistoryView));
            this.ViewModelContext.NavigateBackTarget = nameof(NavigationView);
        }
    }
}
