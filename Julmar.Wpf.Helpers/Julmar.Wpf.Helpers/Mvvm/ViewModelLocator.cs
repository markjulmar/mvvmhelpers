using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using JulMar.Windows.Interfaces;
using JulMar.Windows.UI;

namespace JulMar.Windows.Mvvm
{
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
    [ExportServiceProvider(typeof(ViewModelLocator))]
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
        [ImportMany(MefLocatorKey, AllowRecomposition = true)] private IEnumerable<Lazy<object, IUIVisualizerMetadata>> _locatedVms;
        #pragma warning restore 649

        /// <summary>
        /// Finds the VM based on the key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Locate(string key)
        {
            var entry = _locatedVms.FirstOrDefault(vm => vm.Metadata.Key == key);
            return entry != null ? entry.Value : null;
        }
    }
}
