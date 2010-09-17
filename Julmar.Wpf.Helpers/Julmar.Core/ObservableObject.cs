using System.Collections.Generic;
using System.Diagnostics;
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
        public PropertyChangedEventArgsEx(string propertyName) : base(propertyName)
        {
        }

        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="propertyName">Property that has changed</param>
        /// <param name="newValue">New Value</param>
        public PropertyChangedEventArgsEx(string propertyName, object newValue) : base(propertyName)
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
        public PropertyChangedEventArgsEx(string propertyName, object oldValue, object newValue) : base(propertyName)
        {
            HasNewValue = HasOldValue = true;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    /// <summary>
    /// This class implements the INotifyPropertyChanged interface to provide
    /// simple property change notifications.  It has the option of passing
    /// the old and new value if thread-safe semantics are required.  This will
    /// ensure the known value is passed with the event so that the consumer doesn't
    /// have to read the (possibly changed) value, or take a lock to retrieve the value.
    /// </summary>
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs during a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// Constructor
        /// </summary>
        protected ObservableObject()
        {
        }

        /// <summary>
        /// This method can be used to change a property value.
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="propertyName">Name of the property</param>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        /// <returns>True if value was changed, False if not</returns>
        protected bool ChangeValue<T>(string propertyName, ref T oldValue, T newValue)
        {
            // See if the property is actually changing values.
            if (EqualityComparer<T>.Default.Equals(oldValue,newValue) == false)
            {
                var lastValue = oldValue;
                oldValue = newValue;
                OnPropertyChanged(new PropertyChangedEventArgsEx(propertyName, lastValue, newValue));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="propertyName">Property Changing value</param>
        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgsEx(propertyName));
        }

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="propertyName">Property Changing value</param>
        /// <param name="newValue">New Value</param>
        protected void OnPropertyChanged(string propertyName, object newValue)
        {
            OnPropertyChanged(new PropertyChangedEventArgsEx(propertyName, newValue));
        }

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="propertyName">Property Changing value</param>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            OnPropertyChanged(new PropertyChangedEventArgsEx(propertyName, oldValue, newValue));
        }

        /// <summary>
        /// This raises the INotifyPropertyChanged.PropertyChanged event to indicate
        /// a specific property has changed value.
        /// </summary>
        /// <param name="e">PropertyChangingEventArgs</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgsEx e)
        {
            Debug.Assert(string.IsNullOrEmpty(e.PropertyName) || GetType().GetProperty(e.PropertyName) != null);
            // We capture the current value in case another thread modifies it during iteration.
            // We don't have to test for null since we've already assigned it to a blank delegate.
            var inpc = PropertyChanged;
            inpc.Invoke(this, e);
        }
    }
}
