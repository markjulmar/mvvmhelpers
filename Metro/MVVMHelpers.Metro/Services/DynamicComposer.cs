using JulMar.Core.Internal;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JulMar.Windows.Extensions;

namespace JulMar.Core.Services
{
    /// <summary>
    /// Inversion of Control Composer - currently based on MEF (4.0).
    /// </summary>
    public sealed class DynamicComposer : IDisposable
    {
        private static readonly Lazy<DynamicComposer> _singleton = new Lazy<DynamicComposer>(() => new DynamicComposer());
        private ContainerConfiguration _container = null;
        private CompositionHost _host = null;

        /// <summary>
        /// IoC instance
        /// </summary>
        public static DynamicComposer Instance
        {
            get { return _singleton.Value;  }
        }

        /// <summary>
        /// Internal constructor - stop this class from being created directly.
        /// </summary>
        private DynamicComposer()
        {
        }

        /// <summary>
        /// This is called before any assemblies are added to the container.
        /// </summary>
        public event Func<DynamicComposer, ContainerConfiguration, List<Assembly>, bool> PreCreateContainer;

        /// <summary>
        /// This is called after all setup is performed and the container is about to be created.
        /// </summary>
        public event Action<DynamicComposer, ContainerConfiguration> CreatingContainer;

        /// <summary>
        /// The container being used
        /// </summary>
        public CompositionHost Host
        {
            get
            {
                if (_container == null)
                {
                    // Get a list of all the package assemblies.
                    var assemblies = Task.Run(async () =>
                    {
                        var localAssemblies = new List<Assembly>();
                        var result = await GetPackageAssemblyListAsync();
                        var theAsms = result.ToList();
                        foreach (var newAsm in theAsms.Where(asm => !localAssemblies.Contains(asm)
                            && !asm.FullName.StartsWith("System.Composition")
                            && !asm.FullName.StartsWith("JulMar")))
                            localAssemblies.Add(newAsm);
                        return localAssemblies;
                    }).Result;

                    _container = new ContainerConfiguration();

                    // Let any customization occur
                    if (PreCreateContainer == null
                        || !PreCreateContainer(this, _container, assemblies))
                    {
                        _container = _container.WithAssemblies(assemblies);
                    }

                    // Set up the container with base services
                    _container = _container
                        .WithAssembly(typeof(DynamicComposer).GetTypeInfo().Assembly)
                        .WithProvider(new DefaultExportDescriptorProvider());

                    if (CreatingContainer != null)
                        CreatingContainer(this, _container);

                    _host = _container.CreateContainer();
                }

                return _host;
            }
        }

        /// <summary>
        /// Used to resolve a set of targets.
        /// </summary>
        public void Compose(object target)
        {
            Host.SatisfyImports(target);
        }

        /// <summary>
        /// Retrieves the specified exported object by type, throws an exception if it cannot be found/created.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <exception cref="CompositionFailedException" />
        /// <returns>Created object</returns>
        public T GetExportedValue<T>()
        {
            return Host.GetExport<T>();
        }

        /// <summary>
        /// Retrieves the specified exported object by type, or NULL if it doesn't exist.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="value">Returning object, null if not found/created</param>
        /// <returns>True/False result</returns>
        public bool TryGetExportedValue<T>(out T value)
        {
            return Host.TryGetExport<T>(out value);
        }

        /// <summary>
        /// Retrieves the specified exported object by type and contract name.  
        /// Throws an exception if it cannot be found/created.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="contractName">Contract name</param>
        /// <exception cref="CompositionFailedException" />
        /// <returns>Created object</returns>
        public T GetExportedValue<T>(string contractName)
        {
            return Host.GetExport<T>(contractName);
        }

        /// <summary>
        /// Retrieves the specified exported object by type and contract.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="contractName">Contract name</param>
        /// <param name="value">Returning object, null if not found/created</param>
        /// <returns>True/False result</returns>
        public bool TryGetExportedValue<T>(string contractName, out T value)
        {
            return Host.TryGetExport<T>(contractName, out value);
        }

