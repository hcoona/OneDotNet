// <copyright file="PercentageConverter.cs" company="Shuai Zhang">
// Copyright Shuai Zhang. All rights reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Data;

namespace GeothermalResearchInstitute.Wpf
{

    public class KGToBooleanConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value == true)
                {
                    return "开";
                }
                else
                {
                    return "关";
                }
            }

            switch (value.ToString())
            {
                case "开":
                    return true;
                case "关":
                    return false;
                default:
                    break;
            }

            throw new NotImplementedException();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value == true)
                {
                    return "开";
                }
                else
                {
                    return "关";
                }
            }

            switch (value.ToString())
            {
                case "开":
                    return true;
                case "关":
                    return false;
                default:
                    break;
            }

            throw new NotImplementedException();
        }
    }
}
