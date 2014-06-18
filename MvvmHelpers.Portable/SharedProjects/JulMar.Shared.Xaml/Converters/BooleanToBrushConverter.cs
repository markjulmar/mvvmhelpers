using System;
#if DESKTOP
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
#elif WINRT || WINDOWS_PHONE_APP || WINDOWS_PHONE_APP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
#endif

namespace JulMar.Converters
{
    /// <summary>
    /// This converts a Boolean value to a Brush
    /// </summary>
    public sealed class BooleanToBrushConverter : IValueConverter
    {
        /// <summary>
        /// Mapping for True value
        /// </summary>
        public Brush TrueBrush { get; set; }

        /// <summary>
        /// Mapping for False value
        /// </summary>
        public Brush FalseBrush { get; set; }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
#if DESKTOP
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
#elif WINRT || WINDOWS_PHONE_APP || WINDOWS_PHONE_APP
        public object Convert(object value, Type targetType, object parameter, string language)
#endif
        {
            if (targetType != typeof(Brush))
                throw new ArgumentException("BoolToBrushConverter used inappropriately.");

            bool flag = false;
            if (value is bool)
            {
                flag = (bool)value;
            }

            return flag ? TrueBrush : FalseBrush;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
#if DESKTOP
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
#elif WINRT || WINDOWS_PHONE_APP || WINDOWS_PHONE_APP
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#endif
        {
            throw new NotImplementedException();
        }
    }
}