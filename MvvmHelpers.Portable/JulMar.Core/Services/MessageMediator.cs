using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;

namespace JulMar.Services
{
    /// <summary>
    /// This class creates a simple Mediator which loosely connects different objects together. 
    /// The message handlers are organized using string-based message keys and are held in a WeakReference
    /// collection.
    /// </summary>
    public sealed class MessageMediator : IMessageMediator
    {
        private static IMessageMediator instance;
        private readonly Dictionary<object, List<WeakAction>> registeredHandlers = new Dictionary<object, List<WeakAction>>();
 
        /// <summary>
        /// Global instance of message mediator, can also create local versions for private messaging.
        /// </summary>
        public static IMessageMediator Instance
        {
            get
            {
                return instance ?? (instance = new MessageMediator());
            }
        }

        /// <summary>
        /// Can be used to set the message mediator instance; this is useful to replace
        /// the built-in system service.
        /// </summary>
        /// <param name="mediator">Mediator to use.</param>
        public static void SetMessageMediator(IMessageMediator mediator)
        {
            if (instance != null)
                throw new InvalidOperationException("Can only set the MessageMediator once.");
            instance = mediator;
        }

        /// <summary>
        /// This class creates a weak delegate of form Action(Of Object)
        /// </summary>
        internal class WeakAction
        {
            private readonly WeakReference _target;
            private readonly Type _ownerType;
            private readonly Type _actionType;
            private readonly MethodInfo _methodInfo;

            public WeakAction(Delegate work)
            {
                object target = work.Target; 
                MethodInfo mi = work.GetMethodInfo();

                Debug.Assert((target == null && mi.IsStatic) || (target != null && !mi.IsStatic));

                if (target == null)
                    _ownerType = mi.DeclaringType;
                else
                    _target = new WeakReference(target);

                _methodInfo = mi;
                _actionType = work.GetType();
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
                    return _methodInfo.CreateDelegate(_actionType, _ownerType);
                }

                if (_target != null && _target.IsAlive)
                {
                    object target = _target.Target;
                    if (target != null)
                        return _methodInfo.CreateDelegate(_actionType, target);
                }

                return null;
            }
        }

        /// <summary>
        /// Used to unsubscribe handlers.
        /// </summary>
        internal class WeakActionKey : IDisposable
        {
            readonly MessageMediator _mediator;
            readonly object _key;
            readonly WeakAction _handler;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="key">Key used for subscription</param>
            /// <param name="handler">Handler</param>
            /// <param name="mediator">Mediator registered with</param>
            public WeakActionKey(object key, WeakAction handler, MessageMediator mediator)
            {
                this._key = key;
                this._handler = handler;
                this._mediator = mediator;
            }

            /// <summary>
            /// Unregister the handler.
            /// </summary>
            void IDisposable.Dispose()
            {
                _mediator.InternalUnregisterHandler(_key, _handler);
            }
        }

        /// <summary>
        /// Registers a specific method with no parameters as a handler
        /// </summary>
        /// <param name="key">Message key</param>
        /// <param name="handler">Handler method</param>
        public IDisposable Subscribe(string key, Action handler)
        {
            return InternalRegisterHandler(key, handler);
        }

        /// <summary>
        /// This registers a specific method as a message handler for a specific type.
        /// </summary>
        /// <param name="key">Message key</param>
        /// <param name="handler">Handler method</param>
        public IDisposable Subscribe<T>(string key, Action<T> handler)
        {
            return InternalRegisterHandler(key, handler);
        }

        /// <summary>
        /// This registers a specific method as a message handler for a specific type.
        /// </summary>
        /// <param name="handler">Handler method</param>
        public IDisposable Subscribe<T>(Action<T> handler)
        {
            return InternalRegisterHandler(typeof(Action<T>), handler);
        }

        /// <summary>
        /// Performs the actual registration of a target
        /// </summary>
        /// <param name="key">Key to store in dictionary</param>
        /// <param name="handler">Method</param>
        private IDisposable InternalRegisterHandler(object key, Delegate handler)
        {
            Type actionType = handler.GetType();
            var action = new WeakAction(handler);

            lock (this.registeredHandlers)
            {
                List<WeakAction> wr;
                if (this.registeredHandlers.TryGetValue(key, out wr))
                {
                    if (wr.Count > 0)
                    {
                        WeakAction wa = wr[0];
                        if (wa.ActionType != actionType &&
                            !wa.ActionType.GetTypeInfo().IsAssignableFrom(actionType.GetTypeInfo()))
                            throw new ArgumentException("Invalid key passed to RegisterHandler - existing handler has incompatible parameter type");
                    }

                    wr.Add(action);
                }
                else
                {
                    wr = new List<WeakAction> { action };
                    this.registeredHandlers.Add(key, wr);
                }
            }

            return new WeakActionKey(key, action, this);
        }

        /// <summary>
        /// Performs the unregistration from a target
        /// </summary>
        /// <param name="key">Key to store in dictionary</param>
        /// <param name="handler">Method</param>
        private void InternalUnregisterHandler(object key, WeakAction handler)
        {
            lock (this.registeredHandlers)
            {
                List<WeakAction> wr;
                if (this.registeredHandlers.TryGetValue(key, out wr))
                {
                    wr.RemoveAll(wa => ReferenceEquals(handler, wa));
                    if (wr.Count == 0)
                        this.registeredHandlers.Remove(key);
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
            lock (this.registeredHandlers)
            {
                if (!this.registeredHandlers.TryGetValue(key, out wr))
                    return false;

                // Make a copy while locked
                wr = wr.ToList();
            }

            bool foundHandler = false;

            foreach (var action in wr.Select(cb => cb.GetMethod()).Where(action => action != null))
            {
                action.DynamicInvoke(message);
                foundHandler = true;
            }

            lock (this.registeredHandlers)
            {
                wr.RemoveAll(wa => wa.HasBeenCollected);
            }

            return foundHandler;
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
            lock (this.registeredHandlers)
            {
                if (!this.registeredHandlers.TryGetValue(key, out wr))
                    return false;

                // Make a copy while locked.
                wr = wr.ToList();
            }

            bool foundHandler = false;
            foreach (var action in wr.Select(cb => cb.GetMethod()).OfType<Action>())
            {
                action.Invoke();
                foundHandler = true;
            }

            lock (this.registeredHandlers)
            {
                wr.RemoveAll(wa => wa.HasBeenCollected);
            }

            return foundHandler;
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
            bool foundHandler = false;

            // If it's just a string, it might be the key with no parameters.
            // Attempt to locate a method by key
            if (typeof(T) == typeof(string))
                foundHandler = SendSimpleMessage(message as string);

            Type actionType = typeof(Action<>).MakeGenericType(typeof(T));

            List<object> keyList;
            lock (this.registeredHandlers)
            {
                keyList = this.registeredHandlers.Keys.Where(key => key is Type && ( ((Type)key).GetTypeInfo().IsAssignableFrom(actionType.GetTypeInfo()) 
                    || actionType.GetTypeInfo().IsAssignableFrom((((Type)key).GetTypeInfo()))) ).ToList();
            }

            return keyList.Aggregate(false, (current, key) => current | SendMessage(key, message)) || foundHandler;
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