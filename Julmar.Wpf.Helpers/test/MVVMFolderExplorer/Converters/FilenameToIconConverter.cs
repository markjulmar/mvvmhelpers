using System;
using System.Windows.Data;
using System.Windows.Media;
using JulMar.Windows.Extensions;
using System.Windows;

namespace MVVMFolderExplorer.Converters
{
    /// <summary>
    /// This converter takes a filename and returns a valid ImageSource icon for it
    /// using the Windows Shell.
    /// </summary>
    [ValueConversion(typeof(string), typeof(ImageSource))]
    public class FilenameToIconConverter : IValueConverter
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
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Don't hit the shell if in the designer.
            if (Designer.InDesignMode)
                return DependencyProperty.UnsetValue;

            // Check input
            if (value == null || value.GetType() != typeof(string))
                return DependencyProperty.UnsetValue;

            try
            {
                return Win32.LoadIcon(value.ToString());
            }
            catch (Exception)
            {
                return null;
            }
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
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
