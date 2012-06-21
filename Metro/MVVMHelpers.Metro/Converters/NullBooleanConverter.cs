using System;
using Windows.UI.Xaml.Data;

namespace JulMar.Windows.Converters
{
    /// <summary>
    /// This converts an object value to a boolean
    /// </summary>
    public sealed class NullBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Mapping value for null - defaults to false.
        /// </summary>
        public bool NullTreatment { get; set; }
        
        /// <summary>
        /// Mapping value for non-null, defaults to true.
        /// </summary>
        public bool NonNullTreatment { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public NullBooleanConverter()
        {
            NullTreatment = false;
            NonNullTreatment = true;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(bool))
                throw new ArgumentException("Bad type conversion for NullBooleanConverter");
            
            return (value == null) ? NullTreatment : NonNullTreatment;
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