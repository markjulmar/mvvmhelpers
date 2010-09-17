using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;

namespace JulMar.Windows.Converters
{
    /// <summary>
    /// This converter can be applied to the Validation.Errors to pull the error description
    /// out of any ExceptionValidationRule that has been violated.  Typically this ends up being
    /// a TargetInvocationException and you need to reach into the InnerException to get the actual
    /// error.  In the case where multiple validation rules are applied, this converter will ensure
    /// the proper text is displayed.
    /// </summary>
    /// <example><![CDATA[
    /// <Style TargetType="{x:Type TextBox}">
    ///    <Style.Triggers>
    ///       <Trigger Property="Validation.HasError" Value="true">
    ///          <Setter Property="ToolTip" 
    ///                  Value="{Binding RelativeSource={RelativeSource Self}, Path=(Validation.Errors), Converter={StaticResource errorConverter}}"/>
    ///       </Trigger>
    ///    </Style.Triggers>
    /// </Style>
    /// ]]>
    /// </example>
    public class ValidationErrorConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var errors = value as IList<ValidationError>;
            if (errors == null || errors.Count == 0)
                return string.Empty;

            // Get the first exception
            Exception exception = errors[0].Exception;
            if (exception != null)
            {
                // If it's a TargetInvocationException then a property setter has failed; get the
                // actual failure from the inner exception.
                if (exception is TargetInvocationException)
                    exception = exception.InnerException;
                
                return exception.Message;
            }

            // No exception present -- use the ErrorContent instead.
            return errors[0].ErrorContent;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
