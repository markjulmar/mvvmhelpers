using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Markup;

namespace JulMar.Windows.Converters
{
    /// <summary>
    /// This provides a simple Math converter which supports to add/subtract/multiply/divide by a constant.
    /// The operation can be "+", "-", "*" and "%".
    /// </summary>
    /// <example>
    /// <![CDATA[
    ///     <me:MathConverter x:Key="mathCvt" />
    ///     <TextBlock Text="{Binding ElementName=slider, Path=Value, Converter={StaticResource mathCvt}, ConverterParameter=+1}" />
    /// ]]>
    /// </example>
    public class MathConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// Operation to set - used if no parameter is passed.
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            // Get the operation - we use the parameter if supplied, if not, use the field data set on the resource itself.
            string operation;
            if (parameter == null || string.IsNullOrEmpty(parameter.ToString()))
                operation = Operation;
            else
                operation = parameter.ToString();

            // No parameter? Return the original value.
            if (string.IsNullOrEmpty(operation) || operation.Length < 2)
                return value;

            string opValue = operation.Substring(1);

            try
            {
                double num = Double.Parse(value.ToString(), culture);
                var operandNumber = Double.Parse(opValue, culture);
                switch (operation[0])
                {
                    case '+':
                        return (num + operandNumber).ToString(culture);
                    case '-':
                        return (num - operandNumber).ToString(culture);
                    case '*':
                        return (num * operandNumber).ToString(culture);
                    case '%':
                        return (num % operandNumber).ToString(culture);
                    case '/':
                        return (num/operandNumber).ToString(culture);
                }
            }
            catch
            {
                // Ignore formatting converter errors
            }

            return value;
        }


        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the converter
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new MathConverter {Operation = this.Operation};
        }
    }
}