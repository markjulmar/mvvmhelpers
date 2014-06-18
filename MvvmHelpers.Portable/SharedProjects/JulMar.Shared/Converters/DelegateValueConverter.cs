using System;
#if DESKTOP
using System.Windows;
using System.Windows.Data;
using System.Globalization;
#elif WINRT || WINDOWS_PHONE_APP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#else
using System.Globalization;
using JulMar.Interfaces;
#endif

namespace JulMar.Converters
{
	/// <summary>
	/// An IValueConverter instance based on delegates.
	/// </summary>
	public class DelegateValueConverter : IValueConverter
	{
		/// <summary>
		/// Conversion delegate
        /// </summary>
#if WINRT || WINDOWS_PHONE_APP
        public Func<object, Type, object, string, object> Convert { get; set; }
#else
        public Func<object, Type, object, CultureInfo, object> Convert { get; set; }
#endif

		/// <summary>
		/// Convert Back delegate
		/// </summary>
#if WINRT || WINDOWS_PHONE_APP
        public Func<object, Type, object, string, object> ConvertBack { get; set; }
#else
        public Func<object, Type, object, CultureInfo, object> ConvertBack { get; set; }
#endif

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="convertFunc">Convert func.</param>
		/// <param name="convertBackFunc">Convert back func.</param>
        public DelegateValueConverter(
#if WINRT || WINDOWS_PHONE_APP
            Func<object, Type, object, string, object> convertFunc,
            Func<object, Type, object, string, object> convertBackFunc = null
#else
Func<object, Type, object, CultureInfo, object> convertFunc,
            Func<object, Type, object, CultureInfo, object> convertBackFunc = null
#endif
)
		{
            if (convertFunc == null)
                throw new ArgumentNullException("convertFunc");

            this.ConvertBack = convertBackFunc;
            this.Convert = convertFunc;
        }

	    /// <summary>
	    /// Converts a value. 
	    /// </summary>
	    /// <returns>
	    /// A converted value. If the method returns null, the valid null value is used.
	    /// </returns>
	    /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
#if WINRT || WINDOWS_PHONE_APP
        object IValueConverter.Convert(object value, Type targetType, object parameter, string culture)
#else
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
#endif
		{
            return this.Convert(value, targetType, parameter, culture);
		}

	    /// <summary>
	    /// Converts a value. 
	    /// </summary>
	    /// <returns>
	    /// A converted value. If the method returns null, the valid null value is used.
	    /// </returns>
	    /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
#if WINRT || WINDOWS_PHONE_APP
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string culture)
#else
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#endif
        {
            return this.ConvertBack(value, targetType, parameter, culture);
		}
	}
}
