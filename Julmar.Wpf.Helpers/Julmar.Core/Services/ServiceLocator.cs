using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using JulMar.Core.Interfaces;

namespace JulMar.Core.Services
{
    /// <summary>
    /// This class acts as a resolver for typed services (interfaces and implementations).
    /// </summary>
    /// <example>
    /// To register a service use Add:
    /// <![CDATA[
    /// ServiceLocator.Instance.Add(typeof(IService), new Service());
    /// 
    /// To retrieve a service use Resolve:
    /// 
    /// IService svc = ServiceLocator.Instance.Resolve<IService>();
    /// ]]>
    /// </example>
    public sealed class ServiceLocator
    {
        /// <summary>
        /// Lazy created instance located through MEF.
        /// </summary>
        private static readonly Lazy<IServiceLocator> _instance = new Lazy<IServiceLocator>(CreateServiceLocator);

        /// <summary>
        /// Creates or locates the service locator object
        /// </summary>
        /// <returns></returns>
        private static IServiceLocator CreateServiceLocator()
        {
            return DynamicComposer.Instance.GetExportedValue<IServiceLocator>();
        }

        /// <summary>
        /// Service locator
        /// </summary>
        public static IServiceLocator Instance
        {
            get { return _instance.Value; }
        }
    }

    namespace Internal
    {
        /// <summary>
        /// Internal implementation of the service provider; public so MEF can create/expose it.
        /// </summary>
        [Export(typeof(IServiceLocator))]
        [PartCreationPolicy(CreationPolicy.Shared)]
        sealed class ServiceLocator : IServiceLocator, IDisposable
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
                    return DynamicLoadAndAdd(type) != null;

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
            public void Add<T>(Func<IServiceContainer, T> creatorCallback)
            {
                lock (_lock)
                {
                    EnsureServiceContainer();
                    if (Exists(typeof (T)))
                        Remove(typeof (T));
                    _serviceContainer.AddService(typeof (T), (isc, type) => creatorCallback(isc));
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
                    if (Exists(typeof (T)))
                        Remove(typeof (T));
                    _serviceContainer.AddService(typeof (T), value);
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
                return (T) GetService(typeof (T));
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
            /// <param name="value">Returning object</param>
            /// <returns>Lazy object or null</returns>
            private bool CheckLocatedServices(Type type, out object value)
            {
                return DynamicComposer.Instance.TryGetExportedValue(type, out value);
            }

            /// <summary>
            /// This searches the located MEF components and creates it and loads it into the service container.
            /// </summary>
            /// <param name="serviceType">Type we are looking for</param>
            /// <returns>Created object</returns>
            private object DynamicLoadAndAdd(Type serviceType)
            {
                object service;
                if (CheckLocatedServices(serviceType, out service) && service != null)
                {
                    _serviceContainer.AddService(serviceType, service);
                    return service;
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
                }
            }
        }
    }
}