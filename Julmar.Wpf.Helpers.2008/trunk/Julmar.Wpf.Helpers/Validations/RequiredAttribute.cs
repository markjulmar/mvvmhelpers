namespace JulMar.Windows.Validations
{
    /// <summary>
    /// This validates that a property has a value and is non-null.
    /// </summary>
    public sealed class RequiredAttribute : ValidatorAttribute
    {
        /// <summary>
        /// This is the method called to validate a specific property value.
        /// </summary>
        /// <param name="name">Property name</param>
        /// <param name="value">Value</param>
        /// <returns>Error string, empty if no error</returns>
        public override string Validate(string name, object value)
        {
            return value == null || value.ToString().Length == 0 ? name + " must be supplied." : null;
        }
    }
}