        /// <summary>
        /// Retrieves the specified exported objects by type.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Created objects</returns>
        public IEnumerable<T> GetExportedValues<T>()
        {
            return Host.GetExports<T>();
        }

        /// <summary>
        /// Retrieves the specified exported objects by type.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="contractName">Contract name</param>
        /// <returns>Created objects</returns>
        public IEnumerable<T> GetExportedValues<T>(string contractName)
        {
            return Host.GetExports<T>(contractName);
        }

        /// <summary>
        /// Retrieves the specified export by type.
        /// Throws an exception if it does not exist/cannot be created
        /// </summary>
        /// <param name="type">Type</param>
        /// <exception cref="CompositionFailedException" />
        /// <returns>Created object</returns>
        public object GetExportedValue(Type type)
        {
            return Host.GetExport(type);
        }

        /// <summary>
        /// Retrieves the specified export by type.
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="value">Returning value, null if not created/found.</param>
        /// <returns>True/False result</returns>
        public bool TryGetExportedValue(Type type, out object value)
        {
            return Host.TryGetExport(type, out value);
        }

        /// <summary>
        /// Retrieves the specified export by type/contract name.
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="contractName">Contract name</param>
        /// <exception cref="CompositionFailedException" />
        /// <returns>Created object</returns>
        public object GetExportedValue(Type type, string contractName)
        {
            return Host.GetExport(type, contractName);
        }

        /// <summary>
        /// Retrieves the specified export by type.
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="contractName">Contract name</param>
        /// <param name="value">Created object or null</param>
        /// <exception cref="CompositionFailedException" />
        /// <returns>True/False result</returns>
        public bool TryGetExportedValue(Type type, string contractName, out object value)
        {
            return Host.TryGetExport(type, contractName, out value);
        }

        /// <summary>
        /// Retrieves the specified exports by type.
        /// </summary>
        /// <param name="type">Type</param>
        public IEnumerable<object> GetExportedValues(Type type)
        {
            return Host.GetExports(type);
        }

        /// <summary>
        /// Retrieves the specified exports by type.
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="contractName"> </param>
        public IEnumerable<object> GetExportedValues(Type type, string contractName)
        {
            return Host.GetExports(type, contractName);
        }

        /// <summary>
        /// This method retrieves all the assemblies in the current app package
        /// </summary>
        /// <returns></returns>
        internal async static Task<IEnumerable<Assembly>> GetPackageAssemblyListAsync()
        {
            var installFolder = global::Windows.ApplicationModel.Package.Current.InstalledLocation;

            // If we are in the designer, then load all subdirectories which have DLLs too.
            // This allows the ViewModelLocator and ServiceLocator to find elements properly
            // when being shadow copied
            if (Designer.InDesignMode)
            {
                // Grab all the possibilities
                var assemblies = new List<Assembly>();
                foreach (var folder in await installFolder.GetFoldersAsync())
                {
                    assemblies.AddRange((await folder.GetFilesAsync())
                            .Where(file => file.FileType == ".dll" || file.FileType == ".exe")
                            .Select(file => file.Name.Substring(0, file.Name.Length - file.FileType.Length))
                            .Distinct()
                            .Select(asmName => Assembly.Load(new AssemblyName(asmName))));
                }
                return assemblies;
            }

            // Otherwise we just look in the package folder.
            return ((await installFolder.GetFilesAsync())
                .Where(file => file.FileType == ".dll" || file.FileType == ".exe")
                .Select(file => file.Name.Substring(0, file.Name.Length - file.FileType.Length))
                .Select(asmName => Assembly.Load(new AssemblyName(asmName))))
                .ToList();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_host != null)
            {
                _host.Dispose();
                _host = null;
            }

            _container = null;
        }
    }
}
