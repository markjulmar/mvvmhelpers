using System;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace JulMar.Windows.Converters
{
    /// <summary>
    /// This converts a Boolean value to a Brush
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Brush))]
    public class BooleanToBrushConverter : MarkupExtension, IValueConverter
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
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(Brush))
                throw new ArgumentException("BoolToBrushConverter used inappropriately.");

            bool flag = false;
            if (value != null && value is bool)
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
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the converter
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new BooleanToBrushConverter {TrueBrush = TrueBrush, FalseBrush = FalseBrush};
        }
    }
}