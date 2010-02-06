using System;
using System.Windows;
using System.Windows.Data;

namespace JulMar.Windows.Converters
{
    /// <summary>
    /// This class can be used to convert a resource key to a resource within a binding expression:
    /// <ContentControl ContentTemplate="{Binding ActiveResourceKey, Converter=KeyToResourceConverter}" />
    /// </summary>
    public class KeyToResourceConverter : Freezable, IValueConverter
    {
        #region Element Property
        /// <summary>
        /// Dependency Property backing the element - used for Data Binding
        /// </summary>
        public static readonly DependencyProperty ElementProperty = DependencyProperty.Register("Element", typeof(FrameworkElement), typeof(KeyToResourceConverter), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// FrameworkElement to use for locating resources
        /// </summary>
        public FrameworkElement Element
        {
            get { return (FrameworkElement)GetValue(ElementProperty); }
            set { SetValue(ElementProperty, value); }
        }
        #endregion

        #region IValueConverter
        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value != null && Element != null)
                ? Element.TryFindResource(value)
                : null;
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
        /// Used to create an instance of the Freezable.
        /// </summary>
        /// <returns></returns>
        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
    }
}
