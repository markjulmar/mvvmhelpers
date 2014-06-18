using System;

namespace JulMar.Core
{
    /// <summary>
    /// This attribute is used to decorate all "auto-located" services.
    /// Must be placed at the assembly level.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class ExportViewModelAttribute : Attribute
    {
        /// <summary>
        /// Key used to export the View/ViewModel
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The Type for the view model.
        /// </summary>
        public Type ViewModelType { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">Key used to lookup ViewModel</param>
        /// <param name="viewModelType"></param>
        public ExportViewModelAttribute(string key, Type viewModelType)
        {
            this.Key = key;
            this.ViewModelType = viewModelType;
        }
    }
}