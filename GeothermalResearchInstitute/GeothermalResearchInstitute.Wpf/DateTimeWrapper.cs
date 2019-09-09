using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentLocalDateTime)));
                },
                Application.Current.Dispatcher);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string CurrentLocalDateTime => DateTime.Now.ToString();

        private readonly DispatcherTimer timer;
    }
}
