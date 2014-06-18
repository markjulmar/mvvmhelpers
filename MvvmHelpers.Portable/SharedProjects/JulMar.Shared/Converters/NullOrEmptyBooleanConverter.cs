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
    /// This converts an object value to a boolean
    /// </summary>
    public sealed class NullOrEmptyBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Mapping value for null - defaults to false.
        /// </summary>
        public bool EmptyTreatment { get; set; }
        
        /// <summary>
        /// Mapping value for non-null, defaults to true.
        /// </summary>
        public bool NotEmptyTreatment { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public NullOrEmptyBooleanConverter()
        {
            EmptyTreatment = false;
            NotEmptyTreatment = true;
        }

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
            if (targetType != typeof(bool))
                throw new ArgumentException("Bad type conversion for NullBooleanConverter");
            return (string.IsNullOrEmpty(value as string)) ? EmptyTreatment : NotEmptyTreatment;
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
    }
}