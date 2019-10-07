// <copyright file="MainWindowViewModel.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using Prism.Mvvm;
using PrismLab.Models;

namespace PrismLab.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string title = "Prism Application";
        private UserIdentity userIdentity;

        public MainWindowViewModel()
        {
            this.userIdentity = new UserIdentity
            {
                Role = "操作员",
                NickName = "开发者1",
            };
        }

        public string Title
        {
            get => this.title;
            set => this.SetProperty(ref this.title, value);
        }

        public UserIdentity UserIdentity
        {
            get => this.userIdentity;
            set => this.SetProperty(ref this.userIdentity, value);
        }
    }
}
