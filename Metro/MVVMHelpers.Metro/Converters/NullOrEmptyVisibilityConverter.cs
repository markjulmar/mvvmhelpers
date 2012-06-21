using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace JulMar.Windows.Converters
{
    /// <summary>
    /// This converts object presence to visibility.
    /// </summary>
    public sealed class NullOrEmptyVisibilityConverter : IValueConverter
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
            EmptyTreatment = Visibility.Collapsed;
            NotEmptyTreatment = Visibility.Visible;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
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
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
