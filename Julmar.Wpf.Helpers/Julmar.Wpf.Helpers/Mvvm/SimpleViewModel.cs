using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

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
        protected void OnPropertyChanged()
        {
            PropertyChanged(this, new PropertyChangedEventArgs(string.Empty));
        }

        /// <summary>
        /// This raises the INotifyPropertyChanged.PropertyChanged event to indicate
        /// a specific property has changed value. This version provides a compile-time safe
        /// way to indicate the property through the use of an expression tree / lambda.
        /// </summary>
        /// <example>
        /// <![CDATA[
        ///    public string Name
        ///    {
        ///       get { return _name; }
        ///       set
        ///       {
        ///           _name = value;
        ///           OnPropertyChanged(() => Name);
        ///       }
        ///    }
        /// ]]>
        /// </example>
        /// <typeparam name="T">Type where it is being raised</typeparam>
        /// <param name="propExpr">Property</param>
        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> propExpr)
        {
            var prop = (PropertyInfo)((MemberExpression)propExpr.Body).Member;
            PropertyChanged(this, new PropertyChangedEventArgs(prop.Name));
        }

        /// <summary>
        /// This raises the INotifyPropertyChanged.PropertyChanged event to indicate
        /// a specific property has changed value.
        /// </summary>
        /// <param name="name">Primary property</param>
        /// <param name="propertyNames">Additional properties</param>
        protected virtual void OnPropertyChanged(string name, params string[] propertyNames)
        {
            Debug.Assert(string.IsNullOrEmpty(name) || GetType().GetProperty(name) != null);
            PropertyChanged(this, new PropertyChangedEventArgs(name));
            if (propertyNames != null)
            {
                foreach (var propName in propertyNames)
                    OnPropertyChanged(propName);
            }
        }

    }
}
