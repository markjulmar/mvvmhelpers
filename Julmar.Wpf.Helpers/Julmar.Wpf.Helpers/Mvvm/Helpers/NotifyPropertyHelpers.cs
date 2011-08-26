using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace JulMar.Windows.Mvvm.Helpers
{
    /// <summary>
    /// This class provides a couple of extension methods for raising property change notifications.
    /// These allow easy implementation of INotifyPropertyChanged without deriving from a specific base class.
    /// </summary>
    public static class NotifyPropertyHelpers
    {
        /// <summary>
        /// This method raises a PropertyChanged notification completely through reflection
        /// against the underlying field.  It requires that the type implement the INotifyPropertyChanged interface
        /// with a simple backing event.
        /// </summary>
        /// <typeparam name="T">Property type being passed (string)</typeparam>
        /// <param name="self">Object that implements INotifyPropertyChanged</param>
        /// <param name="expr">Expression Tree for typesafe property</param>
        public static void RaisePropertyChanged<T>(this INotifyPropertyChanged self, Expression<Func<T>> expr)
        {
            FieldInfo fi = self.GetType().GetField("PropertyChanged", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            if (fi != null)
            {
                var prop = (PropertyInfo)((MemberExpression)expr.Body).Member;
                var pc = (PropertyChangedEventHandler) fi.GetValue(self);
                if (pc != null && pc.GetInvocationList().Length > 0)
                    pc.Invoke(self, new PropertyChangedEventArgs(prop.Name));
            }
        }

        /// <summary>
        /// This method allows you to pass in the PropertyChangedEventHandler (event) and raises it without reflection.
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
        /// This method uses the string name for the property.
        /// </summary>
        /// <param name="self">Object that implements INotifyPropertyChanged</param>
        /// <param name="propertyName">Property Name (string)</param>
        public static void RaisePropertyChanged(this INotifyPropertyChanged self, string propertyName)
        {
            Debug.Assert(string.IsNullOrEmpty(propertyName) || self.GetType().GetProperty(propertyName) != null);
            FieldInfo fi = self.GetType().GetField("PropertyChanged", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            if (fi != null)
            {
                var pc = (PropertyChangedEventHandler)fi.GetValue(self);
                if (pc != null && pc.GetInvocationList().Length > 0)
                    pc.Invoke(self, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// This method uses the string name for the property.
        /// </summary>
        /// <param name="self">Object that implements INotifyPropertyChanged</param>
        /// <param name="propertyChangedHandler">Event Handler for INotifyPropertyChanged</param>
        /// <param name="propertyName">Property Name (string)</param>
        public static void RaisePropertyChanged(this INotifyPropertyChanged self, PropertyChangedEventHandler propertyChangedHandler, string propertyName)
        {
            Debug.Assert(string.IsNullOrEmpty(propertyName) || self.GetType().GetProperty(propertyName) != null);
            if (propertyChangedHandler != null)
                propertyChangedHandler(self, new PropertyChangedEventArgs(propertyName));
        }
    }
}