using System;
using System.Diagnostics;
using Windows.UI.Xaml.Data;

namespace JulMar.Windows.Converters
{
    /// <summary>
    /// This provides a debugging output for a binding converter
    /// </summary>
    public sealed class DebugConverter : IValueConverter
    {
        /// <summary>
        /// Outputs all parameters to the debug console.
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Debug.WriteLine(string.Format("Convert: Value={0}, TargetType={1}, Parameter={2}, Language={3}", 
                                          value, targetType, parameter, language));
            return value;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Debug.WriteLine(string.Format("ConvertBack: Value={0}, TargetType={1}, Parameter={2}, Language={3}",
                                          value, targetType, parameter, language));
            return value;
        }
    }
}