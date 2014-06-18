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
    /// Converts an integer value into a boolean true/false
    /// </summary>
    public sealed class IntegerToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Mapping for zero to Visibility.  Defaults to Collapsed.
        /// </summary>
        public bool ZeroTreatment { get; set; }

        /// <summary>
        /// Mapping for non-zero to Visibility.  Defaults to Visible.
        /// </summary>
        public bool NonzeroTreatment { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public IntegerToBooleanConverter()
        {
            NonzeroTreatment = true;
            ZeroTreatment = false;
        }

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
            return value == null 
#if WINRT || WINDOWS_PHONE_APP || DESKTOP
            || value == DependencyProperty.UnsetValue
#endif
                       ? ZeroTreatment
                       : (System.Convert.ToInt32(value) == 0 ? ZeroTreatment : NonzeroTreatment);
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
            throw new NotImplementedException();
        }

        #endregion
    }
}
