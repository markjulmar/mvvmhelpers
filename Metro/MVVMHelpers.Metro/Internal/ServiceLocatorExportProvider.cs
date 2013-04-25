using System.Collections.Generic;
using System.Composition.Hosting.Core;
using JulMar.Core.Interfaces;
using JulMar.Core.Services;

namespace JulMar.Windows.Internal
{
    /// <summary>
    /// This simple export provider searches the ServiceLocator for exports.
    /// </summary>
    sealed class ServiceLocatorExportProvider : ExportDescriptorProvider
    {
        /// <summary>
        /// Promise export descriptors for the specified export key.
        /// </summary>
        /// <param name="contract">The export key required by another component.</param><param name="descriptorAccessor">Accesses the other export descriptors present in the composition.</param>
        /// <returns>
        /// Promises for new export descriptors.
        /// </returns>
        /// <remarks>
        /// A provider will only be queried once for each unique export key.
        /// The descriptor accessor can only be queried immediately if the descriptor being promised is an adapter, such as
        /// <see cref="T:System.Lazy`1"/>; otherwise, dependencies should only be queried within execution of the function provided
        /// to the <see cref="T:System.Composition.Hosting.Core.ExportDescriptorPromise"/>. The actual descriptors provided should not close over or reference any
        /// aspect of the dependency/promise structure, as this should be able to be GC'ed.
        /// </remarks>
        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor descriptorAccessor)
        {
            IServiceLocator sp = ServiceLocator.Instance;
            var providedTypes = sp.RegisteredServices;
            if (!providedTypes.ContainsKey(contract.ContractType))
                return NoExportDescriptors;

            object value = providedTypes[contract.ContractType];
            return new[]
                       {
                           new ExportDescriptorPromise(contract, "ServiceLocator", true, NoDependencies,
                                                       _ => ExportDescriptor.Create((c, o) => value, NoMetadata)),
                       };
        }
    }
}