// <copyright file="WelcomeViewModel.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using Prism.Mvvm;

namespace PrismLab.ViewModels
{
    public class WelcomeViewModel : BindableBase
    {
        private string message;

        public WelcomeViewModel()
        {
            this.Message = "View A from your Prism Module";
        }

        public string Message
        {
            get { return this.message; }
            set { this.SetProperty(ref this.message, value); }
        }
    }
}
