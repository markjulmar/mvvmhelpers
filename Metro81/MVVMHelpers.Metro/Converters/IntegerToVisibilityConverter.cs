using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace JulMar.Windows.Converters
{
    /// <summary>
    /// This converts an integer value to a Visibility type.
    /// </summary>
    public sealed class IntegerToVisibilityConverter : IValueConverter
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