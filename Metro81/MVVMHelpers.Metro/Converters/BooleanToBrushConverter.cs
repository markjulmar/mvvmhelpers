using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace JulMar.Windows.Converters
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
        public object Convert(object value, Type targetType, object parameter, string language)
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
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}