using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace butterflow_ui
{
    /// <summary> An inverse boolean converter. </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBoolConverter : IValueConverter
    {
        /// <summary> Converts a boolean to its inverse. </summary>
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
            if(targetType == typeof(bool))
            {
                return !(bool)value;
            }

            throw new InvalidCastException(string.Format("Cannot convert type {0} to boolean.", targetType.Name));
        }

        /// <summary> Converts an inverse boolean back to its original state. </summary>
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
            if (targetType == typeof(bool))
            {
                return !(bool)value;
            }

            throw new InvalidCastException(string.Format("Cannot convert type {0} to boolean.", targetType.Name));
        }
    }
}
