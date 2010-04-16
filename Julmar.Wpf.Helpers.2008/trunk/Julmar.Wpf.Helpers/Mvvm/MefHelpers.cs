using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using JulMar.Windows.Interfaces;

namespace JulMar.Windows.Mvvm
{
    /// <summary>
    /// Managed Extension Framework Helpers
    /// </summary>
    class MefLoader : IDynamicResolver
    {
        private CompositionContainer _container;

        /// <summary>
        /// Used to resolve a set of targets.
        /// </summary>
        public void Compose(params object[] targets)
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
    }
}
