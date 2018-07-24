using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace butterflow_ui
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(Visibility))
            {
                return (bool)value ? Visibility.Visible : Visibility.Hidden;
            }

            throw new InvalidCastException(string.Format("Cannot convert type to {0} from bool.", targetType.Name));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(Visibility))
            {
                return ((Visibility)value == Visibility.Visible) ? true : false;
            }

            throw new InvalidCastException(string.Format("Cannot convert type {0} to boolean.", targetType.Name));
        }
    }
}
