// <copyright file="DeviceAlarmTypeConverter.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using GeothermalResearchInstitute.v2;

namespace GeothermalResearchInstitute.Wpf.Views
{
    public class DeviceAlarmTypeConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (AlarmType)value switch
            {
                AlarmType.ElectricalHeaterBroken => "电加热器故障",
                AlarmType.HeaterOverloadedBroken => "热泵过载故障（热继电器）",
                AlarmType.HighHeaterPressure => "热泵压力高",
                AlarmType.LowFlowRate => "流量低",
                AlarmType.LowHeaterPressure => "热泵压力低",
                AlarmType.NoPower => "电源断相或相序错",
                AlarmType.Unspecified => "未知",
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
