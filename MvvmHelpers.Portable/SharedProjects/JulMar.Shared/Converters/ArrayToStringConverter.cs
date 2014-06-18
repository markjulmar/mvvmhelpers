using System;
using System.Linq;
#if DESKTOP
using System.Windows;
using System.Windows.Data;
using System.Globalization;
#elif WINRT || WINDOWS_PHONE_APP || WINDOWS_PHONE_APP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#else
using System.Globalization;
using JulMar.Interfaces;
#endif

namespace JulMar.Converters
{
    /// <summary>
    /// This converts an array of objects to a string with a given separator.  The default
    /// separator is a comma, but it can be changed through a property.
    /// </summary>
    public sealed class ArrayToStringConverter : IValueConverter
    {
        /// <summary>
        /// Separator to use when joining strings
        /// </summary>
        public string Separator { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ArrayToStringConverter()
        {
            Separator = ",";                
        }

        #region IValueConverter Members

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
#if WINRT || WINDOWS_PHONE_APP || WINDOWS_PHONE_APP
        public object Convert(object value, Type targetType, object parameter, string language)
#else
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
#endif
        {
#if WINRT || WINDOWS_PHONE_APP || DESKTOP || WINDOWS_PHONE_APP
            if (value == null || value == DependencyProperty.UnsetValue) 
                return string.Empty;
#else
            if (value == null) return string.Empty;
#endif

            var arr = value as string[];
            if (arr == null)
                throw new ArgumentException("ArrayToStringConverter must take an Array.", "value");

            // Drop out any empty strings
            arr = (arr.Where(s => !string.IsNullOrEmpty(s))).ToArray();
            return arr.Length == 0 ? string.Empty : string.Join(Separator, arr);
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
#if WINRT || WINDOWS_PHONE_APP || WINDOWS_PHONE_APP
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#else
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
#endif
        {
            if (value == null)
                return new string[0];
            string sVal = (value.GetType() != typeof (string)) ? value.ToString() : (string) value;
            return sVal.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion
    }
}