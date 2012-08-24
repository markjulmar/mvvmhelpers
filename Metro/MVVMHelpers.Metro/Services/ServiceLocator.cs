using System;
using System.Collections.Generic;
using System.Composition;
using JulMar.Core.Interfaces;
using JulMar.Core.Internal;

namespace JulMar.Core.Services
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
    public sealed class ServiceLocator
    {
        /// <summary>
        /// Lazy created instance located through MEF.
        /// </summary>
        private static readonly Lazy<IServiceLocator> _instance = 
            new Lazy<IServiceLocator>(() => DynamicComposer.Instance.GetExportedValue<IServiceLocator>());

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
        [DefaultExport(typeof(IServiceLocator)), Shared]
        public sealed class ServiceProviderImpl : IServiceLocator
        {
            /// <summary>
            /// Lock
            /// </summary>
            private readonly object _lock = new object();

            /// <summary>
            /// Service container
            /// </summary>
            private readonly Dictionary<Type,object> _serviceContainer = new Dictionary<Type, object>();

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
                    if (_serviceContainer.ContainsKey(type))
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
                    if (Exists(type))
                        Remove(type);
                    _serviceContainer.Add(type, value);
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
                    if (Exists(typeof(T)))
                        Remove(typeof(T));
                    _serviceContainer.Add(typeof(T), value);
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
                            _serviceContainer.Remove(type);
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
                    object returningObject;
                    return _serviceContainer.TryGetValue(serviceType, out returningObject) 
                        ? returningObject 
                        : DynamicLoadAndAdd(serviceType);
                }
            }

            /// <summary>
            /// This locates a lazy service record for a give type.
            /// </summary>
            /// <param name="type">Type to search for</param>
            /// <returns>Lazy object or null</returns>
            private object CheckLocatedServices(Type type)
            {
                return DynamicComposer.Instance.GetExportedValue(type);
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
                    _serviceContainer.Add(serviceType, service);
                    return service;
                }
                return null;
            }
        }
    }
}