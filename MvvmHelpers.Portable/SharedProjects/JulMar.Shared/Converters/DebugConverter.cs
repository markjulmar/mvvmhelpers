#define DEBUG
using System;
using System.Diagnostics;
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
    /// This provides a debugging output for a binding converter
    /// </summary>
    public sealed class DebugConverter : IValueConverter
    {
        /// <summary>
        /// Header to add to string
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Outputs all parameters to the debug console.
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
            Debug.WriteLine(string.Format("{0}{1}Convert: Value={2}, TargetType={3}, Parameter={4}, Language={5}", 
                    Header, string.IsNullOrEmpty(Header) ? "" : " ", value, targetType, parameter, language));
            return value;
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
            Debug.WriteLine(string.Format("{0}{1}ConvertBack: Value={2}, TargetType={3}, Parameter={4}, Language={5}",
                    Header, string.IsNullOrEmpty(Header) ? "" : " ", value, targetType, parameter, language));
            return value;
        }
    }
}