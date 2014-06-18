using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JulMar.Core;
using JulMar.Interfaces;
using JulMar.Internal;

namespace JulMar.Services
{
    /// <summary>
    /// This class acts as a resolver for typed services (interfaces and implementations).
    /// </summary>
    /// <example>
    /// To register a service use Add:
    /// <![CDATA[
    /// ServiceLocater.Instance.Add<IService, Service>();
    /// 
    /// To retrieve a service use Resolve:
    /// 
    /// IService svc = ServiceLocater.Instance.Resolve<IService>();
    /// ]]>
    /// </example>
    public sealed class ServiceLocater : IServiceLocater, IDisposable
    {
        static IServiceLocater instance;
        readonly Dictionary<Tuple<Type, string>, Lazy<object>> registeredServices 
            = new Dictionary<Tuple<Type, string>, Lazy<object>>();

        /// <summary>
        /// Singleton instance for default service locater
        /// </summary>
        public static IServiceLocater Instance
        {
            get
            {
                return instance ?? (instance = OnCreateServiceLocater());
            }
        }

        /// <summary>
        /// Can use this method to set a specific locater to replace the 
        /// built-in implementation. Must call this prior to using the locater.
        /// </summary>
        /// <param name="locater">Locater instance</param>
        public static void SetServiceLocater(IServiceLocater locater)
        {
            if (instance != null)
                throw new InvalidOperationException("Can only set the ServiceLocater once.");
            instance = locater;
        }

        /// <summary>
        /// Creates the service locater
        /// </summary>
        /// <returns></returns>
        private static ServiceLocater OnCreateServiceLocater()
        {
            var locator = new ServiceLocater();
            locator.RegisterAssemblyTypes();
            return locator;
        }

        /// <summary>
        /// Creates a key from a type and optional name.
        /// </summary>
        /// <param name="contractType">Contract type</param>
        /// <param name="name">Name</param>
        /// <returns></returns>
        private Tuple<Type, string> MakeKey(Type contractType, string name = null)
        {
            if (name != null)
                name = name.ToLowerInvariant();

            return Tuple.Create(contractType, name);
        }

        /// <summary>
        /// Registers all the assemblies - looks for ExportService attributes
        /// </summary>
        private void RegisterAssemblyTypes()
        {
            var assemblies = PlatformHelpers.GetAssemblies();
            foreach (var asm in assemblies)
            {
                try
                {
                    var attributes = asm.GetCustomAttributes<ExportServiceAttribute>().ToList();

                    // Look for assembly level attributes.
                    foreach (var att in attributes.Where(a => !a.IsFallback))
                    {
                        Add(att.ContractType, att.ServiceType);
                    }

                    // Register fall back services if no other service is in place.
                    foreach (var att in attributes.Where(a => a.IsFallback))
                    {
                        if (!this.Exists(att.ContractType))
                            Add(att.ContractType, att.ServiceType);
                    }

                }
                catch
                {
                    // Skip.
                }
            }
        }

        /// <summary>
        /// This adds a new service to the resolver list.
        /// </summary>
        /// <typeparam name="T">Type to create</typeparam>
        /// <param name="name">Name for the registration</param>
        public void Add<T>(string name) where T : class, new()
        {
            Add(name, typeof(T), typeof(T));
        }

        /// <summary>
        /// Add a new contract + service implementation
        /// </summary>
        /// <param name="name">Optional name for multiples</param>
        /// <param name="contractType">Contract type</param>
        /// <param name="serviceType">Service type</param>
        public void Add(string name, Type contractType, Type serviceType)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (contractType == null)
                throw new ArgumentNullException("contractType");
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            this.InternalAdd(name, contractType, serviceType);
        }

        /// <summary>
        /// Adds a new service to the resolver list
        /// </summary>
        /// <param name="type">Service Type (typically an interface)</param>
        /// <param name="value">Object that implements service</param>
        public void Add(Type type, object value)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (value == null)
                throw new ArgumentNullException("value");

