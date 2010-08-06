using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.Linq;
using JulMar.Windows.Interfaces;
using JulMar.Windows.Extensions;

namespace JulMar.Windows.Mvvm
{
    /// <summary>
    /// Interface used to populate metadata we use for services.
    /// </summary>
    public interface IServiceProviderMetadata
    {
        /// <summary>
        /// Service Type being exported (typically an interface)
        /// </summary>
        Type ServiceType { get; }

        /// <summary>
        /// Only provide this service at design-time.  If set,
        /// this service will not resolve at runtime - but will be
        /// prioritized instead for design time.
        /// </summary>
        bool DesignTimeOnly { get; }
    }

    /// <summary>
    /// This attribute is used to decorate all "auto-located" services.
    /// MEF is used to locate and bind each service with this attribute decoration.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ExportServiceProviderAttribute : ExportAttribute
    {
        /// <summary>
        /// Service Type being exported (typically an interface)
        /// </summary>
        public Type ServiceType { get; set; }

        /// <summary>
        /// Only provide this service at design-time.  If set,
        /// this service will not resolve at runtime - but will be
        /// prioritized instead for design time.
        /// </summary>
        public bool DesignTimeOnly { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Service Type to export</param>
        public ExportServiceProviderAttribute(Type type) : base(ServiceProvider.MefLocatorKey)
        {
            ServiceType = type;
        }
    }

    /// <summary>
    /// This class acts as a resolver for typed services (interfaces and implementations).
    /// Internally it relies on an IServiceContainer - it will create a BCL version if one is not 
    /// supplied.  Any custom implementation can also be used - this provider will not use the 
    /// promotion features so those do not need to be implemented.
    /// </summary>
    /// <example>
    /// To register a service use Add:
    /// <![CDATA[
    /// serviceResolver.Add(typeof(IService), new Service());
    /// 
    /// To retrieve a service use Resolve:
    /// 
    /// IService svc = serviceResolver<IService>.Resolve();
    /// ]]>
    /// </example>
    [Export(typeof(IServiceProvider))]
    public class ServiceProvider : IServiceProvider, IDisposable
    {
        private static readonly Lazy<ServiceProvider> _globalServiceProvider = new Lazy<ServiceProvider>(() => new ServiceProvider());

        /// <summary>
        /// Primary service provider used for VMs.  You can still create a new
        /// service provider (for private elements), but this is the main one shared 
        /// with the VM.
        /// </summary>
        public static ServiceProvider Primary
        {
            get { return _globalServiceProvider.Value; }
        }

        /// <summary>
        /// The MEF loader.
        /// </summary>
        [Export(typeof(IDynamicResolver))]
        private IDynamicResolver _mefLoader = new MefLoader();

        /// <summary>
        /// Key used to bind exports together
        /// </summary>
        internal const string MefLocatorKey = "JulMar.Mvvm.ServiceProviderExport";

        [ImportMany(MefLocatorKey, AllowRecomposition = true)]
        #pragma warning disable 649
        private IEnumerable<Lazy<object, IServiceProviderMetadata>> _locatedServices;
        #pragma warning restore 649

        /// <summary>
        /// Lock
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// Service container
        /// </summary>
        private IServiceContainer _serviceContainer;

        /// <summary>
        /// Construct the service provider.  Resolve all the imported services.
        /// </summary>
        public ServiceProvider()
        {
            _mefLoader.Compose(this);
        }

        /// <summary>
        /// This allows the user to set the container to a different type.
        /// If not performed, the default ServiceContainer will be created and used.
        /// </summary>
        /// <param name="container"></param>
        public void SetServiceContainer(IServiceContainer container)
        {
            lock (_lock)
            {
                if (_serviceContainer != null)
                    throw new InvalidOperationException("Cannot set container once provider is active.");

                _serviceContainer = container;
            }
        }

        /// <summary>
        /// Returns whether the service exists.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>True/False</returns>
        public bool Exists(Type type)
        {
            lock (_lock)
            {
                // Quick check of the container.
                if (_serviceContainer != null
                    && _serviceContainer.GetService(type) != null)
                    return true;

                // Not in the container - try the dynamic elements (MEF).
                // We do not create it here, just check for the presence.  If the user
                // actually REQUESTS the service we will create it then.
                return CheckLocatedServices(type) != null;
                
                // Not found.
            }
        }

        /// <summary>
        /// Adds a new service to the resolver list
        /// </summary>
        /// <param name="type">Service Type (typically an interface)</param>
        /// <param name="value">Object that implements service</param>
        public void Add(Type type, object value)
        {
            lock (_lock)
            {
                EnsureServiceContainer();
                if (Exists(type))
                    Remove(type);
                _serviceContainer.AddService(type, value);
            }
        }

        /// <summary>
        /// This registers a new service by type, with a creator callback
        /// to generate the service at runtime.
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="creatorCallback">Callback to create the service</param>
        public void Add(Type type, ServiceCreatorCallback creatorCallback)
        {
            lock (_lock)
            {
                EnsureServiceContainer();
                if (Exists(type))
                    Remove(type);
                _serviceContainer.AddService(type, creatorCallback);
            }
        }

        /// <summary>
        /// This registers a new service by type with a creator callback.
        /// </summary>
        /// <typeparam name="T">Type to generate</typeparam>
        /// <param name="creatorCallback">Creator callback</param>
        public void Add<T>(Func<IServiceContainer,T> creatorCallback)
        {
            lock (_lock)
            {
                EnsureServiceContainer();
                if (Exists(typeof(T)))
                    Remove(typeof(T));
                _serviceContainer.AddService(typeof(T), (isc, type) => creatorCallback(isc));
            }
        }

        /// <summary>
        /// This adds a new service to the resolver list.
        /// </summary>
        /// <typeparam name="T">Type of the service</typeparam>
        /// <param name="value">Value</param>
        public void Add<T>(T value)
        {
            lock (_lock)
            {
                EnsureServiceContainer();
                if (Exists(typeof(T)))
                    Remove(typeof(T));
                _serviceContainer.AddService(typeof(T), value);
            }
        }

        /// <summary>
        /// Ensures the container has been created.
        /// </summary>
        private void EnsureServiceContainer()
        {
            if (_serviceContainer == null)
            {
                _serviceContainer = new ServiceContainer();
                _serviceContainer.AddService(typeof (IDynamicResolver), _mefLoader);
            }
        }

        /// <summary>
        /// Remove a service
        /// </summary>
        /// <param name="type">Type to remove</param>
        public void Remove(Type type)
        {
            lock (_lock)
            {
                if (_serviceContainer != null)
                {
                    if (Exists(type))
                        _serviceContainer.RemoveService(type);
                }
            }
        }

        /// <summary>
        /// This resolves a service type and returns the implementation. Note that this
        /// assumes the key used to register the object is of the appropriate type or
        /// this method will throw an InvalidCastException!
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>Implementation</returns>
        public T Resolve<T>()
        {
            return (T) GetService(typeof(T));
        }

        /// <summary>
        /// Implementation of IServiceProvider
        /// </summary>
        /// <param name="serviceType">Service Type</param>
        /// <returns>Object implementing service</returns>
        public object GetService(Type serviceType)
        {
            lock (_lock)
            {
                EnsureServiceContainer();
                return _serviceContainer.GetService(serviceType) ?? DynamicLoadAndAdd(serviceType);
            }
        }

        /// <summary>
        /// This locates a lazy service record for a give type.
        /// </summary>
        /// <param name="type">Type to search for</param>
        /// <returns>Lazy object or null</returns>
        private Lazy<object,IServiceProviderMetadata> CheckLocatedServices(Type type)
        {
            // No located services (MEF not initialized).
            if (_locatedServices == null)
                return null;

            // Get a list of all services matching the type.
            var services = _locatedServices.Where(svc => svc.Metadata.ServiceType == type);

            // If we are in design mode, search for a design-time specific version.
            // Return the first one found.
            if (Designer.InDesignMode)
            {
                var foundService = services.FirstOrDefault(svc => svc.Metadata.DesignTimeOnly);
                if (foundService != null)
                    return foundService;
            }

            // No design-time service found, or not in design mode.  Return the first
            // non-design time service.
            return services.FirstOrDefault(svc => !svc.Metadata.DesignTimeOnly);
        }

        /// <summary>
        /// This searches the located MEF components and creates it and loads it into the service container.
        /// </summary>
        /// <param name="serviceType">Type we are looking for</param>
        /// <returns>Created object</returns>
        private object DynamicLoadAndAdd(Type serviceType)
        {
            var service = CheckLocatedServices(serviceType);
            if (service != null)
            {
                var value = service.Value;
                if (value != null)
                {
                    _serviceContainer.AddService(serviceType, value);
                    return value;
                }
            }
            return null;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            lock (_lock)
            {
                var id = _serviceContainer as IDisposable;
                if (id != null)
                    id.Dispose();
                _serviceContainer = null;
                _mefLoader = null;
            }
        }
    }
}