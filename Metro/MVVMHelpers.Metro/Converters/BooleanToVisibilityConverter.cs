using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace JulMar.Windows.Converters
{
    /// <summary>
    /// This converts a Boolean to a Visibility.  It supports mapping the conversions.
    /// </summary>
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Mapping for True to Visibility.  Defaults to Visible.
        /// </summary>
        public Visibility TrueTreatment { get; set; }

        /// <summary>
        /// Mapping for False to Visibility.  Defaults to Collapsed.
        /// </summary>
        public Visibility FalseTreatment { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public BooleanToVisibilityConverter()
        {
            TrueTreatment = Visibility.Visible;
            FalseTreatment = Visibility.Collapsed;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool flag = false;
            if (value != null && value is bool)
            {
                flag = (bool)value;
            }

            return (flag ? TrueTreatment : FalseTreatment);
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return ((value is Visibility) && (((Visibility)value) == Visibility.Visible));
        }
    }
}