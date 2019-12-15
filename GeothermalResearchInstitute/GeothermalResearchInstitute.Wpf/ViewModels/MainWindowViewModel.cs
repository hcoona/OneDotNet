// <copyright file="MainWindowViewModel.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.ComponentModel;
using System.Windows;
using Prism.Mvvm;

namespace GeothermalResearchInstitute.Wpf.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private ViewModelContext viewModelContext;

        public MainWindowViewModel(ViewModelContext viewModelContext)
        {
            this.ViewModelContext = viewModelContext ?? throw new System.ArgumentNullException(nameof(viewModelContext));
            this.ViewModelContext.PropertyChanged += this.ViewModelContext_PropertyChanged;
        }

        public ViewModelContext ViewModelContext
        {
            get => this.viewModelContext;
            set => this.SetProperty(ref this.viewModelContext, value);
        }

        public Visibility UserBarVisibility
        {
            get => this.ViewModelContext.UserBarVisibility;
            set => this.ViewModelContext.UserBarVisibility = value;
        }

        public Visibility BannerVisibility
        {
            get => this.ViewModelContext.BannerVisibility;
            set => this.ViewModelContext.BannerVisibility = value;
        }

        private void ViewModelContext_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(e.PropertyName);
        }
    }
}
