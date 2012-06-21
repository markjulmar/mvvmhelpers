using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace JulMar.Windows.Converters
{
    /// <summary>
    /// Converts an integer value into a boolean true/false
    /// </summary>
    public sealed class IntegerToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Mapping for zero to Visibility.  Defaults to Collapsed.
        /// </summary>
        public bool ZeroTreatment { get; set; }

        /// <summary>
        /// Mapping for non-zero to Visibility.  Defaults to Visible.
        /// </summary>
        public bool NonzeroTreatment { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public IntegerToBooleanConverter()
        {
            NonzeroTreatment = true;
            ZeroTreatment = false;
        }

        #region IValueConverter Members

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value == null || value == DependencyProperty.UnsetValue
                       ? ZeroTreatment
                       : (System.Convert.ToInt32(value) == 0 ? ZeroTreatment : NonzeroTreatment);
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
