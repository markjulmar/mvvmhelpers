using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace JulMar.Windows.Converters
{
    /// <summary>
    /// This converts an array of objects to a string with a given separator.  The default
    /// separator is a comma, but it can be changed through a property.
    /// </summary>
    [ValueConversion(typeof(Array), typeof(string))]
    public class ArrayToStringConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// Separator to use when joining strings
        /// </summary>
        public string Separator { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ArrayToStringConverter()
        {
            Separator = ",";                
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
            if (value == null || value == DependencyProperty.UnsetValue) 
                return string.Empty;

            var arr = value as string[];
            if (arr == null)
                throw new ArgumentException("ArrayToStringConverter must take an Array.", "value");

            // Drop out any empty strings
            arr = (arr.Where(s => !string.IsNullOrEmpty(s))).ToArray();
            if (arr.Length == 0)
                return string.Empty;

            return string.Join(Separator, arr);
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
            if (value == null)
                return new string[0];
            string sVal = (value.GetType() != typeof (string)) ? value.ToString() : (string) value;
            return sVal.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion

        /// <summary>
        /// Returns the converter
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new ArrayToStringConverter {Separator = this.Separator};
        }
    }
}