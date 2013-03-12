using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using JulMar.Core.Internal;
using JulMar.Core.Services;
using System.Reflection;
using JulMar.Windows.Interfaces;

namespace JulMar.Windows.Mvvm
{
    /// <summary>
    /// This class holds ViewModels that are registered with the ExportViewModelAttribute.
    /// </summary>
    [DefaultExport(typeof(IViewModelLocator)), Shared]
    sealed class ViewModelLocatorService : IViewModelLocator
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

                        Debug.Assert(entry != null);
                        Debug.Assert(entry.IsValueCreated == false);
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
    }

    /// <summary>
    /// Class used to populate metadata used to identify view models
    /// </summary>
    public sealed class ViewModelMetadata
    {
        /// <summary>
        /// Key used to export the ViewModel.  We only allow one export for VMs.
        /// </summary>
        public string Key { get; set; }
    }

    /// <summary>
    /// Class used to locate view models but keep property hidden
    /// </summary>
    internal sealed class ViewModelData
    {
        /// <summary>
        /// Located view models
        /// </summary>
        [ImportMany(ViewModelLocatorService.MefLocatorKey)]
        public IList<Lazy<object, ViewModelMetadata>> LocatedViewModels { get; set; }
    }
}
