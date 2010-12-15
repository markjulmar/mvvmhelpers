using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace JulMar.Windows.Converters
{
    /// <summary>
    /// This converter can be applied to a binding where the property setter changes the value
    /// being passed in from the binding. WPF does not re-read the value after the setter is applied
    /// unless a converter is applied - in which case the ConvertBack will be used.  This method ensures
    /// changes are propogated back to the UI.
    /// 
    /// See the following blog post for the original report:
    /// http://www.lhotka.net/weblog/DataBindingIssueInWPFWithSolution.aspx
    /// </summary>
    [ValueConversion(typeof(object), typeof(object))]
    public class RefreshValueConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.
        /// </param><param name="targetType">The type of the binding target property.
        /// </param><param name="parameter">The converter parameter to use.
        /// </param><param name="culture">The culture to use in the converter.
        /// </param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.
        /// </param><param name="targetType">The type to convert to.
        /// </param><param name="parameter">The converter parameter to use.
        /// </param><param name="culture">The culture to use in the converter.
        /// </param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        /// <summary>
        /// When implemented in a derived class, returns an object that is set as the value of the target property for this markup extension. 
        /// </summary>
        /// <returns>
        /// The object value to set on the property where the extension is applied. 
        /// </returns>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new RefreshValueConverter();
        }
    }
}

