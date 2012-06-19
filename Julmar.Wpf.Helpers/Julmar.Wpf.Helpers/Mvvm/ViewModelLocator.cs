using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using JulMar.Core;
using JulMar.Core.Services;

namespace JulMar.Windows.Mvvm
{
    /// <summary>
    /// Interface used to populate metadata we use for services.
    /// </summary>
    public interface IViewModelMetadata
    {
        /// <summary>
        /// Key used to export the ViewModel.  We only allow one export for VMs.
        /// </summary>
        string[] Key { get; }

        /// <summary>
        /// The type being exported
        /// </summary>
        string ExportTypeIdentity { get; }
    }

    /// <summary>
    /// This attribute is used to decorate all "auto-located" services.
    /// MEF is used to locate and bind each service with this attribute decoration.
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
    [ExportService(typeof(ViewModelLocator))]
    public class ViewModelLocator
    {
        /// <summary>
        /// Key used to bind exports together
        /// </summary>
        internal const string MefLocatorKey = "JulMar.ViewModel.Export";

        #pragma warning disable 649
        /// <summary>
        /// Collection of VMs
        /// </summary>
        [ImportMany(MefLocatorKey, AllowRecomposition = true)]
        private IEnumerable<Lazy<object, IViewModelMetadata>> _locatedVms;
        #pragma warning restore 649

        /// <summary>
        /// Finds the VM based on the key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Locate(string key)
        {
            object returnValue = null;

            var vmType = (from locatedVm in _locatedVms 
                    where locatedVm.Metadata.Key.Any(uiKey => uiKey == key) 
                    select locatedVm)
               .FirstOrDefault();
            
            if (vmType != null)
            {
                Type type = AppDomain.CurrentDomain.GetAssemblies()
                    .Select(a => a.GetType(vmType.Metadata.ExportTypeIdentity))
                    .FirstOrDefault(a => a != null);

                if (type != null)
                {
                    object[] attributes = type.GetCustomAttributes(typeof (PartCreationPolicyAttribute), true);
                    if (attributes.Length > 0)
                    {
                        PartCreationPolicyAttribute pca = attributes[0] as PartCreationPolicyAttribute;
                        if (pca != null)
                        {
                            // If it's not a shared object and we've already created it
                            // then go ahead and create another one.
#if NET35
                            if (pca.CreationPolicy == CreationPolicy.NonShared)
#else
                            if (pca.CreationPolicy == CreationPolicy.NonShared
                                && vmType.IsValueCreated)
#endif
                            {
                                // Create a new copy of the VM
                                returnValue = DynamicComposer.Instance.GetExportedVaue(type);
                                if (returnValue == null)
                                {
                                    returnValue = Activator.CreateInstance(type);
                                    DynamicComposer.Instance.Compose(returnValue);
                                }
                            }
                        }
                    }
                }

                if (returnValue == null)
                    returnValue = vmType.Value;
            }

            return returnValue;
        }
    }
}
