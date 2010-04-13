using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;

// This source file was taken from a blog post by Josh Smith
// http://joshsmithonwpf.wordpress.com/2009/07/11/one-way-to-avoid-messy-propertychanged-event-handling/
// 
// Many thanks to Josh for allowing me to include it with this framework, 
// I find this class incredibly useful.

namespace JulMar.Windows
{
    /// <summary>
    /// Monitors the PropertyChanged event of an object that implements INotifyPropertyChanged,
    /// and executes callback methods (i.e. handlers) registered for properties of that object.
    /// </summary>
    /// <typeparam name="T">The type of object to monitor for property changes.</typeparam>
    public class PropertyObserver<T> : IDisposable, IWeakEventListener where T : class, INotifyPropertyChanged
    {
        private readonly object _lock = new object();
        private Dictionary<string, Action<T>> _propertyNameToHandlerMap;
        private WeakReference _propertySourceRef;

        /// <summary>
        /// Initializes a new instance of PropertyObserver, which
        /// observes the 'propertySource' object for property changes.
        /// </summary>
        /// <param name="propertySource">The object to monitor for property changes.</param>
        public PropertyObserver(T propertySource)
        {
            if (propertySource == null)
                throw new ArgumentNullException("propertySource");

            _propertySourceRef = new WeakReference(propertySource);
            _propertyNameToHandlerMap = new Dictionary<string, Action<T>>();
        }

        /// <summary>
        /// Registers a callback to be invoked when the PropertyChanged event has been raised for the specified property.
        /// </summary>
        /// <param name="expression">A lambda expression like 'n => n.PropertyName'.</param>
        /// <param name="handler">The callback to invoke when the property has changed.</param>
        /// <returns>The object on which this method was invoked, to allow for multiple invocations chained together.</returns>
        public PropertyObserver<T> RegisterHandler(Expression<Func<T, object>> expression, Action<T> handler)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            string propertyName = GetPropertyName(expression);
            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentException("'expression' did not provide a property name.");

            if (handler == null)
                throw new ArgumentNullException("handler");

            var propertySource = this.GetPropertySource();
            if (propertySource != null)
            {
                lock (_lock)
                {
                    _propertyNameToHandlerMap[propertyName] = handler;
                    PropertyChangedEventManager.AddListener(propertySource, this, propertyName);
                }
            }

            return this;
        }

        /// <summary>
        /// Removes the callback associated with the specified property.
        /// </summary>
        /// <param name="expression">A lambda expression like 'n => n.PropertyName'.</param>
        /// <returns>The object on which this method was invoked, to allow for multiple invocations chained together.</returns>
        public PropertyObserver<T> UnregisterHandler(Expression<Func<T, object>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            string propertyName = GetPropertyName(expression);
            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentException("'expression' did not provide a property name.");

            T propertySource = GetPropertySource();
            if (propertySource != null)
            {
                lock (_lock)
                {
                    if (_propertyNameToHandlerMap.ContainsKey(propertyName))
                    {
                        _propertyNameToHandlerMap.Remove(propertyName);
                        PropertyChangedEventManager.RemoveListener(propertySource, this, propertyName);
                    }
                }
            }

            return this;
        }

        private static string GetPropertyName(Expression<Func<T, object>> expression)
        {
            var lambda = expression as LambdaExpression;
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }

            Debug.Assert(memberExpression != null, "Please provide a lambda expression like 'n => n.PropertyName'");

            if (memberExpression != null)
            {
                var propertyInfo = memberExpression.Member as PropertyInfo;
                if (propertyInfo != null)
                    return propertyInfo.Name;
            }

            return null;
        }

        private T GetPropertySource()
        {
            try
            {
                return (T)_propertySourceRef.Target;
            }
            catch
            {
                return default(T);
            }
        }

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(PropertyChangedEventManager))
            {
                string propertyName = ((PropertyChangedEventArgs)e).PropertyName;
                var propertySource = (T)sender;

                if (String.IsNullOrEmpty(propertyName))
                {
                    List<Action<T>> entries;

                    // Get a safe copy of the list
                    lock (_lock)
                    {
                        entries = _propertyNameToHandlerMap.Values.ToList();
                    }

                    // When the property name is empty, all properties are considered to be invalidated.
                    // Iterate over a copy of the list of handlers, in case a handler is registered by a callback.
                    entries.ForEach(h => h(propertySource));
                    return true;
                }

                Action<T> handler = null;
                lock (_lock)
                {
                    _propertyNameToHandlerMap.TryGetValue(propertyName, out handler);
                }

                if (handler != null)
                {
                    handler(propertySource);
                }
            }

            // WPF throws ExecutionEngineException if you return false
            // from here during binding updates.. yikes!
            return true;
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            lock(_lock)
            {
                if (_propertyNameToHandlerMap != null)
                {
                    T propertySource = GetPropertySource();
                    if (propertySource != null)
                    {
                        foreach (var propertyName in _propertyNameToHandlerMap.Keys)
                            PropertyChangedEventManager.RemoveListener(propertySource, this, propertyName);
                    }

                    _propertyNameToHandlerMap.Clear();
                    _propertySourceRef.Target = null;
                }
            }
        }

        #endregion
    }
}