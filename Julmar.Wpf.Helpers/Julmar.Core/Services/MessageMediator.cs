using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using JulMar.Core.Interfaces;
using System.ComponentModel.Composition;

namespace JulMar.Core.Services
{
    /// <summary>
    /// This class creates a simple Mediator which loosely connects different objects together. 
    /// The message handlers are organized using string-based message keys and are held in a WeakReference
    /// collection.
    /// </summary>
    [Export(typeof(IMessageMediator))]
    sealed class MessageMediator : IMessageMediator
    {
        /// <summary>
        /// This class creates a weak delegate of form Action(Of Object)
        /// </summary>
        internal class WeakAction
        {
            private readonly WeakReference _target;
            private readonly Type _ownerType;
            private readonly Type _actionType;
            private readonly string _methodName;

            public WeakAction(object target, Type actionType, MethodBase mi)
            {
                Debug.Assert((target == null && mi.IsStatic) || (target != null && !mi.IsStatic));

                if (target == null)
                    _ownerType = mi.DeclaringType;
                else
                    _target = new WeakReference(target);

                _methodName = mi.Name;
                _actionType = actionType;
            }

            public Type ActionType
            {
                get { return _actionType; }
            }

            public bool HasBeenCollected
            {
                get
                {
                    return (_ownerType == null && (_target == null || !_target.IsAlive));
                }
            }

            public Delegate GetMethod()
            {
                if (_ownerType != null)
                {
                    return Delegate.CreateDelegate(_actionType, _ownerType, _methodName);
                }

                if (_target != null && _target.IsAlive)
                {
                    object target = _target.Target;
                    if (target != null)
                        return Delegate.CreateDelegate(_actionType, target, _methodName);
                }

                return null;
            }
        }

        private readonly Dictionary<object, List<WeakAction>> _registeredHandlers = new Dictionary<object, List<WeakAction>>();

        /// <summary>
        /// This registers a Type with the mediator.  Any methods decorated with <seealso cref="MessageMediatorTargetAttribute"/> will be 
        /// registered as target method handlers for the given message key.
        /// </summary>
        /// <param name="view">Object to register</param>
        public void Register(object view)
        {
            // Look at all instance/static methods on this object type.
            foreach (var mi in view.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                // See if we have a target attribute - if so, register the method as a handler.
                foreach (var att in mi.GetCustomAttributes(typeof(MessageMediatorTargetAttribute), true))
                {
                    var mha = (MessageMediatorTargetAttribute)att;
                    var pi = mi.GetParameters();
                    
                    Type actionType;
                    if (pi.Length == 0)
                    {
                        actionType = typeof (Action);
                        if (mha.MessageKey == null)
                            throw new ArgumentNullException("MessageKey", "MessageKey must be supplied when using simple Action type");
                    }
                    else if (pi.Length == 1)
                        actionType = typeof(Action<>).MakeGenericType(pi[0].ParameterType);
                    else
                        throw new InvalidCastException("Cannot cast " + mi.Name + " to Action or Action<T> delegate type.");

                    object key = (mha.MessageKey) ?? actionType;

                    RegisterHandler(key, actionType,
                                    mi.IsStatic
                                        ? Delegate.CreateDelegate(actionType, mi)
                                        : Delegate.CreateDelegate(actionType, view, mi.Name));
                }
            }
        }

        /// <summary>
        /// This method unregisters a type from the message mediator.
        /// </summary>
        /// <param name="view">Object to unregister</param>
        public void Unregister(object view)
        {
            foreach (var mi in view.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                foreach (var att in mi.GetCustomAttributes(typeof(MessageMediatorTargetAttribute), true))
                {
                    var mha = (MessageMediatorTargetAttribute)att;
                    var pi = mi.GetParameters();

                    Type actionType;
                    if (pi.Length == 0)
                    {
                        actionType = typeof (Action);
                        if (mha.MessageKey == null)
                            throw new ArgumentNullException("MessageKey", "MessageKey must be supplied when using simple Action type");
                    }
                    else if (pi.Length == 1)
                        actionType = typeof(Action<>).MakeGenericType(pi[0].ParameterType);
                    else
                        throw new InvalidCastException("Cannot cast " + mi.Name + " to Action or Action<T> delegate type.");
                    
                    object key = (mha.MessageKey) ?? actionType;

                    UnregisterHandler(key, actionType,
                                      mi.IsStatic
                                          ? Delegate.CreateDelegate(actionType, mi)
                                          : Delegate.CreateDelegate(actionType, view, mi.Name));
                }
            }
        }

