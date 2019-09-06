// <copyright file="DateTimeWrapper.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Wpf_lab
{
    public class DateTimeWrapper : INotifyPropertyChanged
    {
        public DateTimeWrapper()
        {
            this.Timer = new DispatcherTimer(TimeSpan.FromSeconds(1),
                DispatcherPriority.DataBind,
                (_, __) =>
                {
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CurrentLocalDateTime)));
                },
                Application.Current.Dispatcher);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string CurrentLocalDateTime
        {
            get
            {
                return DateTime.Now.ToString();
            }
        }

        public DispatcherTimer Timer { get; }
    }
}
