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
    /// This converts object presence to visibility.
    /// </summary>
    public sealed class NullVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Visibility conversion when object is null.  Defaults to hidden.
        /// </summary>
        public Visibility NullTreatment { get; set; }
        
        /// <summary>
        /// Visibility conversion when object is non-null.  Defaults to visible.
        /// </summary>
        public Visibility NonNullTreatment { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public NullVisibilityConverter()
        {
            NullTreatment = Visibility.Collapsed;
            NonNullTreatment = Visibility.Visible;
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
            if (targetType != typeof(Visibility))
                throw new ArgumentException("Bad type conversion for NullVisibilityConverter");
            
            return (value == null) ? NullTreatment : NonNullTreatment;
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
            throw new NotImplementedException();
        }
    }
}