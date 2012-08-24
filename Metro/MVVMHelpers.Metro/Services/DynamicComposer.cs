using JulMar.Core.Internal;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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
        private List<Assembly> _assemblies; 

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
        /// The container being used
        /// </summary>
        public CompositionHost Host
        {
            get
            {
                if (_container == null)
                {
                    // Gather list of assemblies to include and put them into the 
                    // proper order.
                    if (_assemblies == null)
                    {
                        _assemblies = new List<Assembly>();
                    }

                    // Add all package assemblies
                    Task.Run(async () =>
                    {
                        var result = await GetPackageAssemblyListAsync();
                        var theAsms = result.ToList();
                        foreach (var newAsm in theAsms.Where(asm => !_assemblies.Contains(asm) 
                            && !asm.FullName.StartsWith("System.Composition")
                            && !asm.FullName.StartsWith("JulMar")))
                            _assemblies.Add(newAsm);

                    }).Wait();

                    // Add the primary JulMar assembly last; it's where all the services are defined
                    _assemblies.Add(typeof(DynamicComposer).GetTypeInfo().Assembly);

                    _container = new ContainerConfiguration()
                        .WithAssemblies(_assemblies)
                        .WithProvider(new DefaultExportDescriptorProvider());

                    _host = _container.CreateContainer();
                    _assemblies = null;
                }

                return _host;
            }
        }

        /// <summary>
        /// This is used to add assemblies to the resolution process
        /// </summary>
        /// <param name="assemblies"></param>
        public void AddAssembliesToResolver(params Assembly[] assemblies)
        {
            if (_container != null)
                throw new Exception("Cannot add assemblies after container has been created.");

            if (_assemblies == null)
                _assemblies = new List<Assembly>();
            
            _assemblies.AddRange(assemblies);
        }

        /// <summary>
        /// Used to resolve a set of targets.
        /// </summary>
        public void Compose(object target)
        {
            Host.SatisfyImports(target);
        }

        /// <summary>
        /// Retrieves the specified exported object by type, or NULL if it doesn't exist.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Created object</returns>
        public T GetExportedValue<T>()
        {
            return Host.GetExport<T>();
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
        /// Retrieves the specified export by type.
        /// </summary>
        /// <param name="type">Type</param>
        public object GetExportedValue(Type type)
        {
            return Host.GetExport(type);
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
        /// This method retrieves all the assemblies in the current app package
        /// </summary>
        /// <returns></returns>
        internal async static Task<IEnumerable<Assembly>> GetPackageAssemblyListAsync()
        {
            var folder = global::Windows.ApplicationModel.Package.Current.InstalledLocation;

            var result = from file in await folder.GetFilesAsync()
                         where file.FileType == ".dll" || file.FileType == ".exe"
                         let filename = file.Name.Substring(0, file.Name.Length - file.FileType.Length)
                         select new AssemblyName { Name = filename }
                             into name
                             select Assembly.Load(name);

            return result.ToList();
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
