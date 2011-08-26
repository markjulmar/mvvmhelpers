using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace JulMar.Windows.Converters
{
    /// <summary>
    /// This converts a double into an integer value, rounding the value off.
    /// It is useful when providing textual versions of scrollbar or slider values (on a tooltip).
    /// </summary>
    [ValueConversion(typeof(double), typeof(int))]
    public class DoubleToIntegerConverter : MarkupExtension, IValueConverter
    {
        #region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DoConvert(value, targetType);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DoConvert(value, targetType);
        }

        #endregion

        private static object DoConvert(object value, Type targetType)
        {
            if (value != null)
            {
                if (targetType == typeof(int) && value is double)
                    return Convert.ToInt32((double) value);
                if (targetType == typeof(double) && value is int)
                    return Convert.ToDouble((int)value);
            }

            return value;
        }

        /// <summary>
        /// When implemented in a derived class, returns an object that is set as the value of the target property for this markup extension. 
        /// </summary>
        /// <returns>
        /// The object value to set on the property where the extension is applied. 
        /// </returns>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.
        ///                 </param>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new DoubleToIntegerConverter();
        }
    }
}
