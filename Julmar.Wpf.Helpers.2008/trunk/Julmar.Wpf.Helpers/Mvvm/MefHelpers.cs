using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;

namespace JulMar.Windows.Mvvm
{
    /// <summary>
    /// This interface is exposed through the service provider to get to 
    /// the resolver (MEF).
    /// </summary>
    public interface IDynamicLoader
    {
        /// <summary>
        /// Used to resolve a set of targets.
        /// </summary>
        void Resolve(params object[] targets);
    }

    /// <summary>
    /// Managed Extension Framework Helpers
    /// </summary>
    class MefLoader : IDynamicLoader
    {
        private CompositionContainer _container;

        /// <summary>
        /// Used to resolve a set of targets.
        /// </summary>
        public void Resolve(params object[] targets)
        {
            if (_container == null)
                CreateContainer();
            _container.ComposeParts(targets);
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
            foreach (string fname in Directory.GetFiles(@".\", "*.*"))
            {
                if (string.Compare(Path.GetFileName(fname), defaultAssembly, true) != 0)
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
            }

            _container = new CompositionContainer(catalog, localCatalog);
            localCatalog.SourceProvider = _container;
        }
    }
}
