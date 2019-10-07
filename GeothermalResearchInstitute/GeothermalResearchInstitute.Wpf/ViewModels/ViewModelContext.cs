// <copyright file="ViewModelContext.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Security.Principal;
using System.Windows;
using GeothermalResearchInstitute.v2;
using Prism.Mvvm;

namespace GeothermalResearchInstitute.Wpf.ViewModels
{
    public class ViewModelContext : BindableBase
    {
        private IPrincipal principal;
        private Visibility userBarVisibility;
        private Visibility bannerVisibility;
        private string title;
        private string navigateBackTarget;
        private Device selectedDevice;

        public IPrincipal Principal
        {
            get => this.principal;
            set => this.SetProperty(ref this.principal, value);
        }

        public Visibility UserBarVisibility
        {
            get => this.userBarVisibility;
            set => this.SetProperty(ref this.userBarVisibility, value);
        }

        public Visibility BannerVisibility
        {
            get => this.bannerVisibility;
            set => this.SetProperty(ref this.bannerVisibility, value);
        }

        public string Title
        {
            get => this.title;
            set => this.SetProperty(ref this.title, value);
        }

        public string NavigateBackTarget
        {
            get => this.navigateBackTarget;
            set => this.SetProperty(ref this.navigateBackTarget, value);
        }

        public Device SelectedDevice
        {
            get => this.selectedDevice;
            set => this.SetProperty(ref this.selectedDevice, value);
        }
    }
}
