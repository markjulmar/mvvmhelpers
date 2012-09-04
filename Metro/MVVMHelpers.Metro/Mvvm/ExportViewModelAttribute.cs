using System;
using System.Composition;
using JulMar.Windows.Mvvm.Internal;

namespace JulMar.Windows.Mvvm
{
    /// <summary>
    /// This attribute is used to decorate all "auto-located" services.
    /// MEF is used to locate and bind each service with this attribute decoration.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ExportViewModelAttribute : ExportAttribute
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
            : base(ViewModelLocatorImpl.MefLocatorKey, typeof(object))
        {
            Key = key;
        }
    }
}