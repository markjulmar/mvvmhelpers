using System.Collections.Generic;
using System.Linq;
using JulMar.Core.Extensions;

#if NET35
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Simple implementation of the .NET 4.0 ConditionalWeakTable.
    /// Note that this implementation cannot support circular references.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ConditionalWeakTable<TKey, TValue> 
        where TKey : class
        where TValue : class
    {
        private readonly Dictionary<object, TValue> _table = new Dictionary<object, TValue>();

        /// <summary>
        /// Adds a new key into the dictionary
        /// </summary>
        /// <param name="key">Object owner</param>
        /// <param name="value">Value</param>
        public void Add(TKey key, TValue value)
        {
            _table.Keys
                .Where(weakRef => !((EquivalentWeakReference)weakRef).IsAlive)
                .ToArray()
                .ForEach(o => _table.Remove(o));

            _table.Add(new EquivalentWeakReference(key), value);
        }

        /// <summary>
        /// Removes a key from the dictionary
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Success/Fail</returns>
        public bool Remove(TKey key)
        {
            return _table.Remove(key);
        }

        /// <summary>
        /// Method to retrieve the associated key/value.
        /// </summary>
        /// <param name="key">Object owner</param>
        /// <param name="value">Returning value</param>
        /// <returns>Success/Fail</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _table.TryGetValue(key, out value);
        }

        /// <summary>
        /// Method to retrieve/create the associated key/value
        /// </summary>
        /// <param name="key">Object owner</param>
        /// <param name="creatorFunc">Creation delegate</param>
        /// <returns>Value</returns>
        public TValue GetValue(TKey key, Func<TKey, TValue> creatorFunc)
        {
            TValue value;
            if (TryGetValue(key, out value))
                return value;

            value = creatorFunc(key);
            if (value != default(TValue))
                Add(key, value);

            return value;
        }

        /// <summary>
        /// Method to retrieve/create the associated key/value
        /// </summary>
        /// <param name="key">Object owner</param>
        /// <returns>Value</returns>
        public TValue GetOrCreateValue(TKey key)
        {
            TValue value;
            if (TryGetValue(key, out value))
                return value;

            value = (TValue) Activator.CreateInstance(typeof(TValue));
            Add(key, value);
            return value;
        }

        /// <summary>
        /// Internal WeakReference with GetHashCode duplication 
        /// for internally held object. 
        /// </summary>
        private class EquivalentWeakReference
        {
            private readonly WeakReference _value;
            private readonly int _hashCode;

            public EquivalentWeakReference(object obj)
            {
                _value = new WeakReference(obj);
                _hashCode = obj.GetHashCode();
            }

                        public override int GetHashCode()
            {
                return _hashCode;
            }

            public bool IsAlive
            {
                get { return _value.IsAlive; }
            }

            public override bool Equals(object obj)
            {
                var weakRef = obj as EquivalentWeakReference;
                if (weakRef != null)
                    obj = weakRef._value.Target;

                return obj == null 
                    ? base.Equals(weakRef) 
                    : Equals(_value.Target, obj);
            }
        }
    }
}
#endif