        /// <summary>
        /// Registers a specific method with no parameters as a handler
        /// </summary>
        /// <param name="key">Message key</param>
        /// <param name="handler">Handler method</param>
        public void RegisterHandler(string key, Action handler)
        {
            RegisterHandler(key, handler.GetType(), handler);
        }

        /// <summary>
        /// This registers a specific method as a message handler for a specific type.
        /// </summary>
        /// <param name="key">Message key</param>
        /// <param name="handler">Handler method</param>
        public void RegisterHandler<T>(string key, Action<T> handler)
        {
            RegisterHandler(key, handler.GetType(), handler);
        }

        /// <summary>
        /// This registers a specific method as a message handler for a specific type.
        /// </summary>
        /// <param name="handler">Handler method</param>
        public void RegisterHandler<T>(Action<T> handler)
        {
            RegisterHandler(typeof(Action<T>), handler.GetType(), handler);
        }

        /// <summary>
        /// Unregisters simple handler
        /// </summary>
        /// <param name="key">Message key</param>
        /// <param name="handler">Handler method</param>
        public void UnregisterHandler(string key, Action handler)
        {
            UnregisterHandler(key, handler.GetType(), handler);
        }

        /// <summary>
        /// This unregisters a method as a handler.
        /// </summary>
        /// <param name="key">Message key</param>
        /// <param name="handler">Handler</param>
        public void UnregisterHandler<T>(string key, Action<T> handler)
        {
            UnregisterHandler(key, handler.GetType(), handler);
        }

        /// <summary>
        /// This unregisters a method as a handler for a specific type
        /// </summary>
        /// <param name="handler">Handler</param>
        public void UnregisterHandler<T>(Action<T> handler)
        {
            UnregisterHandler(typeof(Action<T>), handler.GetType(), handler);
        }

        /// <summary>
        /// Performs the actual registration of a target
        /// </summary>
        /// <param name="key">Key to store in dictionary</param>
        /// <param name="actionType">Delegate type</param>
        /// <param name="handler">Method</param>
        private void RegisterHandler(object key, Type actionType, Delegate handler)
        {
            var action = new WeakAction(handler.Target, actionType, handler.Method);

            lock (_registeredHandlers)
            {
                List<WeakAction> wr;
                if (_registeredHandlers.TryGetValue(key, out wr))
                {
                    if (wr.Count > 0)
                    {
                        WeakAction wa = wr[0];
                        if (wa.ActionType != actionType &&
                            !wa.ActionType.IsAssignableFrom(actionType))
                            throw new ArgumentException("Invalid key passed to RegisterHandler - existing handler has incompatible parameter type");
                    }

                    wr.Add(action);
                }
                else
                {
                    wr = new List<WeakAction> { action };
                    _registeredHandlers.Add(key, wr);
                }
            }
        }

        /// <summary>
        /// Performs the unregistration from a target
        /// </summary>
        /// <param name="key">Key to store in dictionary</param>
        /// <param name="actionType">Delegate type</param>
        /// <param name="handler">Method</param>
        private void UnregisterHandler(object key, Type actionType, Delegate handler)
        {
            lock (_registeredHandlers)
            {
                List<WeakAction> wr;
                if (_registeredHandlers.TryGetValue(key, out wr))
                {
                    wr.RemoveAll(wa => handler == wa.GetMethod() && actionType == wa.ActionType);
                    if (wr.Count == 0)
                        _registeredHandlers.Remove(key);
                }
            }
        }

