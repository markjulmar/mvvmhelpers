namespace JulMar.Windows.Converters
{
    using System;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;

    /// <summary>
    /// This converts object presence to visibility.
    /// </summary>
    [ValueConversion(typeof(string), typeof(Visibility))]
    public class NullOrEmptyVisibilityConverter : MarkupExtension, IValueConverter
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
            EmptyTreatment = Visibility.Hidden;
            NotEmptyTreatment = Visibility.Visible;
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
            return (string.IsNullOrEmpty(value as string)) ? EmptyTreatment : NotEmptyTreatment;
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
            return new NullOrEmptyVisibilityConverter { EmptyTreatment = this.EmptyTreatment, NotEmptyTreatment = this.NotEmptyTreatment };
        }
    }
}
