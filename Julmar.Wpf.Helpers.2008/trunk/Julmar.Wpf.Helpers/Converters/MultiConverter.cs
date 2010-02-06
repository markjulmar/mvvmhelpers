using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace JulMar.Windows.Converters
{
    /// <summary>
    /// Converter collection
    /// </summary>
    public class ValueConverterCollection : Collection<IValueConverter> {/* */}

    /// <summary>
    /// This class holds a list of converters and runs the value through each.
    /// </summary>
    [ContentProperty("Converters")]
    public class MultiConverter : IValueConverter
    {
        /// <summary>
        /// The collection of value converters to run
        /// </summary>
        public ValueConverterCollection Converters { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MultiConverter()
        {
            Converters = new ValueConverterCollection();
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
            // No converters? Return raw value
            if (Converters == null || Converters.Count == 0)
                return value;

            return Converters.Aggregate(value, (current, converter) => converter.Convert(current, targetType, parameter, culture));
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
            // No converters? Return raw value
            if (Converters == null || Converters.Count == 0)
                return value;

            object newValue = value;
            foreach (var converter in Converters)
            {
                try
                {
                    object testValue = converter.ConvertBack(newValue, targetType, parameter, culture);
                    newValue = testValue;
                }
                catch (NotImplementedException)
                {
                }
            }
            return newValue;
        }
    }
}
