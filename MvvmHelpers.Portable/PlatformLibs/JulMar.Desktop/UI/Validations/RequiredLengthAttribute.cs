using System.Globalization;

namespace JulMar.UI.Validations
{
    /// <summary>
    /// This validates that a property has a specific length
    /// </summary>
    public sealed class RequiredLengthAttribute : ValidatorAttribute
    {
        /// <summary>
        /// Minimum length required
        /// </summary>
        public int Minimum { get; set; }
        /// <summary>
        /// Maximum length required
        /// </summary>
        public int Maximum { get; set; }

        /// <summary>
        /// This is the method called to validate a specific property value.
        /// </summary>
        /// <param name="name">Property name</param>
        /// <param name="value">Value</param>
        /// <returns>Error string, empty if no error</returns>
        public override string Validate(string name, object value)
        {
            if (value == null)
                return null;

            int len = value.ToString().Length;
            return (len >= this.Minimum && len <= this.Maximum)
                       ? null
                       : string.Format(CultureInfo.CurrentCulture, "Length of {0} must be between {1} and {2}.", name, this.Minimum, this.Maximum);
        }
    }
}