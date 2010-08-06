using System.ComponentModel;
using JulMar.Windows.Mvvm;

namespace JulMar.Windows.Validations
{
    /// <summary>
    /// This class can be used as a base class for ViewModel objects that want to support
    /// validations through IDataErrorInfo.
    /// </summary>
    public class ValidatingViewModel : ViewModel, IDataErrorInfo
    {
        #region IDataErrorInfo
        string IDataErrorInfo.Error
        {
            get { return ValidateOverride(null, this, ValidationManager.Validate(null, this)); }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get { return ValidateOverride(columnName, this, ValidationManager.Validate(columnName, this)); }
        }
        #endregion

        /// <summary>
        /// This method is called during the validation process.  It can be overridden
        /// to alter or remove the error text being returned.
        /// </summary>
        /// <param name="propertyName">Property being validated (null for all)</param>
        /// <param name="instance">Object instance</param>
        /// <param name="errorText">Existing error text</param>
        /// <returns>New error text</returns>
        protected virtual string ValidateOverride(string propertyName, object instance, string errorText)
        {
            return errorText;
        }
    }
}