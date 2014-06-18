using System;

namespace JulMar.Core
{
    /// <summary>
    /// This attribute is used to decorate all "auto-located" services.
    /// MEF is used to locate and bind each service with this attribute decoration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class ExportServiceAttribute : Attribute
    {
        /// <summary>
        /// Contract being exported
        /// </summary>
        public Type ContractType { get; private set; }

        /// <summary>
        /// Implementation type (optional)
        /// </summary>
        public Type ServiceType { get; private set; }

        /// <summary>
        /// Set to true to use as fallback (default)
        /// </summary>
        public bool IsFallback { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ExportServiceAttribute(Type contractType, Type serviceType = null)
        {
            ContractType = contractType;
            ServiceType = serviceType ?? contractType;
            IsFallback = false;
        }
    }
}