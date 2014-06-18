using System;
#if DESKTOP
using System.Windows;
using System.Windows.Data;
using System.Globalization;
#elif WINRT || WINDOWS_PHONE_APP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#endif

namespace JulMar.Converters
{
    /// <summary>
    /// This converts a Boolean to a Visibility.  It supports mapping the conversions.
    /// </summary>
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Mapping for True to Visibility.  Defaults to Visible.
        /// </summary>
        public Visibility TrueTreatment { get; set; }

        /// <summary>
        /// Mapping for False to Visibility.  Defaults to Collapsed.
        /// </summary>
        public Visibility FalseTreatment { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public BooleanToVisibilityConverter()
        {
            TrueTreatment = Visibility.Visible;
            FalseTreatment = Visibility.Collapsed;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
#if DESKTOP
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
#elif WINRT || WINDOWS_PHONE_APP
        public object Convert(object value, Type targetType, object parameter, string language)
#endif
        {
            bool flag = false;
            if (value != null && value is bool)
            {
                flag = (bool)value;
            }

            return (flag ? TrueTreatment : FalseTreatment);
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
#if DESKTOP
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
#elif WINRT || WINDOWS_PHONE_APP
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#endif
        {
            return ((value is Visibility) && (((Visibility)value) == Visibility.Visible));
        }
    }
}