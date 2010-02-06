using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace JulMar.Windows.Converters
{
    /// <summary>
    /// This converts an integer value to a Visibility type.
    /// </summary>
    [ValueConversion(typeof(int), typeof(Visibility))]
    public class IntegerToVisibilityConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// Mapping for zero to Visibility.  Defaults to Collapsed.
        /// </summary>
        public Visibility ZeroTreatment { get; set; }
        /// <summary>
        /// Mapping for non-zero to Visibility.  Defaults to Visible.
        /// </summary>
        public Visibility NonzeroTreatment { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public IntegerToVisibilityConverter()
        {
            NonzeroTreatment = Visibility.Visible;
            ZeroTreatment = Visibility.Collapsed;
        }

        #region IValueConverter Members

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null || value == DependencyProperty.UnsetValue
                       ? ZeroTreatment
                       : (System.Convert.ToInt32(value, culture) == 0 ? ZeroTreatment : NonzeroTreatment);
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

        #endregion

        /// <summary>
        /// Returns the converter
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new IntegerToVisibilityConverter {ZeroTreatment = this.ZeroTreatment, NonzeroTreatment = this.NonzeroTreatment};
        }
    }
}