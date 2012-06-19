using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace JulMar.Windows.Extensions
{
    /// <summary>
    /// This class provides a couple of extension methods for raising property change notifications.
    /// These allow easy implementation of INotifyPropertyChanged without deriving from a specific base class.
    /// </summary>
    public static class NotifyPropertyHelpers
    {
        /// <summary>
        /// Raises a property change notification on any INotifyPropertyChanged implementation.
        /// </summary>
        /// <typeparam name="T">Property type being passed (string)</typeparam>
        /// <param name="self">Object that implements INotifyPropertyChanged</param>
        /// <param name="propertyChangedHandler">Event Handler for INotifyPropertyChanged</param>
        /// <param name="expr">Expression Tree for typesafe property</param>
        public static void RaisePropertyChanged<T>(this INotifyPropertyChanged self, PropertyChangedEventHandler propertyChangedHandler, Expression<Func<T>> expr)
        {
            var prop = (PropertyInfo)((MemberExpression)expr.Body).Member;
            if (propertyChangedHandler != null)
                propertyChangedHandler(self, new PropertyChangedEventArgs(prop.Name));
        }

        /// <summary>
        /// Raises an "all properties changed" event
        /// </summary>
        /// <param name="self">Object that implements INotifyPropertyChanged</param>
        /// <param name="propertyChangedHandler">Event Handler for INotifyPropertyChanged</param>
        public static void RaiseAllPropertiesChanged(this INotifyPropertyChanged self, PropertyChangedEventHandler propertyChangedHandler)
        {
            if (propertyChangedHandler != null)
                propertyChangedHandler(self, new PropertyChangedEventArgs(string.Empty));
        }

        /// <summary>
        /// This method uses the string name for the property.
        /// </summary>
        /// <param name="self">Object that implements INotifyPropertyChanged</param>
        /// <param name="propertyChangedHandler">Event Handler for INotifyPropertyChanged</param>
        /// <param name="propertyName">Property Name (string)</param>
        public static void RaisePropertyChanged(this INotifyPropertyChanged self, PropertyChangedEventHandler propertyChangedHandler, [CallerMemberName] string propertyName = "")
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyName) && self.GetType().GetProperty(propertyName) != null);
            if (propertyChangedHandler != null)
                propertyChangedHandler(self, new PropertyChangedEventArgs(propertyName));
        }
    }
}