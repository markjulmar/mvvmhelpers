using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting.Core;
using System.Linq;

namespace JulMar.Core.Internal
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public sealed class DefaultExportAttribute : ExportAttribute
    {
        public DefaultExportAttribute(Type contractType)
            : base(DefaultExportDescriptorProvider.DefaultContractNamePrefix, contractType)
        {
        }

        public DefaultExportAttribute(string contractName, Type contractType)
            : base(DefaultExportDescriptorProvider.DefaultContractNamePrefix + contractName, contractType)
        {
        }
    }

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
