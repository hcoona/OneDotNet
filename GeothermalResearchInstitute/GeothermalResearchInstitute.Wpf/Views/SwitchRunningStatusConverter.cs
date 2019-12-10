// <copyright file="SwitchRunningStatusConverter.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace GeothermalResearchInstitute.Wpf.Views
{
    public class SwitchRunningStatusConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return float.Parse((string)value, CultureInfo.InvariantCulture) > 0 ? "true" : "false";
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
