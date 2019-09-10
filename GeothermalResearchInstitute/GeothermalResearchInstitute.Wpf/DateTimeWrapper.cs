// <copyright file="DateTimeWrapper.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace GeothermalResearchInstitute.Wpf
{
    public class DateTimeWrapper : INotifyPropertyChanged
    {
        public DateTimeWrapper()
        {
            this.timer = new DispatcherTimer(
                TimeSpan.FromSeconds(1),
                DispatcherPriority.DataBind,
                (_, __) =>
                {
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CurrentLocalDateTime)));
                },
                Application.Current.Dispatcher);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string CurrentLocalDateTime => DateTime.Now.ToString();

        private readonly DispatcherTimer timer;
    }
}
