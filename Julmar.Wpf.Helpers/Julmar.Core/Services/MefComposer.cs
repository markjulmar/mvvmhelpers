using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using JulMar.Core.Interfaces;
using System.Diagnostics;

namespace JulMar.Core.Services
{
    /// <summary>
    /// Inversion of Control Composer - currently based on MEF (4.0).
    /// </summary>
    public sealed class IoCComposer : IDynamicResolver, IDisposable
    {
        /// <summary>
        /// Singleton of MEF composer.
        /// </summary>
        private static readonly Lazy<IoCComposer> _singleton = new Lazy<IoCComposer>(() => new IoCComposer());

        /// <summary>
        /// IoC instance
        /// </summary>
        public static IDynamicResolver Instance
        {
            get { return _singleton.Value;  }
        }

        /// <summary>
        /// Internal constructor - stop this class from being created directly.
        /// </summary>
        private IoCComposer()
        {
        }

        /// <summary>
        /// Container used to resolve parts
        /// </summary>
        private CompositionContainer _container;

        /// <summary>
        /// Used to resolve a set of targets.
        /// </summary>
        public void Compose(params object[] targets)
        {
            if (_container == null)
                CreateContainer();
            Debug.Assert(_container != null);
            _container.ComposeParts(targets);
        }

        /// <summary>
        /// This is used to resolve a set of targets once - 
        /// with this method, MEF will not hold a reference to
        /// the composed tree of objects
        /// </summary>
        public void ComposeOnce(params object[] targets)
        {
            if (_container == null)
                CreateContainer();
            Debug.Assert(_container != null);

            foreach (var obj in targets)
            {
                try
                {
                    _container.SatisfyImportsOnce(obj);
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
        public T GetExportedValue<T>()
        {
            if (_container == null)
                CreateContainer();
            Debug.Assert(_container != null);
            return _container.GetExportedValueOrDefault<T>();
        }

        /// <summary>
        /// Create the composition container used to locate and fixup imports.
        /// </summary>
        private void CreateContainer()
        {
            var defaultAssembly = Path.GetFileName(Assembly.GetExecutingAssembly().CodeBase);
            var localCatalog = new CatalogExportProvider(new AssemblyCatalog(Assembly.GetExecutingAssembly()));

            // Get a catalog of all the assemblies *except* this one
            var catalog = new AggregateCatalog();
            foreach (string fname in
                Directory.GetFiles(@".\", "*.exe")
                    .Concat(Directory.GetFiles(@".\", "*.dll"))
                    .Where(fname => string.Compare(Path.GetFileName(fname), defaultAssembly, true) != 0))
            {
                try
                {
                    if (Assembly.ReflectionOnlyLoadFrom(fname) != null)
                        catalog.Catalogs.Add(new AssemblyCatalog(fname));
                }
                catch
                {
                }
            }

            _container = new CompositionContainer(catalog, localCatalog);
            localCatalog.SourceProvider = _container;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_container != null)
            {
                _container.Dispose();
                _container = null;
            }
        }
    }
}
