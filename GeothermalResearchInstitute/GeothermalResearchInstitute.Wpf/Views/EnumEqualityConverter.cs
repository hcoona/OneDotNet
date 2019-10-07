// <copyright file="EnumEqualityConverter.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace GeothermalResearchInstitute.Wpf.Views
{
    public class EnumEqualityConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null || !(parameter is Enum))
            {
                throw new ArgumentException(nameof(parameter) + " must be an enum.", nameof(parameter));
            }

            return parameter.Equals(value);
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
