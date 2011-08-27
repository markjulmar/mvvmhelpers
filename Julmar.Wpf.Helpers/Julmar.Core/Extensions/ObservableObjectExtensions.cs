using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace JulMar.Core.Extensions
{
    /// <summary>
    /// This class provides methods to manage the INotifyPropertyChanged interface to provide
    /// simple property change notifications.  It has the option of passing
    /// the old and new value if thread-safe semantics are required.  This will
    /// ensure the known value is passed with the event so that the consumer doesn't
    /// have to read the (possibly changed) value, or take a lock to retrieve the value.
    /// </summary>
    public static class ObservableObjectExtensions
    {
        static readonly ConditionalWeakTable<object, Dictionary<PropertyInfo, object>> DataStore = new ConditionalWeakTable<object, Dictionary<PropertyInfo, object>>();
        static readonly ConditionalWeakTable<object, Func<PropertyChangedEventHandler>> PropertyChangedHandlers = new ConditionalWeakTable<object, Func<PropertyChangedEventHandler>>();

        /// <summary>
        /// This is used to associate the PropertyChanged event handler with the object so it can be cached.
        /// </summary>
        /// <param name="self">Owner</param>
        /// <param name="handlerFunc">PropertyChanged handler</param>
        public static void RegisterPropertyChangedHandler(this INotifyPropertyChanged self, Func<PropertyChangedEventHandler> handlerFunc)
        {
            PropertyChangedHandlers.Add(self, handlerFunc);
        }

        /// <summary>
        /// This is used to raise the property changed event for all properties
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="self">Owner</param>
        public static void OnPropertyChanged<T>(this INotifyPropertyChanged self)
        {
            self.OnPropertyChanged(new PropertyChangedEventArgs(null));
        }

        /// <summary>
        /// This is used to raise the property changed event. It attempts to use the cached handler, if it cannot
        /// then it resorts to a runtime lookup of the property changed event field.
        /// </summary>
        /// <param name="self">Owner</param>
        /// <param name="propertyName">Property name</param>
        public static void OnPropertyChanged(this INotifyPropertyChanged self, string propertyName)
        {
            Debug.Assert(string.IsNullOrEmpty(propertyName) || self.GetType().GetProperty(propertyName) != null);
            self.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// This is used to raise the property changed event using an expression.
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="self">Owner</param>
        /// <param name="propertyExpression">Expression tree</param>
        public static void OnPropertyChanged<T>(this INotifyPropertyChanged self, Expression<Func<T>> propertyExpression)
        {
            var propertyInfo = (PropertyInfo)((MemberExpression)propertyExpression.Body).Member;
            self.OnPropertyChanged(new PropertyChangedEventArgs(propertyInfo.Name));
        }

        /// <summary>
        /// This is used to raise the property changed event. It attempts to use the cached handler, if it cannot
        /// then it resorts to a runtime lookup of the property changed event field.
        /// </summary>
        /// <param name="self">Owner</param>
        /// <param name="e">Property event argument</param>
        public static void OnPropertyChanged(this INotifyPropertyChanged self, PropertyChangedEventArgs e)
        {
            Func<PropertyChangedEventHandler> propertyChangedHandler;
            if (PropertyChangedHandlers.TryGetValue(self, out propertyChangedHandler))
            {
                var inpc = propertyChangedHandler();
                if (inpc != null)
                    inpc(self, e);
            }
            else
            {
                Trace.WriteLine("WARNING ObservableObjectExtensions: PropertyChangedHandler not registered for " + self + ", attempted slow reflection lookup instead.");
                FieldInfo fi = self.GetType().GetField("PropertyChanged", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                if (fi != null)
                {
                    var pc = (PropertyChangedEventHandler)fi.GetValue(self);
                    if (pc != null && pc.GetInvocationList().Length > 0)
                        pc.Invoke(self, e);
                }
            }
        }

        /// <summary>
        /// This is used to retrieve the backing field for a given property.
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="self">Owner</param>
        /// <param name="propertyExpression">Expression tree</param>
        /// <returns>Current value of property</returns>
        public static T GetBackingValue<T>(this INotifyPropertyChanged self, Expression<Func<T>> propertyExpression)
        {
            var values = DataStore.GetValue(self, o => new Dictionary<PropertyInfo, object>());
            var propertyInfo = (PropertyInfo)((MemberExpression)propertyExpression.Body).Member;
            object value;
            
            return !values.TryGetValue(propertyInfo, out value) ? default(T) : (T) value;
        }

        /// <summary>
        /// This is used to change the backing field for a given property. If it has a new value then the 
        /// property change notification is raised.
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="self">Owner</param>
        /// <param name="propertyExpression">Expression tree</param>
        /// <param name="value">New value</param>
        /// <param name="useExtendedNotify">True to use extended PropertyChangedEventArgs</param>
        public static void SetBackingValue<T>(this INotifyPropertyChanged self, Expression<Func<T>> propertyExpression, T value, bool useExtendedNotify = false)
        {
            var values = DataStore.GetValue(self, o => new Dictionary<PropertyInfo, object>());
            var propertyInfo = (PropertyInfo)((MemberExpression)propertyExpression.Body).Member;
            object oldValue;
            if (!values.TryGetValue(propertyInfo, out oldValue))
            {
                oldValue = default(T);
            }
 
            if (!Equals(value, oldValue))
            {
                values[propertyInfo] = value;

                PropertyChangedEventArgs e = (useExtendedNotify)
                                                 ? new PropertyChangedEventArgsEx(propertyInfo.Name, oldValue, value)
                                                 : new PropertyChangedEventArgs(propertyInfo.Name);
                self.OnPropertyChanged(e);
            }
        }
    }
}
