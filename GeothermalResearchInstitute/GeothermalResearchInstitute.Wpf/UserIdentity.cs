using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace GeothermalResearchInstitute.Wpf
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
            get
            {
                return this.username_;
            }

            set
            {
                this.username_ = value;
                this.OnPropertyChanged(nameof(this.Username));
            }
        }

        public string Role
        {
            get
            {
                return role_;
            }

            set
            {
                this.role_ = value;
                this.OnPropertyChanged(nameof(this.Role));
            }
        }

        public void Reset()
        {
            this.Username = (string)Application.Current.FindResource("AnonymousUserName");
            this.Role = (string)Application.Current.FindResource("AnonymousUserRole");
        }

        private string username_;
        private string role_;

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
