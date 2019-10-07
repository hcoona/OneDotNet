// <copyright file="DeviceStatusConverter.cs" company="Shuai Zhang">
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
    public class DeviceStatusConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (DeviceStatus)value switch
            {
                DeviceStatus.Disconnected => "离线",
                DeviceStatus.Healthy => "正常",
                DeviceStatus.Unhealthy => "报警",
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
