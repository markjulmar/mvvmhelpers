using System.Globalization;
using System.Text.RegularExpressions;

namespace JulMar.UI.Validations
{
    /// <summary>
    /// This implements a validator which utilizes a regular expression.
    /// </summary>
    public sealed class RegexValidatorAttribute : ValidatorAttribute
    {
        /// <summary>
        /// Regular expression to test the parameter against.
        /// </summary>
        public string RegularExpression { get; private set; }

        /// <summary>
        /// The error text for the pattern.
        /// </summary>
        public string ErrorText { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="regExpression">Regular Expression</param>
        public RegexValidatorAttribute(string regExpression)
        {
            this.RegularExpression = regExpression;
        }

        /// <summary>
        /// This is the method called to validate a specific property value.
        /// </summary>
        /// <param name="name">Property name</param>
        /// <param name="value">Value</param>
        /// <returns>Error string, empty if no error</returns>
        public override string Validate(string name, object value)
        {
            Regex rex = new Regex(this.RegularExpression);
            if (value == null || rex.IsMatch(value.ToString()))
                return string.Empty;

            return string.IsNullOrEmpty(this.ErrorText) ? string.Format(CultureInfo.CurrentCulture, "{0} does not match proper input requirements.", name) : this.ErrorText;
        }
    }
}
