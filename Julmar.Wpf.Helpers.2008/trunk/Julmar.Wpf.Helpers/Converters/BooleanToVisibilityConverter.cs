using System.Windows.Data;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace JulMar.Windows.Converters
{
    /// <summary>
    /// This converts a Boolean to a Visibility.  It supports mapping the conversions (unlike the default converter in WPF).
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// Mapping for True to Visibility.  Defaults to Visible.
        /// </summary>
        public Visibility TrueTreatment { get; set; }
        /// <summary>
        /// Mapping for False to Visibility.  Defaults to Collapsed.
        /// </summary>
        public Visibility FalseTreatment { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public BooleanToVisibilityConverter()
        {
            TrueTreatment = Visibility.Visible;
            FalseTreatment = Visibility.Collapsed;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = false;
            
            if (value is bool)
            {
                flag = (bool)value;
            }
            else if (value is bool?)
            {
                bool? nullable = (bool?)value;
                flag = nullable.HasValue ? nullable.Value : false;
            }

            return (flag ? TrueTreatment : FalseTreatment);
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((value is Visibility) && (((Visibility)value) == Visibility.Visible));
        }

        /// <summary>
        /// Returns the converter
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new BooleanToVisibilityConverter {FalseTreatment = this.FalseTreatment, TrueTreatment = this.TrueTreatment};
        }
    }
}