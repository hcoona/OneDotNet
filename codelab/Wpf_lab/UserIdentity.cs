// <copyright file="UserIdentity.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;

namespace Wpf_lab
{
    public class UserIdentity : INotifyPropertyChanged
    {
        public UserIdentity()
        {
            this.Reset();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Username
        {
            get { return this.username; }

            set
            {
                this.username = value;
                this.OnPropertyChanged(nameof(this.Username));
            }
        }

        public string Role
        {
            get { return this.role; }

            set
            {
                this.role = value;
                this.OnPropertyChanged(nameof(this.Role));
            }
        }

        public void Reset()
        {
            this.Username = (string)Application.Current.FindResource("AnonymousUserName");
            this.Role = (string)Application.Current.FindResource("AnonymousUserRole");
        }

        private string username;
        private string role;

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
