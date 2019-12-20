// <copyright file="BannerViewModel.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Globalization;
using GeothermalResearchInstitute.Wpf.Common;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace GeothermalResearchInstitute.Wpf.ViewModels
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Performance", "CA1822", Justification = "ViewModel")]
    public class BannerViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;
        private ViewModelContext viewModelContext;

        public BannerViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));
            this.NavigateBackCommand = new DelegateCommand(this.ExecuteNavigateBackCommand);

            var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += this.DispatcherTimer_Tick;
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Start();
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

        public string Title => this.ViewModelContext?.Title;

        public string CurrentLocalDateTimeString => DateTime.Now.ToString(CultureInfo.CurrentCulture);

        public DelegateCommand NavigateBackCommand { get; }

        private void ExecuteNavigateBackCommand()
        {
            this.regionManager.RequestNavigate(Constants.ContentRegion, this.ViewModelContext.NavigateBackTarget);
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            this.RaisePropertyChanged(nameof(this.CurrentLocalDateTimeString));
        }

        private void ViewModelContext_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(e.PropertyName);
        }
    }
}
