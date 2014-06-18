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
    /// This converts a double into an integer value, rounding the value off.
    /// It is useful when providing textual versions of scrollbar or slider values (on a tooltip).
    /// </summary>
    public sealed class DoubleToIntegerConverter : IValueConverter
    {
        #region IValueConverter Members

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
            return DoConvert(value, targetType);
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
            return DoConvert(value, targetType);
        }

        #endregion

        private static object DoConvert(object value, Type targetType)
        {
            if (value != null)
            {
                return targetType == typeof (int) && value is double
                           ? System.Convert.ToInt32((double) value)
                           : (targetType == typeof (double) && value is int
                                  ? System.Convert.ToDouble((int) value)
                                  : value);
            }

            return null;
        }
    }
}
