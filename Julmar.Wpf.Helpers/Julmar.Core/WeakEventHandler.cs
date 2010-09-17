using System;

namespace JulMar.Core
{
    /// <summary>
    /// Simple class to generate WeakEventHandler delegates.  This is modeled
    /// after the WeakEventManager support in WPF but is generic (not dependent upon that class)
    /// Warning: this will not work properly for anonymous methods.
    /// </summary>
    /// <example>
    /// <![CDATA[
    ///   staticClass.SomeEvent += WeakEventHandler(MyHandlerMethod, e => staticClass.SomeEvent -= e);
    ///   staticClass.SomeEvent += WeakEventHandler<MyEventArgs>.Create(MyHandlerMethod);
    /// ]]>
    /// </example>
    public static class WeakEventHandler
    {
        /// <summary>
        /// Creates a new EventHandler wrapper over an existing method that exhibits
        /// WeakEvent semantics.  I.e. the event will not keep the specified target alive
        /// if no other references are outstanding.  This version is for EventHandler.
        /// </summary>
        /// <param name="eventHandler">Method to wrap</param>
        /// <param name="removeTarget">Delegate owner</param>
        /// <returns>EventHandler (weak)</returns>
        public static EventHandler Create(EventHandler eventHandler, Action<EventHandler> removeTarget = null)
        {
            if (eventHandler == null)
                throw new ArgumentNullException("eventHandler");
            if (eventHandler.Method.IsStatic || eventHandler.Target == null)
                throw new ArgumentException("Cannot wrap static methods", "eventHandler");

            Type wehType = typeof(InternalWeakEventHandler<>).MakeGenericType(eventHandler.Method.DeclaringType);
            return ((IWeakEventHandler) Activator.CreateInstance(wehType, new object[] {eventHandler, removeTarget})).Handler;
        }

        /// <summary>
        /// Creates a new EventHandler wrapper over an existing method that exhibits
        /// WeakEvent semantics.  I.e. the event will not keep the specified target alive
        /// if no other references are outstanding.  This version is for EventHandler(OfType(T)).
        /// </summary>
        /// <typeparam name="T">Generic EventArgs type</typeparam>
        /// <param name="eventHandler">Method to wrap</param>
        /// <param name="removeTarget">Delegate owner</param>
        /// <returns>EventHandler (weak)</returns>
        public static EventHandler<T> Create<T>(EventHandler<T> eventHandler, Action<EventHandler<T>> removeTarget = null) where T : EventArgs
        {
            if (eventHandler == null)
                throw new ArgumentNullException("eventHandler");
            if (eventHandler.Method.IsStatic || eventHandler.Target == null)
                throw new ArgumentException("Cannot wrap static methods", "eventHandler");

            Type wehType = typeof(InternalWeakEventHandler<,>).MakeGenericType(eventHandler.Method.DeclaringType, typeof(T));
            return ((IWeakEventHandler<T>) Activator.CreateInstance(wehType, new object[] {eventHandler, removeTarget})).Handler;
        }
    }

    /// <summary>
    /// Interface for non-generic event handler type
    /// </summary>
    internal interface IWeakEventHandler
    {
        EventHandler Handler { get; }
    }

    /// <summary>
    /// This class implements the WeakEvent handler
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    internal sealed class InternalWeakEventHandler<T> : IWeakEventHandler where T : class
    {
        private readonly Action<EventHandler> _removeTarget;
        private readonly WeakReference _targetRef;
        private readonly Action<T, object, EventArgs> _methodInvoker;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventHandler">EventHandler</param>
        /// <param name="removeTarget">Delegate owner</param>
        public InternalWeakEventHandler(EventHandler eventHandler, Action<EventHandler> removeTarget)
        {
            _removeTarget = removeTarget;
            _targetRef = new WeakReference(eventHandler.Target);
            _methodInvoker = (Action<T, object, EventArgs>)Delegate.CreateDelegate(typeof(Action<T, object, EventArgs>), null, eventHandler.Method);
        }

        /// <summary>
        /// Method to invoke the wrapped weak method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Invoke(object sender, EventArgs e)
        {
            // Retrieve the real method.  If it's still alive, then invoke it.
            var target = (T)_targetRef.Target;
            if (target != null)
            {
                _methodInvoker.Invoke(target, sender, e);
            }
            // Unsubscribe from the delegate itself.
            else
            {
                if (_removeTarget != null)
                    _removeTarget(Invoke);
            }
        }

        /// <summary>
        /// Retrieves the handler
        /// </summary>
        EventHandler IWeakEventHandler.Handler
        {
            get { return Invoke; }
        }
    }

    /// <summary>
    /// Interface to allow compile-time binding to generated handler.
    /// </summary>
    /// <typeparam name="T">EventArgs type</typeparam>
    internal interface IWeakEventHandler<T> where T : EventArgs
    {
        /// <summary>
        /// Retrieves the handler
        /// </summary>
        EventHandler<T> Handler { get; }
    }

    /// <summary>
    /// This class implements the WeakEvent handler
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <typeparam name="TE">EventArgs</typeparam>
    internal sealed class InternalWeakEventHandler<T, TE> : IWeakEventHandler<TE>
        where T : class 
        where TE : EventArgs
    {
        private readonly Action<EventHandler<TE>> _removeTarget;
        private readonly WeakReference _targetRef;
        private readonly Action<T,object,TE> _methodInvoker;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventHandler">EventHandler</param>
        /// <param name="removeTarget">Action called on removal</param>
        public InternalWeakEventHandler(EventHandler<TE> eventHandler, Action<EventHandler<TE>> removeTarget)
        {
            _removeTarget = removeTarget;
            _targetRef = new WeakReference(eventHandler.Target);
            _methodInvoker = (Action<T, object, TE>) Delegate.CreateDelegate(typeof(Action<T, object, TE>), null, eventHandler.Method);
        }

        /// <summary>
        /// Method to invoke the wrapped weak method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Invoke(object sender, TE e)
        {
            // Retrieve the real method.  If it's still alive, then invoke it.
            var target = (T)_targetRef.Target;
            if (target != null)
                _methodInvoker.Invoke(target, sender, e);
            // Unsubscribe from the delegate itself.
            else
            {
                if (_removeTarget != null)
                    _removeTarget(Invoke);
            }
        }

        /// <summary>
        /// Retrieves the handler
        /// </summary>
        EventHandler<TE> IWeakEventHandler<TE>.Handler
        {
            get { return Invoke; }
        }
    }
}
