using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel.Composition.Primitives;

namespace JulMar.Core.Services
{
    /// <summary>
    /// Inversion of Control Composer - currently based on MEF (4.0).
    /// </summary>
    public sealed class DynamicComposer : IDisposable
    {
        private static readonly Lazy<DynamicComposer> _singleton = new Lazy<DynamicComposer>(() => new DynamicComposer());
        private Lazy<CompositionContainer> _container;
#if NET35
        private bool _isCreated = false;
#endif
        private AggregateCatalog _userDefinedCatalogs;

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
            _container = new Lazy<CompositionContainer>(CreateContainer);
        }

        /// <summary>
        /// The composition container being used
        /// </summary>
        public CompositionContainer Container
        {
            get
            {
                if (_container == null)
                    throw new ObjectDisposedException("IoCComposer has been disposed.");

                return _container.Value;
            }
        }

        /// <summary>
        /// This method allows direct use of the container to add new catalogs.  This
        /// should be done before anything else in the application occurs.
        /// </summary>
        /// <param name="catalog"></param>
        /// <returns></returns>
        public void AddCatalogResolver(params ComposablePartCatalog[] catalog)
        {
            if (_container == null)
                throw new ObjectDisposedException("IoCComposer has been disposed.");
#if NET35
            if (_isCreated)
#else
            if (_container.IsValueCreated)
#endif
                throw new InvalidOperationException("Parts have already been composed - cannot add new catalog resolvers");
            _userDefinedCatalogs = new AggregateCatalog(catalog);
        }

        /// <summary>
        /// Used to resolve a set of targets.
        /// </summary>
        public void Compose(params object[] targets)
        {
            Container.ComposeParts(targets);
        }

        /// <summary>
        /// This is used to resolve a set of targets once - 
        /// with this method, MEF will not hold a reference to
        /// the composed tree of objects
        /// </summary>
        public void ComposeOnce(params object[] targets)
        {
            foreach (var obj in targets)
            {
                try
                {
                    Container.SatisfyImportsOnce(obj);
                }
                catch
                {
                    // Ignore
                }
            }
        }

        /// <summary>
        /// Retrieves the specified exported object by type, or NULL if it doesn't exist.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Created object</returns>
        /// <exception cref="CompositionException" />
        /// <exception cref="ImportCardinalityMismatchException" />
        /// <exception cref="CompositionContractMismatchException" />
        public T GetExportedValue<T>()
        {
            return Container.GetExportedValue<T>();
        }

        /// <summary>
        /// Retrieves the specified exported object by type, or NULL if it doesn't exist.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="value">Returning object, null if not found/created</param>
        /// <returns>True/False result</returns>
        /// <exception cref="CompositionException" />
        /// <exception cref="ImportCardinalityMismatchException" />
        /// <exception cref="CompositionContractMismatchException" />
        public bool TryGetExportedValue<T>(out T value)
        {
            value = Container.GetExportedValueOrDefault<T>();
            return ReferenceEquals(value, null);
        }

        /// <summary>
        /// Retrieves the specified exported object by type and contract name.  
        /// Throws an exception if it cannot be found/created.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="contractName">Contract name</param>
        /// <exception cref="CompositionException" />
        /// <exception cref="ImportCardinalityMismatchException" />
        /// <exception cref="CompositionContractMismatchException" />
        /// <returns>Created object</returns>
        public T GetExportedValue<T>(string contractName)
        {
            return Container.GetExportedValue<T>(contractName);
        }

        /// <summary>
        /// Retrieves the specified exported object by type and contract.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="contractName">Contract name</param>
        /// <param name="value">Returning object, null if not found/created</param>
        /// <returns>True/False result</returns>
        /// <exception cref="CompositionException" />
        /// <exception cref="ImportCardinalityMismatchException" />
        /// <exception cref="CompositionContractMismatchException" />
        public bool TryGetExportedValue<T>(string contractName, out T value)
        {
            value = Container.GetExportedValueOrDefault<T>(contractName);
            return ReferenceEquals(value, null);
        }

        /// <summary>
        /// Retrieves the specified exported objects by type.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Created objects</returns>
        /// <exception cref="CompositionException" />
        /// <exception cref="ImportCardinalityMismatchException" />
        /// <exception cref="CompositionContractMismatchException" />
        public IEnumerable<T> GetExportedValues<T>()
        {
            return Container.GetExportedValues<T>();
        }

        /// <summary>
        /// Retrieves the specified exported objects by type.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="contractName">Contract name</param>
        /// <returns>Created objects</returns>
        /// <exception cref="CompositionException" />
        /// <exception cref="ImportCardinalityMismatchException" />
        /// <exception cref="CompositionContractMismatchException" />
        public IEnumerable<T> GetExportedValues<T>(string contractName)
        {
            return Container.GetExportedValues<T>(contractName);
        }

        /// <summary>
        /// Retrieves the specified export by type.
        /// Throws an exception if it does not exist/cannot be created
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Created object</returns>
        /// <exception cref="CompositionException" />
        /// <exception cref="ImportCardinalityMismatchException" />
        /// <exception cref="CompositionContractMismatchException" />
        public object GetExportedValue(Type type)
        {
            var result = Container.GetExports(type, null, null).FirstOrDefault();
            if (result == null)
                throw new CompositionException("Failed to locate part " + type.Name);
            return result.Value;
        }

