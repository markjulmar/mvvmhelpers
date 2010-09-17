using System;

namespace JulMar.Core.Interfaces
{
    /// <summary>
    /// Interface used to populate metadata we use for services.
    /// </summary>
    public interface IServiceProviderMetadata
    {
        /// <summary>
        /// Service Type being exported (typically an interface)
        /// </summary>
        Type ServiceType { get; }
    }
}