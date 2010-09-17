using System;
using System.ComponentModel.Composition;
using JulMar.Core.Interfaces;
using JulMar.Core.Services;

namespace JulMar.Core
{
    /// <summary>
    /// This attribute is used to decorate all "auto-located" services.
    /// MEF is used to locate and bind each service with this attribute decoration.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ExportServiceAttribute : ExportAttribute, IServiceProviderMetadata
    {
        /// <summary>
        /// Service Type being exported (typically an interface)
        /// </summary>
        public Type ServiceType { get { return base.ContractType; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Service Type to export</param>
        public ExportServiceAttribute(Type type) : base(ServiceLocator.MefLocatorKey, type)
        {
        }
    }
}