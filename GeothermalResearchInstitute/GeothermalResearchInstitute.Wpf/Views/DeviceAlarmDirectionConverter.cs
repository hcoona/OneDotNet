using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
using GeothermalResearchInstitute.v2;

namespace GeothermalResearchInstitute.Wpf.Views
{
    public class DeviceAlarmDirectionConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (AlarmChangeDirection)value switch
            {
                AlarmChangeDirection.Appearance => "出现",
                AlarmChangeDirection.Disappearance => "消失",
                _ => throw new ArgumentOutOfRangeException(nameof(value)),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
