// <copyright file="UserBarViewModel.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Windows;
using GeothermalResearchInstitute.Wpf.Common;
using Prism.Mvvm;

namespace GeothermalResearchInstitute.Wpf.ViewModels
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Performance", "CA1822", Justification = "ViewModel")]
    public class UserBarViewModel : BindableBase
    {
        private ViewModelContext viewModelContext;

        public ViewModelContext ViewModelContext
        {
            get => this.viewModelContext;
            set
            {
                this.SetProperty(ref this.viewModelContext, value);
                this.viewModelContext.PropertyChanged += this.ViewModelContext_PropertyChanged;
            }
        }

        public string DisplayName
        {
            get
            {
                string name = this.ViewModelContext?.Principal?.Identity.Name;
                if (string.IsNullOrEmpty(name))
                {
                    return (string)Application.Current.FindResource("AnonymousUserName");
                }
                else
                {
                    return name;
                }
            }
        }

        public string Role
        {
            get
            {
                if (this.ViewModelContext?.Principal?.IsInRole(Constants.UserRoleOperator) == true)
                {
                    return (string)Application.Current.FindResource("OperatorUserRole");
                }
                else if (this.ViewModelContext?.Principal?.IsInRole(Constants.UserRoleAdministrator) == true)
                {
                    return (string)Application.Current.FindResource("AdministratorUserRole");
                }
                else
                {
                    return (string)Application.Current.FindResource("AnonymousUserRole");
                }
            }
        }

        public string DeviceName => this.ViewModelContext?.SelectedDevice?.Name ?? "无";

        private void ViewModelContext_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.ViewModelContext.Principal))
            {
                this.RaisePropertyChanged(nameof(this.DisplayName));
                this.RaisePropertyChanged(nameof(this.Role));
            }
            else if (e.PropertyName == nameof(this.ViewModelContext.SelectedDevice))
            {
                this.RaisePropertyChanged(nameof(this.DeviceName));
            }
        }
    }
}
