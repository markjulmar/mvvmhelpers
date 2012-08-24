using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using JulMar.Core.Services;
using System.Reflection;

namespace JulMar.Windows.Mvvm
{
    namespace Internal
    {
        /// <summary>
        /// Interface used to populate metadata we use for services.
        /// </summary>
        public class ViewModelMetadata
        {
            /// <summary>
            /// Key used to export the ViewModel.  We only allow one export for VMs.
            /// </summary>
            public string Key { get; set; }
        }
    }

    /// <summary>
    /// This attribute is used to decorate all "auto-located" services.
    /// MEF is used to locate and bind each service with this attribute decoration.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ExportViewModelAttribute : ExportAttribute
    {
        /// <summary>
        /// Key used to export the View/ViewModel
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">Key used to lookup ViewModel</param>
        public ExportViewModelAttribute(string key)
            : base(ViewModelLocator.MefLocatorKey, typeof(object))
        {
            Key = key;
        }
    }

    /// <summary>
    /// This class holds ViewModels that are registered with the ExportViewModelAttribute.
    /// </summary>
    [Export, Shared]
    public sealed class ViewModelLocator
    {
        /// <summary>
        /// Key used to bind exports together
        /// </summary>
        internal const string MefLocatorKey = "JulMar.ViewModel.Export";

        /// <summary>
        /// Collection of VMs
        /// </summary>
        [ImportMany(MefLocatorKey)]
        public IList<Lazy<object, Internal.ViewModelMetadata>> LocatedViewModels { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ViewModelLocator()
        {
            // Ensure we get our composition
            DynamicComposer.Instance.Compose(this);
        }

        /// <summary>
        /// Bindable operator for retrieving view models.
        /// </summary>
        /// <param name="key">Key to search for</param>
        /// <returns>Located view model or null</returns>
        public object this[string key]
        {
            get
            {
                object value;
                return !TryLocate(key, out value) ? null : value;
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

            // First look for the key in our metadata collection
            var vmType = LocatedViewModels.FirstOrDefault(vm => vm.Metadata.Key == key);
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
                        // If it's not a shared object and we've already created it
                        // then go ahead and create another one.
                        if (vmType.IsValueCreated)
                        {
                            // Create a new copy of the VM
                            returnValue = DynamicComposer.Instance.GetExportedValue(type);
                            if (returnValue == null)
                            {
                                returnValue = Activator.CreateInstance(type);
                                DynamicComposer.Instance.Compose(returnValue);
                            }
                        }
                    }
                }
            }

            return returnValue != null;
        }
    }
}
