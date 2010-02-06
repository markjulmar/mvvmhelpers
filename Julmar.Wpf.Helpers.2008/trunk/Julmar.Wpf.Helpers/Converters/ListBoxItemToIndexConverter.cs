using System;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Markup;

namespace JulMar.Windows.Converters
{
    /// <summary>
    /// This converts a ListBoxItem to the index of the item within the underlying databound collection.
    /// </summary>
    [ValueConversion(typeof(ListBoxItem), typeof(int))]
    public class ListBoxItemToIndexConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// StartIndex, defaults to zero
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ListBoxItem item = value as ListBoxItem;
            if (item != null)
            {
                ListBox view = ItemsControl.ItemsControlFromItemContainer(item) as ListBox;
                if (view != null)
                {
                    int index = view.ItemContainerGenerator.IndexFromContainer(item);
                    return index + StartIndex;
                }
            }
            return 0;
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
            return new ListBoxItemToIndexConverter {StartIndex = this.StartIndex};
        }
    }
}