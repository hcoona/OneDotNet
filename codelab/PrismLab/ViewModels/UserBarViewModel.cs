// <copyright file="UserBarViewModel.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using Prism.Mvvm;
using Prism.Regions;
using PrismLab.Models;

namespace PrismLab.ViewModels
{
    public class UserBarViewModel : BindableBase
    {
        private UserIdentity userIdentity;

        public UserBarViewModel()
        {
        }

        public UserIdentity UserIdentity
        {
            get
            {
                return this.userIdentity;
            }

            set
            {
                this.SetProperty(ref this.userIdentity, value);
                this.RaisePropertyChanged(nameof(this.DisplayName));
                this.RaisePropertyChanged(nameof(this.Role));
            }
        }

        public string DisplayName
        {
            get { return this.UserIdentity.NickName; }
        }

        public string Role
        {
            get { return this.UserIdentity.Role; }
        }
    }
}
