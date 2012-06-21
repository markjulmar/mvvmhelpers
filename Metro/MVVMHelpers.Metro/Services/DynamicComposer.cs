using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Reflection;

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
                    _container = new ContainerConfiguration();
                    if (_assemblies != null)
                        _container.WithAssemblies(_assemblies);
                    _container.WithAssembly(typeof (DynamicComposer).GetTypeInfo().Assembly);

                    _assemblies = null;

                    _host = _container.CreateContainer();
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
        /// Retrieves the specified export by type.
        /// </summary>
        /// <param name="type">Type</param>
        public object GetExportedValue(Type type)
        {
            return Host.GetExport(type);
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
