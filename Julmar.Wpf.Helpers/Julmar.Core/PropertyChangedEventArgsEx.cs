using System.ComponentModel;

namespace JulMar.Core
{
    /// <summary>
    /// A single EventArgs to capture property change events 
    /// that include the old and new values for thread safety.
    /// </summary>
    public class PropertyChangedEventArgsEx : PropertyChangedEventArgs
    {
        /// <summary>
        /// True if the OldValue is valid
        /// </summary>
        public bool HasOldValue { get; private set; }

        /// <summary>
        /// Old value
        /// </summary>
        public object OldValue { get; private set; }

        /// <summary>
        /// True if the NewValue is valid
        /// </summary>
        public bool HasNewValue { get; private set; }

        /// <summary>
        /// New value
        /// </summary>
        public object NewValue { get; private set; }

        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="propertyName">Property that has changed</param>
        public PropertyChangedEventArgsEx(string propertyName)
            : base(propertyName)
        {
        }

        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="propertyName">Property that has changed</param>
        /// <param name="newValue">New Value</param>
        public PropertyChangedEventArgsEx(string propertyName, object newValue)
            : base(propertyName)
        {
            HasNewValue = true;
            NewValue = newValue;
        }

        /// <summary>
        /// Full Constructor
        /// </summary>
        /// <param name="propertyName">Property that has changed</param>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New Value</param>
        public PropertyChangedEventArgsEx(string propertyName, object oldValue, object newValue)
            : base(propertyName)
        {
            HasNewValue = HasOldValue = true;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}