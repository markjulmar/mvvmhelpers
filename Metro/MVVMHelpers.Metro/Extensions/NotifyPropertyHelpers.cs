using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace JulMar.Windows.Extensions
{
    /// <summary>
    /// This class provides a couple of extension methods for raising property change notifications.
    /// These allow easy implementation of INotifyPropertyChanged without deriving from a specific base class.
    /// </summary>
    public static class NotifyPropertyHelpers
    {
        public static SynchronizationContext Context { get; set; }

        /// <summary>
        /// Method to set the context using just the INotifyPropertyChanged interface itself.
        /// You can also just set the above static property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="uiContext"></param>
        public static void SetContext(this INotifyPropertyChanged sender, SynchronizationContext uiContext)
        {
            Context = uiContext;
        }

        /// <summary>
        /// Method to invoke our logic on the correct thread.
        /// </summary>
        /// <param name="eh"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Invoke(PropertyChangedEventHandler eh, object sender, PropertyChangedEventArgs e)
        {
            if (Context != null && SynchronizationContext.Current != Context)
            {
                Context.Post(_ => eh.Invoke(sender, e), null);
            }
            else
            {
                eh.Invoke(sender, e);
            }
        }

        /// <summary>
        /// This can be used to indicate that all property values have changed.
        /// </summary>
        public static void RaiseAllPropertiesChanged(this INotifyPropertyChanged sender, PropertyChangedEventHandler eh)
        {
            if (sender != null && eh != null)
            {
                Invoke(eh, sender, new PropertyChangedEventArgs(string.Empty));
            }
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
        ///           OnPropertyChanged(() => Name);
        ///       }
        ///    }
        /// ]]>
        /// </example>
        /// <typeparam name="T">Type where it is being raised</typeparam>
        /// <param name="eh">Event Handler</param>
        /// <param name="propExpr">Property</param>
        /// <param name="sender">Sender</param>
        public static void RaisePropertyChanged<T>(this INotifyPropertyChanged sender, PropertyChangedEventHandler eh, Expression<Func<T>> propExpr)
        {
            var prop = (PropertyInfo)((MemberExpression)propExpr.Body).Member;
            if (sender != null && eh != null)
            {
                Invoke(eh, sender, new PropertyChangedEventArgs(prop.Name));
            }
        }

        /// <summary>
        /// This raises the INotifyPropertyChanged.PropertyChanged event to indicate
        /// a specific property has changed value.
        /// </summary>
        /// <param name="eh">Event Handler</param>
        /// <param name="name">Primary property</param>
        /// <param name="sender">Sender</param>
        public static void RaisePropertyChanged(this INotifyPropertyChanged sender, PropertyChangedEventHandler eh, [CallerMemberName] string name = null)
        {
            Debug.Assert(!string.IsNullOrEmpty(name) 
                && sender.GetType().GetTypeInfo().GetDeclaredProperty(name) != null);

            if (sender != null && eh != null)
            {
                Invoke(eh, sender, new PropertyChangedEventArgs(name));
            }
        }

        /// <summary>
        /// This is used to set a specific value for a property.
        /// </summary>
        /// <typeparam name="T">Type to set</typeparam>
        /// <param name="sender">Sender</param>
        /// <param name="eh">Event Handler</param>
        /// <param name="storageField">Storage field</param>
        /// <param name="newValue">New value</param>
        /// <param name="propExpr">Property expression</param>
        public static bool SetPropertyValue<T>(this INotifyPropertyChanged sender, PropertyChangedEventHandler eh, ref T storageField, T newValue, Expression<Func<T>> propExpr)
        {
            if (Object.Equals(storageField, newValue))
                return false;

            storageField = newValue;
            var prop = (PropertyInfo)((MemberExpression)propExpr.Body).Member;
            if (sender != null && eh != null)
            {
                Invoke(eh, sender, new PropertyChangedEventArgs(prop.Name));
            }

            return true;
        }

        /// <summary>
        /// This is used to set a specific value for a property.
        /// </summary>
        /// <typeparam name="T">Type to set</typeparam>
        /// <param name="sender">Sender</param>
        /// <param name="eh">Event Handler</param>
        /// <param name="storageField">Storage field</param>
        /// <param name="newValue">New value</param>
        /// <param name="propertyName">Property Name</param>
        public static bool SetPropertyValue<T>(this INotifyPropertyChanged sender, PropertyChangedEventHandler eh, ref T storageField, T newValue, [CallerMemberName] string propertyName = "")
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyName) 
                && sender.GetType().GetTypeInfo().GetDeclaredProperty(propertyName) != null);
            if (Object.Equals(storageField, newValue))
                return false;

            storageField = newValue;
            if (sender != null && eh != null)
            {
                Invoke(eh, sender, new PropertyChangedEventArgs(propertyName));
            }

            return true;
        }
    }
}