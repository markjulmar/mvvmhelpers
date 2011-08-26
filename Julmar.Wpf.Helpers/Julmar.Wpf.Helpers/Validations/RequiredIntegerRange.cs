using System;
using System.Globalization;

namespace JulMar.Windows.Validations
{
    /// <summary>
    /// This validates that a property is numeric and within a certain range
    /// </summary>
    public sealed class RequiredIntegerRange : ValidatorAttribute
    {
        private int? _min;
        private int? _max;

        /// <summary>
        /// Minimum value required
        /// </summary>
        public int Minimum
        {
            get { return (_min.HasValue) ? _min.Value : Int32.MinValue; }
            set { _min = value; }
        }

        /// <summary>
        /// Maximum value required
        /// </summary>
        public int Maximum
        {
            get { return (_max.HasValue) ? _max.Value : Int32.MaxValue; }
            set { _max = value; }
        }

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

            int intValue;
            if (!Int32.TryParse(value.ToString(), out intValue))
                return string.Format(CultureInfo.CurrentUICulture, "{0} must be a valid integer", name);

            return ((!_min.HasValue || intValue >= _min.Value) &&
                    (!_max.HasValue || intValue <= _max.Value))
                       ? null
                       : string.Format(CultureInfo.CurrentUICulture, "{0} must be between {1} and {2}.", name, Minimum, Maximum);
        }
    }
}