        /// <summary>
        /// Retrieves the specified export by type.
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="value">Returning value, null if not created/found.</param>
        /// <returns>True/False result</returns>
        /// <exception cref="CompositionException" />
        /// <exception cref="ImportCardinalityMismatchException" />
        /// <exception cref="CompositionContractMismatchException" />
        public bool TryGetExportedValue(Type type, out object value)
        {
            var result = Container.GetExports(type, null, null).FirstOrDefault();
            value = result == null ? null : result.Value;
            return result != null;
        }

        /// <summary>
        /// Retrieves the specified export by type/contract name.
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="contractName">Contract name</param>
        /// <returns>Created object</returns>
        /// <exception cref="CompositionException" />
        /// <exception cref="ImportCardinalityMismatchException" />
        /// <exception cref="CompositionContractMismatchException" />
        public object GetExportedValue(Type type, string contractName)
        {
            var result = Container.GetExports(type, null, contractName).FirstOrDefault();
            if (result == null)
                throw new CompositionException("Failed to locate part " + type.Name);
            return result.Value;
        }

        /// <summary>
        /// Retrieves the specified export by type.
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="contractName">Contract name</param>
        /// <param name="value">Created object or null</param>
        /// <returns>True/False result</returns>
        /// <exception cref="CompositionException" />
        /// <exception cref="ImportCardinalityMismatchException" />
        /// <exception cref="CompositionContractMismatchException" />
        public bool TryGetExportedValue(Type type, string contractName, out object value)
        {
            var result = Container.GetExports(type, null, contractName).FirstOrDefault();
            value = result == null ? null : result.Value;
            return result != null;
        }

        /// <summary>
        /// Retrieves the specified exports by type.
        /// </summary>
        /// <param name="type">Type</param>
        /// <exception cref="CompositionException" />
        /// <exception cref="ImportCardinalityMismatchException" />
        /// <exception cref="CompositionContractMismatchException" />
        public IEnumerable<object> GetExportedValues(Type type)
        {
            return Container.GetExports(type, null, null)
                .Select(li => li.Value);
        }

        /// <summary>
        /// Retrieves the specified exports by type.
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="contractName"> </param>
        /// <exception cref="CompositionException" />
        /// <exception cref="ImportCardinalityMismatchException" />
        /// <exception cref="CompositionContractMismatchException" />
        public IEnumerable<object> GetExportedValues(Type type, string contractName)
        {
            return Container.GetExports(type, null, contractName)
                .Select(li => li.Value);
        }

        /// <summary>
        /// Create the composition container used to locate and fixup imports.
        /// </summary>
        private CompositionContainer CreateContainer()
        {
            // Create our default catalog.  This is where we will place the core JulMar assemblies.
            AggregateCatalog defaultCatalog = new AggregateCatalog();
            defaultCatalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly())); // core

            // See if helpers is available as well.
            Assembly asm = Assembly.Load("julmar.wpf.helpers");
            if (asm != null)
                defaultCatalog.Catalogs.Add(new AssemblyCatalog(asm));

            // Create the aggregate catalog to hold each of the directory catalogs.
            var catalog = new AggregateCatalog();

            // Add all non-core assemblies in the AppBase first.
            foreach (var fname in GenerateAssemblyList(AppDomain.CurrentDomain.BaseDirectory))
                AddAssemblyToCatalog(fname, catalog);

            // Add any user defined catalogs. These are added after assemblies found in base directory.
            if (_userDefinedCatalogs != null)
                catalog.Catalogs.Add(_userDefinedCatalogs);

            // Place the default catalog which contains the core JulMar assemblies at the end of the chain.
            // This allows applications to override almost all of the base services if desired.
            var defaultCatalogEp = new CatalogExportProvider(defaultCatalog);
            var container = new CompositionContainer(catalog, defaultCatalogEp);
            defaultCatalogEp.SourceProvider = container;

#if NET35
            _isCreated = true;
#endif

            return container;
        }

        /// <summary>
        /// Adds the given assembly to the catalog for composition
        /// </summary>
        /// <param name="fname"></param>
        /// <param name="catalog"></param>
        private static bool AddAssemblyToCatalog(string fname, AggregateCatalog catalog)
        {
            try
            {
                if (Assembly.ReflectionOnlyLoadFrom(fname) != null)
                {
                    catalog.Catalogs.Add(new AssemblyCatalog(fname));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("MEF failed to load/add {0} to aggregate catalog. {1}", fname, ex));
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// This method returns the assembly list to use for composition
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<string> GenerateAssemblyList(string path)
        {
            string[] ignoreAssemblies = { "julmar.core.dll", "julmar.wpf.helpers.dll" };

            // Go through all .exe and .dll images
            foreach (string fname in Directory.GetFiles(path, "*.exe").Concat(Directory.GetFiles(path, "*.dll")))
            {
                string testFilename = Path.GetFileName(fname);
                if (!string.IsNullOrEmpty(testFilename))
                {
                    if (!ignoreAssemblies.Contains(testFilename.ToLower()))
                        yield return fname;
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
#if NET35
            if (_isCreated)
#else
            if (_container.IsValueCreated)
#endif
            {
                _container.Value.Dispose();
                _container = null;
            }
        }
    }
}
