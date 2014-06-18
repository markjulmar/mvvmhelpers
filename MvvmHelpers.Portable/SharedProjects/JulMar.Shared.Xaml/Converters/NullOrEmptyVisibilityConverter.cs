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
    /// This converts object presence to visibility.
    /// </summary>
    public sealed class NullOrEmptyVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Visibility conversion when object is null.  Defaults to hidden.
        /// </summary>
        public Visibility EmptyTreatment { get; set; }
        
        /// <summary>
        /// Visibility conversion when object is non-null.  Defaults to visible.
        /// </summary>
        public Visibility NotEmptyTreatment { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public NullOrEmptyVisibilityConverter()
        {
            EmptyTreatment = Visibility.Collapsed;
            NotEmptyTreatment = Visibility.Visible;
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
            if (targetType != typeof(Visibility))
                throw new ArgumentException("Bad type conversion for NullVisibilityConverter");
            
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
