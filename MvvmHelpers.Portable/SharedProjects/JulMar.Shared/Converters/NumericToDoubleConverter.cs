using System;
#if DESKTOP
using System.Windows;
using System.Windows.Data;
using System.Globalization;
#elif WINRT || WINDOWS_PHONE_APP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#else
using System.Globalization;
using JulMar.Interfaces;
#endif

namespace JulMar.Converters
{
    /// <summary>
    /// This converts any numeric type to a double.  It is primarily used in template bindings where an integer value
    /// is bound to a RangeBase.Value property which requires a double.
    /// </summary>
    public sealed class NumericToDoubleConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
#if WINRT || WINDOWS_PHONE_APP
        public object Convert(object value, Type targetType, object parameter, string language)
#else
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
#endif
        {
            return System.Convert.ToDouble(value);
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
#if WINRT || WINDOWS_PHONE_APP
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#else
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
#endif
        {
            return value;
        }
    }
}