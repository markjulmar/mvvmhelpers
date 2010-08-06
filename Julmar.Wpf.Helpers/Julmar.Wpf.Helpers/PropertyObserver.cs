using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;

// The original source was taken from a blog post by Josh Smith:
// http://joshsmithonwpf.wordpress.com/2009/07/11/one-way-to-avoid-messy-propertychanged-event-handling/
// 
// It's slightly modified for my usage than the original, many thanks to Josh for allowing me to include 
// it with this framework, I find this class incredibly useful.

namespace JulMar.Windows
{
    /// <summary>
    /// Monitors the PropertyChanged event of an object that implements INotifyPropertyChanged,
    /// and executes callback methods (i.e. handlers) registered for properties of that object.
    /// </summary>
    /// <typeparam name="T">The type of object to monitor for property changes.</typeparam>
    public class PropertyObserver<T> : IDisposable, IWeakEventListener 
        where T : class, INotifyPropertyChanged
    {
        private readonly object _lock = new object();
        private readonly Dictionary<string, Action<T>> _propertyChangedToHandlerMap;
        private readonly WeakReference _source;
        private EventHandler<PropertyChangedEventArgs> _propChangedCallback;

        /// <summary>
        /// This event list gets invoked for *every* property change on the object.
        /// However, it will not keep the object alive.
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs> PropertyChanged
        {
            add
            {
                lock (_lock)
                {
                    T source = GetSource();
                    if (source != null)
                    {
                        bool addHandler = _propChangedCallback == null;
                        _propChangedCallback += value;
                        if (addHandler)
                            PropertyChangedEventManager.AddListener(source, this, string.Empty);
                    }
                }
            }

            remove 
            {
                lock (_lock)
                {
                    _propChangedCallback -= value;
                    if (_propChangedCallback == null)
                    {
                        T source = GetSource();
                        if (source != null)
                            PropertyChangedEventManager.RemoveListener(source, this, string.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of PropertyObserver, which
        /// observes the 'propertySource' object for property changes.
        /// </summary>
        /// <param name="propertySource">The object to monitor for property changes.</param>
        public PropertyObserver(T propertySource)
        {
            if (propertySource == null)
                throw new ArgumentNullException("propertySource");

            _source = new WeakReference(propertySource);
            _propertyChangedToHandlerMap = new Dictionary<string, Action<T>>();
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

            var propertySource = GetSource();
            if (propertySource != null)
            {
                lock (_lock)
                {
                    _propertyChangedToHandlerMap[propertyName] = handler;
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

            T propertySource = GetSource();
            if (propertySource != null)
            {
                lock (_lock)
                {
                    if (_propertyChangedToHandlerMap.ContainsKey(propertyName))
                    {
                        _propertyChangedToHandlerMap.Remove(propertyName);
                        PropertyChangedEventManager.RemoveListener(propertySource, this, propertyName);
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Retrieves the property name for a given expression.
        /// </summary>
        /// <param name="expression">Expression to evaluate</param>
        /// <returns>Property name</returns>
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

        /// <summary>
        /// Returns the source object we are monitoring.
        /// </summary>
        /// <returns>Object if it is still alive.</returns>
        private T GetSource()
        {
            return (_source.IsAlive) ? (T)_source.Target : null;
        }

        /// <summary>
        /// Receives events from the centralized event manager.
        /// </summary>
        /// <returns>
        /// true if the listener handled the event. It is considered an error by the <see cref="T:System.Windows.WeakEventManager"/> handling in WPF to register a listener for an event that the listener does not handle. Regardless, the method should return false if it receives an event that it does not recognize or handle.
        /// </returns>
        /// <param name="managerType">The type of the <see cref="T:System.Windows.WeakEventManager"/> calling this method.</param><param name="sender">Object that originated the event.</param><param name="e">Event data.</param>
        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(PropertyChangedEventManager))
            {
                string propertyName = ((PropertyChangedEventArgs)e).PropertyName;
                var propertySource = (T)sender;

                // Spin through and call any "global" handlers
                var propertyChanged = _propChangedCallback;
                if (propertyChanged != null)
                {
                    propertyChanged.GetInvocationList()
                        .Cast<EventHandler<PropertyChangedEventArgs>>()
                        .ToList().ForEach(callback =>
                    {
                        try
                        {
                            callback.Invoke(sender, (PropertyChangedEventArgs) e);
                        }
                        catch
                        {
                            PropertyChanged -= callback;
                        }
                    });
                }

                // Now handle the property-specific changes
                if (String.IsNullOrEmpty(propertyName))
                {
                    // Get a safe copy of the list
                    List<Action<T>> entries;
                    lock (_lock)
                    {
                        entries = _propertyChangedToHandlerMap.Values.ToList();
                    }

                    // When the property name is empty, all properties are considered to be invalidated.
                    // Iterate over a copy of the list of handlers, in case a handler is registered by a callback.
                    entries.ForEach(handler => handler(propertySource));
                    return true;
                }

                Action<T> action = null;
                lock (_lock)
                {
                    _propertyChangedToHandlerMap.TryGetValue(propertyName, out action);
                }

                if (action != null)
                    action(propertySource);
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
                T propertySource = GetSource();
                if (propertySource != null)
                {
                    foreach (var propertyName in _propertyChangedToHandlerMap.Keys)
                        PropertyChangedEventManager.RemoveListener(propertySource, this, propertyName);

                    if (_propChangedCallback != null)
                        PropertyChangedEventManager.RemoveListener(propertySource, this, string.Empty);
                }
                _propertyChangedToHandlerMap.Clear();
                _source.Target = null;
                _propChangedCallback = null;
            }
        }

        #endregion
    }
}