using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace JulMar.Windows.Converters
{
    /// <summary>
    /// This converts anything to a string using the ToString method. This is useful if you 
    /// want to do simple formatting but the element is not a String (StringFormat only works with string types)
    /// </summary>
    public sealed class ToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
                return DependencyProperty.UnsetValue;

            if (parameter != null && parameter is string)
            {
                return string.Format((string) parameter, value);
            }
            
            return value.ToString();
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
