using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using JulMar.Windows.Validations;

namespace JulMar.Windows.Mvvm
{
    /// <summary>
    /// This provides a ViewModel that implements IEditableObject through cloning - a copy of the existing object 
    /// is created during the edits and thrown away when the editing is completed.  If the edit operation fails 
    /// and is canceled, then the copy replaces the existing version.
    /// </summary>
    public class EditingViewModel : EditingViewModelBase<ShallowCopyEditableObject>
    {
        /* No overrides */
    }

    /// <summary>
    /// This provides the base class for editable object support.  The actual saving/restoring of the object
    /// state is accomplished through an implementation class provided as a generic template parameter.  This can
    /// be used as a basis to creating different supporting types for editable objects.
    /// </summary>
    /// <typeparam name="T">Editable persistence implementation</typeparam>
    public class EditingViewModelBase<T> : ValidatingViewModel, IEditableObject where T : IEditableObjectImpl, new()
    {
        private readonly IEditableObjectImpl _editImpl = new T();

        /// <summary>
        /// Returns true if this object is currently being edited
        /// </summary>
        public bool IsEditing
        {
            get { return _editImpl.IsEditing;  }
        }

        /// <summary>
        /// Begins an edit on an object.
        /// </summary>
        public void BeginEdit()
        {
            OnBeginEdit();
        }

        /// <summary>
        /// Discards changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> call.
        /// </summary>
        public void CancelEdit()
        {
            OnEndEdit(false);
        }

        /// <summary>
        /// Pushes changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> or <see cref="M:System.ComponentModel.IBindingList.AddNew"/> call into the underlying object.
        /// </summary>
        public void EndEdit()
        {
            OnEndEdit(true);
        }

        /// <summary>
        /// Interception point for derived logic to do work when beginning edit.
        /// </summary>
        protected virtual void OnBeginEdit()
        {
            _editImpl.OnBeginEdit();
        }

        /// <summary>
        /// This is called in response to either CancelEdit or EndEdit and provides an interception point.
        /// </summary>
        /// <param name="succeeded">True if edit was succesful.</param>
        protected virtual void OnEndEdit(bool succeeded)
        {
            _editImpl.OnEndEdit(succeeded);
        }
    }

    /// <summary>
    /// This is the implementation class used for providing editable object support
    /// </summary>
    public interface IEditableObjectImpl
    {
        /// <summary>
        /// True/False if this object is being edited currently
        /// </summary>
        bool IsEditing { get; }
        /// <summary>
        /// Called to begin an edit operation
        /// </summary>
        void OnBeginEdit();
        /// <summary>
        /// Called to end an edit operation
        /// </summary>
        /// <param name="success">True for persist, False to cancel edits</param>
        void OnEndEdit(bool success);
    }

    /// <summary>
    /// This implementation provides editable support by saving the state in a dictionary.
    /// </summary>
    public class ShallowCopyEditableObject : IEditableObjectImpl
    {
        /// <summary>
        /// This stores the current "copy" of the object.
        /// </summary>
        private Dictionary<string, object> _savedState;

        /// <summary>
        /// Called to begin an edit operation
        /// </summary>
        public void OnBeginEdit()
        {
            _savedState = GetFieldValues();
        }

        /// <summary>
        /// Called to end an edit operation
        /// </summary>
        /// <param name="success">True for persist, False to cancel edits</param>
        public void OnEndEdit(bool success)
        {
            if (!success)
                RestoreFieldValues(_savedState);
            _savedState = null;
        }

        /// <summary>
        /// True/False if this object is being edited currently
        /// </summary>
        public bool IsEditing
        {
            get { return _savedState != null; }
        }

        /// <summary>
        /// This is used to clone the object.  Override the method to provide a more efficient clone.  The
        /// default implementation simply reflects across the object copying every field.
        /// </summary>
        /// <returns>Clone of current object</returns>
        private Dictionary<string, object> GetFieldValues()
        {
            return GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Select(
                fi => new { Key = fi.Name, Value = fi.GetValue(this) }).ToDictionary(k => k.Key, k => k.Value);
        }

        /// <summary>
        /// This restores the state of the current object from the passed clone object.
        /// </summary>
        /// <param name="fieldValues">Object to restore state from</param>
        private void RestoreFieldValues(Dictionary<string, object> fieldValues)
        {
            foreach (FieldInfo fi in GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                object value;
                if (fieldValues.TryGetValue(fi.Name, out value))
                    fi.SetValue(this, value);
                else
                    Debug.WriteLine("Failed to restore field " + fi.Name + " from cloned values, field not found in Dictionary.");
            }
        }
    }

}