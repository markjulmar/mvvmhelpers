using System;
using System.Globalization;

namespace JulMar.UI.Validations
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
            get { return (this._min.HasValue) ? this._min.Value : Int32.MinValue; }
            set { this._min = value; }
        }

        /// <summary>
        /// Maximum value required
        /// </summary>
        public int Maximum
        {
            get { return (this._max.HasValue) ? this._max.Value : Int32.MaxValue; }
            set { this._max = value; }
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

            return ((!this._min.HasValue || intValue >= this._min.Value) &&
                    (!this._max.HasValue || intValue <= this._max.Value))
                       ? null
                       : string.Format(CultureInfo.CurrentUICulture, "{0} must be between {1} and {2}.", name, this.Minimum, this.Maximum);
        }
    }
}