            InternalAdd(null, type, value);
        }

        /// <summary>
        /// Adds a new service to the resolver list
        /// </summary>
        /// <param name="name">Name for the registration</param>
        /// <param name="type">Service Type (typically an interface)</param>
        /// <param name="value">Object that implements service</param>
        public void Add(string name, Type type, object value)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (type == null)
                throw new ArgumentNullException("type");
            if (value == null)
                throw new ArgumentNullException("value");

            InternalAdd(name, type, value);
        }

        /// <summary>
        /// Add a new contract + service implementation
        /// </summary>
        /// <typeparam name="TContract">Contract type</typeparam>
        /// <typeparam name="TService">Service type</typeparam>
        public void Add<TContract, TService>() 
            where TService : class, new() 
            where TContract : class
        {
            InternalAdd(null, typeof(TContract), typeof(TService));
        }

        /// <summary>
        /// Add a new contract + service implementation
        /// </summary>
        /// <typeparam name="TContract">Contract type</typeparam>
        /// <typeparam name="TService">Service type</typeparam>
        /// <param name="name">Optional name for multiples</param>
        public void Add<TContract, TService>(string name) where TContract : class where TService : class, new()
        {
            if (name == null)
                throw new ArgumentNullException("name");
            
            InternalAdd(name, typeof(TContract), typeof(TService));
        }

        /// <summary>
        /// Add a new contract + service implementation
        /// </summary>
        /// <param name="contractType">Contract type</param>
        /// <param name="serviceType">Service type</param>
        public void Add(Type contractType, Type serviceType)
        {
            if (contractType == null)
                throw new ArgumentNullException("contractType");
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");
            InternalAdd(null, contractType, serviceType);
        }

        /// <summary>
        /// This registers a new service by type, with a creator callback
        /// to generate the service at runtime.
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="creatorCallback">Callback to create the service</param>
        public void Add(Type type, Func<object> creatorCallback)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (creatorCallback == null)
                throw new ArgumentNullException("creatorCallback");
            
            InternalAdd(null, type, creatorCallback);
        }

        /// <summary>
        /// This registers a new service by type, with a creator callback
        /// to generate the service at runtime.
        /// </summary>
        /// <param name="name">Name for the registration</param>
        /// <param name="type">Type</param>
        /// <param name="creatorCallback">Callback to create the service</param>
        public void Add(string name, Type type, Func<object> creatorCallback)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (type == null)
                throw new ArgumentNullException("type");
            if (creatorCallback == null)
                throw new ArgumentNullException("creatorCallback");

            InternalAdd(name, type, creatorCallback);
        }

        /// <summary>
        /// This registers a new service by type with a creator callback.
        /// </summary>
        /// <typeparam name="T">Type to generate</typeparam>
        /// <param name="creatorCallback">Creator callback</param>
        public void Add<T>(Func<T> creatorCallback) 
            where T : class
        {
            if (creatorCallback == null)
                throw new ArgumentNullException("creatorCallback");
            
            InternalAdd(null, typeof(T), creatorCallback);
        }

        /// <summary>
        /// This registers a new service by type with a creator callback.
        /// </summary>
        /// <typeparam name="T">Type to generate</typeparam>
        /// <param name="name">Name for the registration</param>
        /// <param name="creatorCallback">Creator callback</param>
        public void Add<T>(string name, Func<T> creatorCallback) where T : class
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (creatorCallback == null)
                throw new ArgumentNullException("creatorCallback");

            InternalAdd(name, typeof(T), creatorCallback);
        }

        /// <summary>
        /// This adds a new service to the resolver list.
        /// </summary>
        /// <typeparam name="T">Type of the service</typeparam>
        /// <param name="value">Value</param>
        public void Add<T>(object value) 
            where T : class
        {
            if (value == null)
                throw new ArgumentNullException("value");
            
            InternalAdd(null, typeof(T), value);
        }

        /// <summary>
        /// This adds a new service to the resolver list.
        /// </summary>
        /// <typeparam name="T">Type of the service</typeparam>
        /// <param name="name">Name for the registration</param>
        /// <param name="value">Value</param>
        public void Add<T>(string name, object value) where T : class
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (value == null)
                throw new ArgumentNullException("value");

            InternalAdd(name, typeof(T), value);
        }

        /// <summary>
        /// This adds a new service to the resolver list.
        /// </summary>
        /// <typeparam name="T">Type to create</typeparam>
        public void Add<T>() 
            where T : class, new()
        {
            InternalAdd(null, typeof(T), typeof(T));
        }

        /// <summary>
        /// Internal method to add a type with default constructor.
        /// </summary>
        /// <param name="name">Name for registered type</param>
        /// <param name="contractType">Contract Type</param>
        /// <param name="serviceType">Service Type to create</param>
        private void InternalAdd(string name, Type contractType, Type serviceType)
        {
            var key = this.MakeKey(contractType, name);
            lock (registeredServices)
            {
                if (this.registeredServices.ContainsKey(key))
                    throw new InvalidOperationException("Service '" + name + "' of type " + contractType.Name + " is already registered.");

                this.registeredServices.Add(key, new Lazy<object>(() => Activator.CreateInstance(serviceType)));
            }
        }

        /// <summary>
        /// Adds a new service to the resolver list with a specific instance.
        /// </summary>
        /// <param name="name">Name for the registration</param>
        /// <param name="type">Service Type (typically an interface)</param>
        /// <param name="value">Object that implements service</param>
        private void InternalAdd(string name, Type type, object value)
        {
            var key = this.MakeKey(type, name);
            lock (registeredServices)
            {
                if (this.registeredServices.ContainsKey(key))
                    throw new InvalidOperationException("Service '" + name + "' of type " + type.Name + " is already registered.");

                this.registeredServices.Add(key, new Lazy<object>(() => value));
            }
        }

        /// <summary>
        /// This registers a new service by type, with a creator callback
        /// to generate the service at runtime.
        /// </summary>
        /// <param name="name">Name for the registration</param>
        /// <param name="type">Type</param>
        /// <param name="creatorCallback">Callback to create the service</param>
        private void InternalAdd(string name, Type type, Func<object> creatorCallback)
        {
            var key = this.MakeKey(type, name);
            lock (registeredServices)
            {
                if (this.registeredServices.ContainsKey(key))
                    throw new InvalidOperationException("Service '" + name + "' of type " + type.Name + " is already registered.");
                
                this.registeredServices.Add(key, new Lazy<object>(creatorCallback));
            }
        }

        /// <summary>
        /// Returns whether the service exists.
        /// </summary>
        /// <param name="contractType">Type</param>
        /// <returns>True/False</returns>
        public bool Exists(Type contractType)
        {
            lock (registeredServices)
            {
                return this.registeredServices.ContainsKey(this.MakeKey(contractType));
            }
        }

        /// <summary>
        /// Returns whether the service exists.
        /// </summary>
        /// <param name="name">Name for the service</param>
        /// <param name="contractType">Type</param>
        /// <returns>True/False</returns>
        public bool Exists(string name, Type contractType)
        {
            lock (registeredServices)
            {
                return this.registeredServices.ContainsKey(this.MakeKey(contractType, name));
            }
        }

        /// <summary>
        /// Remove a service with no name.
        /// </summary>
        /// <param name="contractType">Type to remove</param>
        public void Remove(Type contractType)
        {
            if (contractType == null)
                throw new ArgumentNullException("contractType");

            InternalRemove(null, contractType);
        }

        /// <summary>
        /// Remove a service with a name.
        /// </summary>
        /// <param name="name">Registered name</param>
        /// <param name="contractType">Type to remove</param>
        public void Remove(string name, Type contractType)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (contractType == null)
                throw new ArgumentNullException("contractType");

            this.InternalRemove(name, contractType);
        }

        /// <summary>
        /// Remove a contract type
        /// </summary>
        /// <typeparam name="T">Contract type</typeparam>
        public void Remove<T>() 
            where T : class
        {
            this.InternalRemove(null, typeof(T));
        }

        /// <summary>
        /// Remove a contract type
        /// </summary>
        /// <param name="name">Registered name</param>
        /// <typeparam name="T">Contract type</typeparam>
        public void Remove<T>(string name) where T : class
        {
            if (name == null)
                throw new ArgumentNullException("name");

            this.InternalRemove(name, typeof(T));
        }

        /// <summary>
        /// Removes a service with optional name
        /// </summary>
        /// <param name="name">Registered name</param>
        /// <param name="contractType">Contract type</param>
        private void InternalRemove(string name, Type contractType)
        {
            var key = this.MakeKey(contractType, name);
            lock (registeredServices)
            {
                this.registeredServices.Remove(key);
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
            return (T) this.InternalResolve(null, typeof(T));
        }

        /// <summary>
        /// This resolves a service type and returns the implementation. Note that this
        /// assumes the key used to register the object is of the appropriate type or
        /// this method will throw an InvalidCastException!
        /// </summary>
        /// <param name="name">Name for registration</param>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>Implementation</returns>
        public T Resolve<T>(string name) where T : class
        {
            if (name == null)
                throw new ArgumentNullException("name");

            return (T) this.InternalResolve(name, typeof(T));
        }

        /// <summary>
        /// Locate and return the service by the contract type
        /// </summary>
        /// <param name="contractType">Contract type to lookup</param>
        /// <returns></returns>
        public object Resolve(Type contractType)
        {
            if (contractType == null)
                throw new ArgumentNullException("contractType");

            return this.InternalResolve(null, contractType);
        }

        /// <summary>
        /// This resolves a service type and returns the implementation. 
        /// </summary>
        /// <param name="name">Name for registration</param>
        /// <param name="type">Type to resolve</param>
        /// <returns>Implementation</returns>
        public object Resolve(string name, Type type)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (type == null)
                throw new ArgumentNullException("type");

            return this.InternalResolve(name, type);
        }

        /// <summary>
        /// This resolves a service type and returns the implementation. 
        /// </summary>
        /// <param name="name">Name for registration</param>
        /// <param name="type">Type to resolve</param>
        /// <returns>Implementation</returns>
        private object InternalResolve(string name, Type type)
        {
            Lazy<object> value;
            lock (registeredServices)
            {
                this.registeredServices.TryGetValue(MakeKey(type, name), out value);
            }

            return value != null
                ? value.Value
                : null;
        }

        /// <summary>
        /// Resolve all the types for a given contract with no regard
        /// to the name.
        /// </summary>
        /// <param name="contractType">Contract type</param>
        /// <returns>Enumerable list of services for contract</returns>
        public IEnumerable<object> ResolveAll(Type contractType)
        {
            List<KeyValuePair<Tuple<Type, string>, Lazy<object>>> values;
            lock (registeredServices)
            {
                values = registeredServices.ToList();
            }

            if (values != null)
            {
                foreach (var item in values)
                {
                    if (item.Key.Item1 == contractType)
                    {
                        yield return item.Value.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Resolve all the types for a given contract
        /// </summary>
        /// <typeparam name="T">Contract type</typeparam>
        /// <returns></returns>
        public IEnumerable<T> ResolveAll<T>() where T : class
        {
            List<KeyValuePair<Tuple<Type, string>, Lazy<object>>> values;
            lock (registeredServices)
            {
                values = registeredServices.ToList();
            }

            if (values != null)
            {
                foreach (var item in values)
                {
                    if (item.Key.Item1 == typeof(T))
                    {
                        yield return (T) item.Value.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Dispose the container.
        /// </summary>
        public void Dispose()
        {
            lock (registeredServices)
            {
                registeredServices.Clear();
            }
        }
    }
}