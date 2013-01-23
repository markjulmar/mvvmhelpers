using System;
using System.Reflection;

namespace JulMar.Core
{
    /// <summary>
    /// WeakDelegate holds a WeakReference to a delegate type.
    /// </summary>
    public class WeakDelegate : IEquatable<Delegate>
    {
        private readonly WeakReference _targetReference;
        private readonly Type _delegateType;
        private readonly MethodInfo _method;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="existingDelegate">Existing delegate</param>
        public WeakDelegate(Delegate existingDelegate)
        {
            if (existingDelegate == null)
                throw new ArgumentNullException("existingDelegate");

            _delegateType = existingDelegate.GetType();
            _method = existingDelegate.Method;
            _targetReference = existingDelegate.Target != null
                ? new WeakReference(existingDelegate.Target) 
                : null;
        }

        /// <summary>
        /// True if the underlying reference is still alive, or the delegate points to 
        /// a static method, False if the reference has been collected.
        /// </summary>
        public bool IsAlive
        {
            get
            {
                return _targetReference == null
                       || _targetReference.IsAlive;
            }
        }

        /// <summary>
        /// Gets the delegate if the reference still exists, or returns null if the reference has been collected.
        /// </summary>
        /// <returns>Live delegate or null</returns>
        public Delegate GetDelegate()
        {
            return _targetReference != null
                       ? Delegate.CreateDelegate(_delegateType, _targetReference.Target, _method)
                       : Delegate.CreateDelegate(_delegateType, _method);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Delegate other)
        {
            return other != null
                && other.GetType() == _delegateType
                && other.Method.Equals(_method)
                && ((other.Target == null && _targetReference == null) || (_targetReference != null && other.Target == _targetReference.Target));
        }
    }

    /// <summary>
    /// A typed WeakDelegate
    /// </summary>
    /// <typeparam name="T">Delegate type</typeparam>
    public class WeakDelegate<T> : IEquatable<Delegate>
    {
        private readonly WeakReference _targetReference;
        private readonly MethodInfo _method;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="existingDelegate">Existing delegate</param>
        public WeakDelegate(Delegate existingDelegate)
        {
            if (existingDelegate == null)
                throw new ArgumentNullException("existingDelegate");

            _method = existingDelegate.Method;
            _targetReference = existingDelegate.Target != null
                ? new WeakReference(existingDelegate.Target)
                : null;
        }

        /// <summary>
        /// True if the underlying reference is still alive, or the delegate points to 
        /// a static method, False if the reference has been collected.
        /// </summary>
        public bool IsAlive
        {
            get
            {
                return _targetReference == null
                       || _targetReference.IsAlive;
            }
        }

        /// <summary>
        /// Gets the delegate if the reference still exists, or returns null if the reference has been collected.
        /// </summary>
        /// <returns>Live delegate or null</returns>
        public Delegate GetDelegate()
        {
            return _targetReference != null
                       ? Delegate.CreateDelegate(typeof(T), _targetReference.Target, _method)
                       : Delegate.CreateDelegate(typeof(T), _method);
        }

        /// <summary>
        /// Gets the delegate if the reference still exists, or returns null if the reference has been collected.
        /// </summary>
        /// <returns>Typed delegate</returns>
        public T GetTypedDelegate()
        {
            return (T) (object) GetDelegate();
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Delegate other)
        {
            return other != null
                && other.GetType() == typeof(T)
                && other.Method.Equals(_method)
                && ((other.Target == null && _targetReference == null) || (_targetReference != null && other.Target == _targetReference.Target));
        }
    }
}