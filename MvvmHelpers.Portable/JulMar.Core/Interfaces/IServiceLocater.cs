using System;
using System.Collections.Generic;

namespace JulMar.Interfaces
{
    /// <summary>
    /// Interface that defines Add/Remove and type-safe Resolve
    /// </summary>
    public interface IServiceLocater
    {
        /// <summary>
        /// Add a new contract + service implementation
        /// </summary>
        /// <typeparam name="TContract">Contract type</typeparam>
        /// <typeparam name="TService">Service type</typeparam>
        void Add<TContract, TService>() 
            where TContract : class
            where TService : class, new();

        /// <summary>
        /// Add a new contract + service implementation
        /// </summary>
        /// <typeparam name="TContract">Contract type</typeparam>
        /// <typeparam name="TService">Service type</typeparam>
        /// <param name="name">Optional name for multiples</param>
        void Add<TContract, TService>(string name)
            where TContract : class
            where TService : class, new();

        /// <summary>
        /// Add a new contract + service implementation
        /// </summary>
        /// <param name="contractType">Contract type</param>
        /// <param name="serviceType">Service type</param>
        void Add(Type contractType, Type serviceType);

        /// <summary>
        /// Add a new contract + service implementation
        /// </summary>
        /// <param name="name">Optional name for multiples</param>
        /// <param name="contractType">Contract type</param>
        /// <param name="serviceType">Service type</param>
        void Add(string name, Type contractType, Type serviceType);

        /// <summary>
        /// Adds a new service to the resolver list
        /// </summary>
        /// <param name="type">Service Type (typically an interface)</param>
        /// <param name="value">Object that implements service</param>
        void Add(Type type, object value);

        /// <summary>
        /// Adds a new service to the resolver list
        /// </summary>
        /// <param name="name">Name for the registration</param>
        /// <param name="type">Service Type (typically an interface)</param>
        /// <param name="value">Object that implements service</param>
        void Add(string name, Type type, object value);

        /// <summary>
        /// This registers a new service by type, with a creator callback
        /// to generate the service at runtime.
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="creatorCallback">Callback to create the service</param>
        void Add(Type type, Func<object> creatorCallback);

        /// <summary>
        /// This registers a new service by type, with a creator callback
        /// to generate the service at runtime.
        /// </summary>
        /// <param name="name">Name for the registration</param>
        /// <param name="type">Type</param>
        /// <param name="creatorCallback">Callback to create the service</param>
        void Add(string name, Type type, Func<object> creatorCallback);

        /// <summary>
        /// This registers a new service by type with a creator callback.
        /// </summary>
        /// <typeparam name="T">Type to generate</typeparam>
        /// <param name="creatorCallback">Creator callback</param>
        void Add<T>(Func<T> creatorCallback) where T : class;

        /// <summary>
        /// This registers a new service by type with a creator callback.
        /// </summary>
        /// <typeparam name="T">Type to generate</typeparam>
        /// <param name="name">Name for the registration</param>
        /// <param name="creatorCallback">Creator callback</param>
        void Add<T>(string name, Func<T> creatorCallback) where T : class;

        /// <summary>
        /// This adds a new service to the resolver list.
        /// </summary>
        /// <typeparam name="T">Type of the service</typeparam>
        /// <param name="value">Value</param>
        void Add<T>(object value) where T: class;

        /// <summary>
        /// This adds a new service to the resolver list.
        /// </summary>
        /// <typeparam name="T">Type of the service</typeparam>
        /// <param name="name">Name for the registration</param>
        /// <param name="value">Value</param>
        void Add<T>(string name, object value) where T : class;

        /// <summary>
        /// This adds a new service to the resolver list.
        /// </summary>
        /// <typeparam name="T">Type to create</typeparam>
        void Add<T>() where T : class, new();

        /// <summary>
        /// This adds a new service to the resolver list.
        /// </summary>
        /// <typeparam name="T">Type to create</typeparam>
        /// <param name="name">Name for the registration</param>
        void Add<T>(string name) where T : class, new();

        /// <summary>
        /// Returns whether the service exists.
        /// </summary>
        /// <param name="contractType">Type</param>
        /// <returns>True/False</returns>
        bool Exists(Type contractType);

        /// <summary>
        /// Returns whether the service exists.
        /// </summary>
        /// <param name="name">Name for the service</param>
        /// <param name="contractType">Type</param>
        /// <returns>True/False</returns>
        bool Exists(string name, Type contractType);

        /// <summary>
        /// Remove a service
        /// </summary>
        /// <param name="contractType">Type to remove</param>
        void Remove(Type contractType);

        /// <summary>
        /// Remove a service
        /// </summary>
        /// <param name="name">Registered name</param>
        /// <param name="contractType">Type to remove</param>
        void Remove(string name, Type contractType);

        /// <summary>
        /// Remove a contract type
        /// </summary>
        /// <typeparam name="T">Contract type</typeparam>
        void Remove<T>() where T : class;

        /// <summary>
        /// Remove a contract type
        /// </summary>
        /// <param name="name">Registered name</param>
        /// <typeparam name="T">Contract type</typeparam>
        void Remove<T>(string name) where T : class;

        /// <summary>
        /// This resolves a service type and returns the implementation. Note that this
        /// assumes the key used to register the object is of the appropriate type or
        /// this method will throw an InvalidCastException!
        /// </summary>
        /// <param name="type">Type to resolve</param>
        /// <returns>Implementation</returns>
        object Resolve(Type type);

        /// <summary>
        /// This resolves a service type and returns the implementation. Note that this
        /// assumes the key used to register the object is of the appropriate type or
        /// this method will throw an InvalidCastException!
        /// </summary>
        /// <param name="name">Name for registration</param>
        /// <param name="type">Type to resolve</param>
        /// <returns>Implementation</returns>
        object Resolve(string name, Type type);

        /// <summary>
        /// This resolves a service type and returns the implementation. Note that this
        /// assumes the key used to register the object is of the appropriate type or
        /// this method will throw an InvalidCastException!
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>Implementation</returns>
        T Resolve<T>();

        /// <summary>
        /// This resolves a service type and returns the implementation. Note that this
        /// assumes the key used to register the object is of the appropriate type or
        /// this method will throw an InvalidCastException!
        /// </summary>
        /// <param name="name">Name for registration</param>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>Implementation</returns>
        T Resolve<T>(string name) where T : class;

        /// <summary>
        /// Resolves all instances of a given contract.
        /// </summary>
        /// <returns>Instances matching contract</returns>
        IEnumerable<object> ResolveAll(Type contractType);

        /// <summary>
        /// Resolves all instances of a given contract.
        /// </summary>
        /// <typeparam name="T">Contract type</typeparam>
        /// <returns>Instances matching contract</returns>
        IEnumerable<T> ResolveAll<T>() where T : class;
    }
}