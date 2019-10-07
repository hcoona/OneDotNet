// <copyright file="DeviceWorkingModeConverter.cs" company="Shuai Zhang">
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
    public class DeviceWorkingModeConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (DeviceWorkingMode)value switch
            {
                DeviceWorkingMode.TemperatureDetermination => "温度测定",
                DeviceWorkingMode.FixedWarmPower => "稳定热流（恒定热量）",
                DeviceWorkingMode.FixedColdPower => "稳定热流（恒定冷量）",
                DeviceWorkingMode.SummerSituation => "稳定工况（夏季工况）",
                DeviceWorkingMode.WinterSituation => "稳定工况（冬季工况）",
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
