using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using GeothermalResearchInstitute.v1;

namespace GeothermalResearchInstitute.Wpf
{
    public class FrenquencyToChinese : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((MotorMode)value)
            {
                case MotorMode.VariableFrequency:
                    return "变频";
                case MotorMode.WorkFrequency:
                    return "工频";
                default:
                    break;
            }

            throw new NotImplementedException();

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value.ToString())
            {
                case "变频":
                    return MotorMode.VariableFrequency;
                case "工频":
                    return MotorMode.WorkFrequency;
            }

            throw new NotImplementedException();
        }
    }
}
