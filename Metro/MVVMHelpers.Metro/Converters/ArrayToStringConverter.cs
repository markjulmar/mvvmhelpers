using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace JulMar.Windows.Converters
{
    /// <summary>
    /// This converts an array of objects to a string with a given separator.  The default
    /// separator is a comma, but it can be changed through a property.
    /// </summary>
    public sealed class ArrayToStringConverter : IValueConverter
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
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || value == DependencyProperty.UnsetValue) 
                return string.Empty;

            var arr = value as string[];
            if (arr == null)
                throw new ArgumentException("ArrayToStringConverter must take an Array.", "value");

            // Drop out any empty strings
            arr = (arr.Where(s => !string.IsNullOrEmpty(s))).ToArray();
            return arr.Length == 0 ? string.Empty : string.Join(Separator, arr);
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return new string[0];
            string sVal = (value.GetType() != typeof (string)) ? value.ToString() : (string) value;
            return sVal.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion
    }
}