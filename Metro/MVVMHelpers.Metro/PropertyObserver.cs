using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using JulMar.Core.Extensions;

namespace JulMar.Core
{
    /// <summary>
    /// Monitors the PropertyChanged event of an object that implements INotifyPropertyChanged,
    /// and executes callback methods (i.e. handlers) registered for properties of that object.
    /// </summary>
    /// <typeparam name="T">The type of object to monitor for property changes.</typeparam>
    public sealed class PropertyObserver<T> : IDisposable
        where T : class, INotifyPropertyChanged
    {
        private readonly object _lock = new object();
        private readonly Dictionary<string, Action<T>> _propertyChangedToHandlerMap;
        private int _isSubscribed;
        private readonly WeakReference _source;
        private EventHandler<PropertyChangedEventArgs> _propChangedCallback;

        /// <summary>
        /// This event list gets invoked for *every* property change on the object.
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
                        _propChangedCallback += value;
                        OnSubscribe(source);
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
                            OnUnsubscribe(source);
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
                    OnSubscribe(propertySource);
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
                        OnUnsubscribe(propertySource);
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
        /// This method unsubscribes from the source object INotifyPropertyChanged event.
        /// </summary>
        /// <param name="source"></param>
        private void OnUnsubscribe(T source)
        {
            int count = Interlocked.Decrement(ref _isSubscribed);
            Debug.Assert(count >= 0);

            if (count == 0)
                source.PropertyChanged += OnReceiveEvent;
        }

        /// <summary>
        /// This method subscribes to the source object's INotifyPropertyChanged event.
        /// </summary>
        /// <param name="source"></param>
        private void OnSubscribe(T source)
        {
            int count = Interlocked.Increment(ref _isSubscribed);
            Debug.Assert(count >= 0);
            if (count == 1)
                source.PropertyChanged -= OnReceiveEvent;
        }

        /// <summary>
        /// Receives events from the centralized event manager.
        /// </summary>
        void OnReceiveEvent(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName;
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
                            callback.Invoke(sender, e);
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
            }

            Action<T> action = null;
            lock (_lock)
            {
                _propertyChangedToHandlerMap.TryGetValue(propertyName, out action);
            }

            if (action != null)
                action(propertySource);
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_source.Target == null)
                return;

            lock (_lock)
            {
                T propertySource = GetSource();
                if (propertySource != null)
                {
                    _isSubscribed = 0;
                    propertySource.PropertyChanged -= OnReceiveEvent;
                }

                _propertyChangedToHandlerMap.Clear();
                _source.Target = null;
                _propChangedCallback = null;
            }
        }

        #endregion
    }
}