        /// <summary>
        /// This method broadcasts a message to all message targets for a given
        /// message key and passes a parameter.
        /// </summary>
        /// <param name="key">Message key</param>
        /// <param name="message">Message parameter</param>
        /// <returns>True/False if any handlers were invoked.</returns>
        private bool SendMessage(object key, object message)
        {
            List<WeakAction> wr;
            lock (_registeredHandlers)
            {
                if (!_registeredHandlers.TryGetValue(key, out wr))
                    return false;

                // Make a copy while locked
                wr = wr.ToList();
            }

            bool foundService = false;
            foreach (var action in wr.Select(cb => cb.GetMethod()).Where(action => action != null))
            {
                action.DynamicInvoke(message);
                foundService = true;
            }

            lock (_registeredHandlers)
            {
                wr.RemoveAll(wa => wa.HasBeenCollected);
            }

            return foundService;
        }

        /// <summary>
        /// This method broadcasts a message with no parameters
        /// </summary>
        /// <param name="key">Message key</param>
        /// <returns>True/False if any handlers were invoked.</returns>
        private bool SendSimpleMessage(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            List<WeakAction> wr;
            lock (_registeredHandlers)
            {
                if (!_registeredHandlers.TryGetValue(key, out wr))
                    return false;

                // Make a copy while locked.
                wr = wr.ToList();
            }

            bool foundService = false;
            foreach (var action in wr.Select(cb => cb.GetMethod()).OfType<Action>())
            {
                action.Invoke();
                foundService = true;
            }

            lock (_registeredHandlers)
            {
                wr.RemoveAll(wa => wa.HasBeenCollected);
            }

            return foundService;
        }

        /// <summary>
        /// This method broadcasts a message to all message targets for a given
        /// message key and passes a parameter.
        /// </summary>
        /// <param name="key">Message key</param>
        /// <param name="message">Message parameter</param>
        /// <returns>True/False if any handlers were invoked.</returns>
        public bool SendMessage<T>(string key, T message)
        {
            return SendMessage((object)key, message);
        }

        /// <summary>
        /// This method broadcasts a message to all message targets for a given parameter type.
        /// If a derived type is passed, any handlers for interfaces or base types will also be
        /// invoked.
        /// </summary>
        /// <param name="message">Message parameter</param>
        /// <returns>True/False if any handlers were invoked.</returns>
        public bool SendMessage<T>(T message)
        {
            bool foundService = false;
            if (typeof(T) == typeof(string))
                foundService = SendSimpleMessage(message as string);

            Type actionType = typeof(Action<>).MakeGenericType(typeof(T));

            List<object> keyList;
            lock (_registeredHandlers)
            {
                keyList = _registeredHandlers.Keys.Where(key => key is Type && ( ((Type)key).IsAssignableFrom(actionType) || actionType.IsAssignableFrom(((Type)key))) ).ToList();
            }

            return keyList.Aggregate(false, (current, key) => current | SendMessage(key, message)) || foundService;
        }

        /// <summary>
        /// This method broadcasts a message to all message targets for a given
        /// message key and passes a parameter.  The message targets are all called
        /// asynchronously and any resulting exceptions are ignored.
        /// </summary>
        /// <param name="key">Message key</param>
        /// <param name="message">Message parameter</param>
        public void SendMessageAsync<T>(string key, T message)
        {
            Func<string, T, bool> smaFunc = SendMessage;
            smaFunc.BeginInvoke(key, message, ia =>
                  {
                      try { smaFunc.EndInvoke(ia); }
                      catch { }
                  }, null);
        }

        /// <summary>
        /// This method broadcasts a message to all message targets for a given parameter type.
        /// If a derived type is passed, any handlers for interfaces or base types will also be
        /// invoked.  The message targets are all called asynchronously and any resulting exceptions
        /// are ignored.
        /// </summary>
        /// <param name="message">Message parameter</param>
        public void SendMessageAsync<T>(T message)
        {
            Func<T, bool> smaFunc = SendMessage;
            smaFunc.BeginInvoke(message, ia =>
                {
                    try { smaFunc.EndInvoke(ia); } 
                    catch { } 
                }, null);
        }
    }
}