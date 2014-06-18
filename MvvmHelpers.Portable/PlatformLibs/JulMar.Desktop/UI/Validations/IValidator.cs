namespace JulMar.UI.Validations
{
    /// <summary>
    /// Validation interface to check a property value.
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// This is the method called to validate a specific property value.
        /// </summary>
        /// <param name="name">Property name</param>
        /// <param name="newValue">New Property value</param>
        /// <returns>Error string, empty if no error</returns>
        string Validate(string name, object newValue);
    }
}
