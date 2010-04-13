using System;
using System.ComponentModel.Design;

namespace JulMar.Windows.Mvvm
{
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
    public class ServiceProvider : IServiceProvider, IDisposable
    {
        /// <summary>
        /// Lock
        /// </summary>
        private readonly object _lock = new object();
        /// <summary>
        /// Service container
        /// </summary>
        private IServiceContainer _serviceContainer;

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
                return (GetService(type) != null);
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
                _serviceContainer = new ServiceContainer();
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
                return _serviceContainer != null ? _serviceContainer.GetService(serviceType) : null;
            }
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
            }
        }
    }
}