using System;
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
    /// 
    /// Note that this will perform a SHALLOW copy of the fields.  Collections/Arrays/KVPs will not be properly
    /// handled by this implementation - you should provide your own implementation if you use these fields in your
    /// view models and expect them to be properly saved.
    /// </summary>
    public class EditingViewModel : EditingViewModelBase<ShallowCopyEditableObject>
    {
        /// <summary>
        /// Optional test as fields are persisted to determine if the field's state should
        /// be saved during an editing operation.  This is not terribly useful for auto-property
        /// backed fields.
        /// </summary>
        public Predicate<FieldInfo> FieldPredicate { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public EditingViewModel() : base(new ShallowCopyEditableObject())
        {
        }
    }

    /// <summary>
    /// This provides the base class for editable object support.  The actual saving/restoring of the object
    /// state is accomplished through an implementation class provided as a generic template parameter.  This can
    /// be used as a basis to creating different supporting types for editable objects.
    /// </summary>
    /// <typeparam name="T">Editable persistence implementation</typeparam>
    public class EditingViewModelBase<T> : ValidatingViewModel, IEditableObject where T : IEditStateBag
    {
        /// <summary>
        /// The editing state
        /// </summary>
        protected IEditStateBag EditState { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        protected EditingViewModelBase(IEditStateBag editingStateBag)
        {
            if (editingStateBag == null)
                throw new ArgumentNullException("editingStateBag", "Must supply state object");
            
            EditState = editingStateBag;
        }

        /// <summary>
        /// Returns true if this object is currently being edited
        /// </summary>
        public bool IsEditing
        {
            get { return EditState.IsEditing; }
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
            OnEditComplete(false);
        }

        /// <summary>
        /// Pushes changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> or <see cref="M:System.ComponentModel.IBindingList.AddNew"/> call into the underlying object.
        /// </summary>
        public void EndEdit()
        {
            bool success = OnEditEnding();
            OnEditComplete(success);
        }

        /// <summary>
        /// Interception point for derived logic to do work when beginning edit.
        /// </summary>
        protected virtual void OnBeginEdit()
        {
            EditState.OnBeginEdit(this);
        }

        /// <summary>
        /// This is called in response to either CancelEdit or EndEdit and provides an interception point.
        /// </summary>
        /// <returns>True to commit, False to rollback</returns>
        protected virtual bool OnEditEnding()
        {
            return true;
        }

        /// <summary>
        /// Called once changes are committed.
        /// </summary>
        /// <param name="succeeded"></param>
        protected virtual void OnEditComplete(bool succeeded)
        {
            EditState.OnEndEdit(this, succeeded);
        }
    }

    /// <summary>
    /// This is the implementation class used for providing editable object support
    /// </summary>
    public interface IEditStateBag
    {
        /// <summary>
        /// True/False if this object is being edited currently
        /// </summary>
        bool IsEditing { get; }
        
        /// <summary>
        /// Called to begin an edit operation
        /// </summary>
        void OnBeginEdit(ValidatingViewModel vm);

        /// <summary>
        /// Called to end an edit operation
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="success">True for persist, False to cancel edits</param>
        void OnEndEdit(ValidatingViewModel vm, bool success);
    }

    /// <summary>
    /// This implementation provides editable support by saving the state in a dictionary.
    /// </summary>
    public class ShallowCopyEditableObject : IEditStateBag
    {
        /// <summary>
        /// This stores the current "copy" of the object.
        /// </summary>
        private Dictionary<string, object> _savedState;

        /// <summary>
        /// Called to begin an edit operation
        /// </summary>
        public void OnBeginEdit(ValidatingViewModel vm)
        {
            _savedState = GetFieldValues(vm);
        }

        /// <summary>
        /// Called to end an edit operation
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="success">True for persist, False to cancel edits</param>
        public void OnEndEdit(ValidatingViewModel vm, bool success)
        {
            if (!success)
                RestoreFieldValues(vm, _savedState);
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
        /// Returns the fields to capture
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private IEnumerable<FieldInfo> GetFieldsToSerialize(ValidatingViewModel target)
        {
            EditingViewModel evm = target as EditingViewModel;
            Predicate<FieldInfo> testField = (evm != null && evm.FieldPredicate != null) ? evm.FieldPredicate : f => true;

            return target.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => f.GetType() != typeof(ShallowCopyEditableObject) && testField(f));
        }

        /// <summary>
        /// This is used to clone the object.  Override the method to provide a more efficient clone.  The
        /// default implementation simply reflects across the object copying every field.
        /// </summary>
        /// <returns>Clone of current object</returns>
        private Dictionary<string, object> GetFieldValues(ValidatingViewModel target)
        {
            return GetFieldsToSerialize(target)
                .Select(fi => new { Key = fi.Name, Value = fi.GetValue(target) })
                .ToDictionary(k => k.Key, k => k.Value);
        }

        /// <summary>
        /// This restores the state of the current object from the passed clone object.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="fieldValues">Object to restore state from</param>
        private void RestoreFieldValues(ValidatingViewModel target, Dictionary<string, object> fieldValues)
        {
            foreach (FieldInfo fi in GetFieldsToSerialize(target))
            {
                object value;
                if (fieldValues.TryGetValue(fi.Name, out value))
                    fi.SetValue(target, value);
                else
                    Debug.WriteLine("Failed to restore field " + fi.Name + " from cloned values, field not found in Dictionary.");
            }
        }
    }

}