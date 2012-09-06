using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using JulMar.Core.Services;
using JulMar.Windows.Interfaces;
using System.Diagnostics;

namespace JulMar.Windows.Mvvm
{
    /// <summary>
    /// Class used to populate metadata used to identify view models
    /// </summary>
    public interface IViewModelMetadata
    {
        /// <summary>
        /// Key(s) used to export the ViewModel.
        /// </summary>
        string[] Key { get; }

        /// <summary>
        /// The type being exported
        /// </summary>
        string ExportTypeIdentity { get; }
    }

    /// <summary>
    /// Class used to locate view models but keep property semi-hidden
    /// </summary>
    internal sealed class ViewModelData
    {
        /// <summary>
        /// Located view models
        /// </summary>
        [ImportMany(ViewModelLocator.MefLocatorKey, AllowRecomposition = true)]
        public IEnumerable<Lazy<object, IViewModelMetadata>> LocatedViewModels { get; set; }
    }

    /// <summary>
    /// This attribute is used to decorate all "auto-located" view models.
    /// MEF is used to locate and bind each VM with this attribute decoration.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ExportViewModelAttribute : ExportAttribute
    {
        /// <summary>
        /// Key used to export the View/ViewModel
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">Key used to lookup ViewModel</param>
        public ExportViewModelAttribute(string key) : base(ViewModelLocator.MefLocatorKey)
        {
            Key = key;
        }
    }

    /// <summary>
    /// This class holds ViewModels that are registered with the ExportViewModelAttribute.
    /// </summary>
    [Export(typeof(IViewModelLocator))]
    sealed class ViewModelLocator : IViewModelLocator
    {
        private IList<Lazy<object, IViewModelMetadata>> _locatedViewModels;

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
                _locatedViewModels = GatherViewModelData().ToList();
            }

            // First look for the key in our metadata collection
            var vmType = (from locatedVm in _locatedViewModels
                          where locatedVm.Metadata.Key.Any(uiKey => uiKey == key)
                          select locatedVm).FirstOrDefault();

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

                    // Look for the shared attribute, MEF parts are shared by default
                    var pca = type.GetCustomAttributes(typeof (PartCreationPolicyAttribute), true).Cast<PartCreationPolicyAttribute>().ToArray();
                    if (pca.Any(cp => cp.CreationPolicy == CreationPolicy.NonShared))
                    {
                        // Attempt to create a brand new one.
                        // No easy way to do this because Lazy<T> always returns same instance above
                        // so, non-shared instances are not possible .. and since we are exporting as typeof(object)
                        // to gather the VMs (otherwise the above ImportMany doesn't work) we can't differentiate based
                        // on type and use MEF to recreate one.
                        var locatedVms = GatherViewModelData();
                        var entry = locatedVms.First(vmd => vmd.Metadata.Key.Any(uiKey => uiKey == key));
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
        private static IEnumerable<Lazy<object, IViewModelMetadata>> GatherViewModelData()
        {
            var data = new ViewModelData();
            DynamicComposer.Instance.Compose(data);
            return data.LocatedViewModels;
        }
    }
}
