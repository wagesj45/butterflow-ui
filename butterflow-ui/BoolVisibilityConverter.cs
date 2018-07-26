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
    /// <summary> A visibility converter. </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolVisibilityConverter : IValueConverter
    {
        /// <summary> Converts a value. </summary>
        /// <exception cref="InvalidCastException"> Thrown when an object cannot be cast to a required
        ///                                         type. </exception>
        /// <param name="value">      The value produced by the binding source. </param>
        /// <param name="targetType"> The type of the binding target property. </param>
        /// <param name="parameter">  The converter parameter to use. </param>
        /// <param name="culture">    The culture to use in the converter. </param>
        /// <returns>
        /// A converted value. If the method returns <see langword="null" />, the valid null value is
        /// used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(Visibility))
            {
                return (bool)value ? Visibility.Visible : Visibility.Hidden;
            }

            throw new InvalidCastException(string.Format("Cannot convert type to {0} from bool.", targetType.Name));
        }

        /// <summary> Converts a value. </summary>
        /// <exception cref="InvalidCastException"> Thrown when an object cannot be cast to a required
        ///                                         type. </exception>
        /// <param name="value">      The value that is produced by the binding target. </param>
        /// <param name="targetType"> The type to convert to. </param>
        /// <param name="parameter">  The converter parameter to use. </param>
        /// <param name="culture">    The culture to use in the converter. </param>
        /// <returns>
        /// A converted value. If the method returns <see langword="null" />, the valid null value is
        /// used.
        /// </returns>
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
