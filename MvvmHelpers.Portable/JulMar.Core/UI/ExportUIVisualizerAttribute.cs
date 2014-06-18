using System;

namespace JulMar.UI
{
    /// <summary>
    /// This attribute is used to decorate all "auto-located" services.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class ExportUIVisualizerAttribute : Attribute
    {
        /// <summary>
        /// Key used to export the View/ViewModel
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Type to create.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ExportUIVisualizerAttribute(string key, Type type)
        {
            this.Key = key;
            this.Type = type;
        }
    }
}