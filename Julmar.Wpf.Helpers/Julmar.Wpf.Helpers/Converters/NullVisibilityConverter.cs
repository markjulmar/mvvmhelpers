using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace JulMar.Windows.Converters
{
    /// <summary>
    /// This converts object presence to visibility.
    /// </summary>
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class NullVisibilityConverter : MarkupExtension, IValueConverter
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
            NullTreatment = Visibility.Hidden;
            NonNullTreatment = Visibility.Visible;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
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
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This returns the converter
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new NullVisibilityConverter {NullTreatment = this.NullTreatment, NonNullTreatment = this.NonNullTreatment};
        }
    }
}