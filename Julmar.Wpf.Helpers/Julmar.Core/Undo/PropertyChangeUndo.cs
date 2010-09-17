using System;
using System.Diagnostics;
using JulMar.Core.Interfaces;

namespace JulMar.Core.Undo
{
    /// <summary>
    /// This implements the undo mechanics for a PropertyChange.
    /// This does not support index array properties.
    /// </summary>
    public class PropertyChangeUndo : ISupportUndo
    {
        /// <summary>
        /// Object that has been changed
        /// </summary>
        private readonly object _target;
        /// <summary>
        /// Property that was changed
        /// </summary>
        private readonly string _propertyName;
        /// <summary>
        /// Old value of property
        /// </summary>
        private readonly object _value;
        /// <summary>
        /// New value of property
        /// </summary>
        private readonly object _newValue;
        /// <summary>
        /// True if we support REDO (old value was supplied)
        /// </summary>
        private readonly bool _supportRedo;

        /// <summary>
        /// Constructor when only the old value is known
        /// </summary>
        /// <param name="target">Target that has changed</param>
        /// <param name="propName">Property</param>
        /// <param name="oldValue">Old value</param>
        public PropertyChangeUndo(object target, string propName, object oldValue)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (string.IsNullOrEmpty(propName))
                throw new ArgumentNullException("propName");

            _supportRedo = false;
            _target = target;
            _propertyName = propName;
            _value = oldValue;

            Debug.Assert(target.GetType().GetProperty(propName) != null);
        }

        /// <summary>
        /// Constructor when an old and new value are known
        /// </summary>
        /// <param name="target">Target that has changed</param>
        /// <param name="propName">Property</param>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        public PropertyChangeUndo(object target, string propName, object oldValue, object newValue)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (string.IsNullOrEmpty(propName))
                throw new ArgumentNullException("propName");

            _supportRedo = true;
            _target = target;
            _propertyName = propName;
            _value = oldValue;
            _newValue = newValue;

            Debug.Assert(target.GetType().GetProperty(propName) != null);
        }

        /// <summary>
        /// True if operation can be "reapplied" after undo.
        /// </summary>
        public bool CanRedo { get { return _supportRedo; } }

        /// <summary>
        /// Method used to undo operation
        /// </summary>
        public void Undo()
        {
            _target.GetType().GetProperty(_propertyName).SetValue(_target, _value, null);
        }

        /// <summary>
        /// Method to redo operation
        /// </summary>
        public void Redo()
        {
            if (_supportRedo)
                _target.GetType().GetProperty(_propertyName).SetValue(_target, _newValue, null);
        }
    }
}