using System;

namespace JulMar.UI.Validations
{
    /// <summary>
    /// This attribute is applied to properties to perform validation checking.
    /// It is derived from to create specific validation types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public abstract class ValidatorAttribute : Attribute, IValidator
    {
        /// <summary>
        /// This is the method called to validate a specific property value.
        /// </summary>
        /// <param name="name">Property name</param>
        /// <param name="newValue">New value for this property</param>
        /// <returns>Error string, empty if no error</returns>
        public abstract string Validate(string name, object newValue);
    }
}