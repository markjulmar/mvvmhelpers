using System;
using System.Globalization;

namespace JulMar.Interfaces
{
	/// <summary>
	/// This represents a value converter applied to a code-based binding
	/// </summary>
	public interface IValueConverter
	{
        /// <summary>
        /// Convert a value from one type to another.
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="targetType">Type to convert to </param>
        /// <param name="parameter">Optional parameter</param>
        /// <param name="culture">Optional culture</param>
        /// <returns>Converted value</returns>
		object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        /// <summary>
        /// Convert a value from one type to another (reverse).
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="targetType">Type to convert to </param>
        /// <param name="parameter">Optional parameter</param>
        /// <param name="culture">Optional culture</param>
        /// <returns>Converted value</returns>
        object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
	}

}

