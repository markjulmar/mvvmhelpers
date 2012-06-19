using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace JulMar.Windows.Mvvm
{
    /// <summary>
    /// This class implements the simplest view model -- one that implements INPC.
    /// </summary>
    public class SimpleViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// This can be used to indicate that all property values have changed.
        /// </summary>
        protected void RaiseAllPropertiesChanged()
        {
            PropertyChanged(this, new PropertyChangedEventArgs(string.Empty));
        }

        /// <summary>
        /// This raises the INotifyPropertyChanged.PropertyChanged event to indicate
        /// a specific property has changed value. This version provides a compile-time safe
        /// way to indicate the property through the use of an expression tree / lambda.
        /// Be aware that for high-volume changes this version might be much slower than
        /// the above "magic-string" version due to the creation of an expression and runtime lookup.
        /// </summary>
        /// <example>
        /// <![CDATA[
        ///    public string Name
        ///    {
        ///       get { return _name; }
        ///       set
        ///       {
        ///           _name = value;
        ///           RaisePropertyChanged(() => Name);
        ///       }
        ///    }
        /// ]]>
        /// </example>
        /// <typeparam name="T">Type where it is being raised</typeparam>
        /// <param name="propExpr">Property</param>
        protected void RaisePropertyChanged<T>(Expression<Func<T>> propExpr)
        {
            var prop = (PropertyInfo)((MemberExpression)propExpr.Body).Member;
            PropertyChanged(this, new PropertyChangedEventArgs(prop.Name));
        }

        /// <summary>
        /// This raises the INotifyPropertyChanged.PropertyChanged event to indicate
        /// a specific property has changed value.
        /// </summary>
        /// <param name="propertyName">Primary property</param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName= "")
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyName) && GetType().GetProperty(propertyName) != null);
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// This is used to set a specific value for a property.
        /// </summary>
        /// <typeparam name="T">Type to set</typeparam>
        /// <param name="storageField">Storage field</param>
        /// <param name="newValue">New value</param>
        /// <param name="propExpr">Property expression</param>
        protected bool SetPropertyValue<T>(ref T storageField, T newValue, Expression<Func<T>> propExpr)
        {
            if (Object.Equals(storageField, newValue))
                return false;
            storageField = newValue;
            var prop = (PropertyInfo)((MemberExpression)propExpr.Body).Member;
            PropertyChanged(this, new PropertyChangedEventArgs(prop.Name));
            return true;
        }

        /// <summary>
        /// This is used to set a specific value for a property.
        /// </summary>
        /// <typeparam name="T">Type to set</typeparam>
        /// <param name="storageField">Storage field</param>
        /// <param name="newValue">New value</param>
        /// <param name="propertyName">Property Name</param>
        protected bool SetPropertyValue<T>(ref T storageField, T newValue, [CallerMemberName] string propertyName = "")
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyName) && GetType().GetProperty(propertyName) != null);
            if (Object.Equals(storageField, newValue))
                return false;
            storageField = newValue;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
