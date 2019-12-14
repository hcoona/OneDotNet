// <copyright file="WelcomeViewModel.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.ComponentModel;
using System.Linq;
using GeothermalResearchInstitute.Wpf.Common;
using GeothermalResearchInstitute.Wpf.Views;
using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace GeothermalResearchInstitute.Wpf.ViewModels
{
    public class WelcomeViewModel : BindableBase
    {
        private readonly ILogger<WelcomeViewModel> logger;
        private readonly IRegionManager regionManager;
        private ViewModelContext viewModelContext;

        public WelcomeViewModel(ILogger<WelcomeViewModel> logger, IRegionManager regionManager)
        {
            this.logger = logger;
            this.regionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));

            this.NavigateToLoginViewCommand = new DelegateCommand(this.ExecuteNavigateToLoginView);
            this.LogoutCommand = new DelegateCommand(this.ExecuteLogout, this.CanExecuteLogout);
            this.NavigateToNavigationViewCommand = new DelegateCommand(
                this.ExecuteNavigateToNavigationView, this.CanExecuteNavigateToNavigationView);
            this.NavigateToContactViewCommand = new DelegateCommand(
                this.ExecuteNavigateToContactView);

            this.logger.LogInformation("Hello World!");
        }

        public ViewModelContext ViewModelContext
        {
            get => this.viewModelContext;
            set
            {
                this.SetProperty(ref this.viewModelContext, value);
                this.viewModelContext.PropertyChanged += this.ViewModelContext_PropertyChanged;
            }
        }

        public DelegateCommand NavigateToLoginViewCommand { get; }

        public DelegateCommand LogoutCommand { get; }

        public DelegateCommand NavigateToNavigationViewCommand { get; }

        public DelegateCommand NavigateToContactViewCommand { get; }

        private bool IsAuthenticated => this.ViewModelContext?.Principal?.Identity.IsAuthenticated == true;

        private void ExecuteNavigateToLoginView()
        {
            this.regionManager.RequestNavigate(Constants.ContentRegion, nameof(LoginView));
            this.ViewModelContext.NavigateBackTarget = nameof(WelcomeView);
        }

        private bool CanExecuteLogout() => this.IsAuthenticated;

        private void ExecuteLogout()
        {
            this.ViewModelContext.Principal = null;
        }

        private bool CanExecuteNavigateToNavigationView() => this.IsAuthenticated;

        private void ExecuteNavigateToNavigationView()
        {
            this.regionManager.RequestNavigate(Constants.ContentRegion, nameof(DeviceListView));
            this.ViewModelContext.NavigateBackTarget = nameof(WelcomeView);
        }

        private void ExecuteNavigateToContactView()
        {
#if DEBUG
            this.ViewModelContext.SelectedDevice = FakeClients.FakeDeviceServiceClient.Devices.Values.First();
            this.regionManager.RequestNavigate(Constants.ContentRegion, nameof(DeviceControlView));
            this.ViewModelContext.NavigateBackTarget = nameof(WelcomeView);
#endif
        }

        private void ViewModelContext_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.ViewModelContext.Principal))
            {
                this.LogoutCommand.RaiseCanExecuteChanged();
                this.NavigateToNavigationViewCommand.RaiseCanExecuteChanged();
            }
        }
    }
}
