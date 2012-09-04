using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting.Core;
using System.Linq;

namespace JulMar.Core.Internal
{
    /// <summary>
    /// This represents a default implementation of an export - it will only
    /// be used if there is no other export of the same type / contract name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public sealed class DefaultExportAttribute : ExportAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contractType">Export type</param>
        public DefaultExportAttribute(Type contractType)
            : base(DefaultExportDescriptorProvider.DefaultContractNamePrefix, contractType)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contractName">Contract name</param>
        /// <param name="contractType">Export type</param>
        public DefaultExportAttribute(string contractName, Type contractType)
            : base(DefaultExportDescriptorProvider.DefaultContractNamePrefix + contractName, contractType)
        {
        }
    }

    /// <summary>
    /// Default export provider
    /// </summary>
    internal sealed class DefaultExportDescriptorProvider : ExportDescriptorProvider
    {
        internal const string DefaultContractNamePrefix = "Default++";

        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor descriptorAccessor)
        {
            // Avoid trying to create defaults-of-defaults-of...
            if (contract.ContractName != null && contract.ContractName.StartsWith(DefaultContractNamePrefix))
                return NoExportDescriptors;

            var implementations = descriptorAccessor.ResolveDependencies("test for default", contract, false);
            if (implementations.Any())
                return NoExportDescriptors;

            var defaultImplementationDiscriminator = DefaultContractNamePrefix + (contract.ContractName ?? "");
            IDictionary<string, object> copiedConstraints = null;
            if (contract.MetadataConstraints != null)
                copiedConstraints = contract.MetadataConstraints.ToDictionary(k => k.Key, k => k.Value);
            var defaultImplementationContract = new CompositionContract(contract.ContractType, defaultImplementationDiscriminator, copiedConstraints);

            CompositionDependency defaultImplementation;
            if (!descriptorAccessor.TryResolveOptionalDependency("default", defaultImplementationContract, true, out defaultImplementation))
                return NoExportDescriptors;

            return new[] { new ExportDescriptorPromise(
                contract,
                "Default Implementation",
                false,
                () => new[] { defaultImplementation },
                _ => {
                    var defaultDescriptor = defaultImplementation.Target.GetDescriptor();
                    return ExportDescriptor.Create((c, o) => defaultDescriptor.Activator(c, o), defaultDescriptor.Metadata);
                })};
        }
    }
}
