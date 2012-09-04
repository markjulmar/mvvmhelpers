using System;
using System.Collections;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using JulMar.Core.Internal;
using JulMar.Core.Services;
using JulMar.Core.Interfaces;
using System.Reflection;

namespace JulMar.Windows.Mvvm.Internal
{
    /// <summary>
    /// This class holds ViewModels that are registered with the ExportViewModelAttribute.
    /// </summary>
    [DefaultExport(typeof(IViewModelLocator)), Shared]
    sealed class ViewModelLocatorImpl : IViewModelLocator
    {
        private IList<Lazy<object, ViewModelMetadata>> _locatedViewModels;

        /// <summary>
        /// Key used to bind exports together
        /// </summary>
        internal const string MefLocatorKey = "JulMar.ViewModel.Export";

        /// <summary>
        /// Operator to retrieve view models.
        /// </summary>
        /// <returns>Read-only version of view model collection</returns>
        public object this[string key]
        {
            get 
            { 
                object value;
                return TryLocate(key, out value) ? value : null;
            }
        }

        /// <summary>
        /// Finds the VM based on the key.
        /// </summary>
        /// <param name="key">Key to search for</param>
        /// <returns>Located view model or null</returns>
        public object Locate(string key)
        {
            object value;
            if (!TryLocate(key, out value))
                throw new Exception("Could not locate view model: " + key);
            
            return value;
        }

        /// <summary>
        /// Finds the VM based on the key.
        /// </summary>
        /// <param name="key">Key to search for</param>
        /// <param name="returnValue">Located view model or null</param>
        /// <returns>true/false if VM was found</returns>
        public bool TryLocate(string key, out object returnValue)
        {
            returnValue = null;

            // Populate our list the first call
            if (_locatedViewModels == null)
            {
                _locatedViewModels = GatherViewModelData();
            }

            // First look for the key in our metadata collection
            var vmType = _locatedViewModels.FirstOrDefault(vm => vm.Metadata.Key == key);
            if (vmType != null)
            {
                // First time?  Just create it and return
                if (!vmType.IsValueCreated)
                {
                    returnValue = vmType.Value;
                }
                else
                {
                    // Object should already be there.
                    Type type = vmType.Value.GetType();

                    // Parts in MEF Win8 are always non-shared, so look for new SharedAttribute to indicate they
                    // are shared.
                    var attributes = type.GetTypeInfo().CustomAttributes.Where(a => a.AttributeType  == typeof(SharedAttribute)).ToArray();
                    if (attributes.Length == 0)
                    {
                        // Attempt to create a brand new one.
                        // No easy way to do this because Lazy<T> always returns same instance above
                        // so, non-shared instances are not possible .. and since we are exporting as typeof(object)
                        // to gather the VMs (otherwise the above ImportMany doesn't work) we can't differentiate based
                        // on type and use MEF to recreate one.
                        var locatedVms = GatherViewModelData();
                        var entry = locatedVms.First(vmd => vmd.Metadata.Key == key);
                        returnValue = entry.Value;
                    }
                    else 
                        returnValue = vmType.Value;
                }
            }

            return returnValue != null;
        }

        /// <summary>
        /// This method uses an internal object to gather the list of ViewModels based
        /// on the ExportViewModel attribute.
        /// </summary>
        /// <returns></returns>
        private static IList<Lazy<object, ViewModelMetadata>> GatherViewModelData()
        {
            var data = new ViewModelData();
            DynamicComposer.Instance.Compose(data);
            return data.LocatedViewModels;
        }

        /// <summary>
        /// Dictionary based on a delegate function. Note that this does
        /// not support enumeration of keys or values - it only supports
        /// read-only retrieval of dynamic keys.
        /// </summary>
        /// <typeparam name="TK">Key type</typeparam>
        /// <typeparam name="TV">Value type</typeparam>
        internal class DelegateDictionary<TK,TV> : IReadOnlyDictionary<TK,TV>
        {
            private readonly Func<TK, TV> _getValue;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="getValueFunc">Function to retrieve key/value pairs</param>
            internal DelegateDictionary(Func<TK,TV> getValueFunc)
            {
                _getValue = getValueFunc;
            }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
            /// </returns>
            public IEnumerator<KeyValuePair<TK, TV>> GetEnumerator()
            {
                yield break;
            }

            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>
            /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
            /// </returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                yield break;
            }

            /// <summary>
            /// Gets the number of elements in the collection.
            /// </summary>
            /// <returns>
            /// The number of elements in the collection. 
            /// </returns>
            public int Count { get { return 0; } }

            /// <summary>
            /// Determines whether the read-only dictionary contains an element that has the specified key.
            /// </summary>
            /// <returns>
            /// true if the read-only dictionary contains an element that has the specified key; otherwise, false.
            /// </returns>
            /// <param name="key">The key to locate.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
            public bool ContainsKey(TK key)
            {
                return false;
            }

            /// <summary>
            /// Gets the value that is associated with the specified key.
            /// </summary>
            /// <returns>
            /// true if the object that implements the <see cref="T:System.Collections.Generic.IReadOnlyDictionary`2"/> interface contains an element that has the specified key; otherwise, false.
            /// </returns>
            /// <param name="key">The key to locate.</param><param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
            public bool TryGetValue(TK key, out TV value)
            {
                value = default(TV);
                return false;
            }

            /// <summary>
            /// Gets the element that has the specified key in the read-only dictionary.
            /// </summary>
            /// <returns>
            /// The element that has the specified key in the read-only dictionary.
            /// </returns>
            /// <param name="key">The key to locate.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key"/> is not found. </exception>
            public TV this[TK key]
            {
                get { return _getValue(key); }
            }

            /// <summary>
            /// Gets an enumerable collection that contains the keys in the read-only dictionary. 
            /// </summary>
            /// <returns>
            /// An enumerable collection that contains the keys in the read-only dictionary.
            /// </returns>
            public IEnumerable<TK> Keys { get { yield break; } }

            /// <summary>
            /// Gets an enumerable collection that contains the values in the read-only dictionary.
            /// </summary>
            /// <returns>
            /// An enumerable collection that contains the values in the read-only dictionary.
            /// </returns>
            public IEnumerable<TV> Values { get { yield break; } }
        }
    }

    /// <summary>
    /// Interface used to populate metadata we use for services.
    /// </summary>
    public sealed class ViewModelMetadata
    {
        /// <summary>
        /// Key used to export the ViewModel.  We only allow one export for VMs.
        /// </summary>
        public string Key { get; set; }
    }

    /// <summary>
    /// Class used to locate view models but keep property semi-hidden
    /// </summary>
    public sealed class ViewModelData
    {
        /// <summary>
        /// Located view models
        /// </summary>
        [ImportMany(ViewModelLocatorImpl.MefLocatorKey)]
        public IList<Lazy<object, ViewModelMetadata>> LocatedViewModels { get; set; }
    }
}
