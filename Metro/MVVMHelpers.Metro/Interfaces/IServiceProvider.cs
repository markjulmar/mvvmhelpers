using System;

namespace JulMar.Core.Interfaces
{
    /// <summary>
    /// Interface that defines Add/Remove and typesafe Resolve
    /// </summary>
    public interface IServiceProviderEx : IServiceProvider
    {
        /// <summary>
        /// Returns whether the service exists.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>True/False</returns>
        bool Exists(Type type);

        /// <summary>
        /// Adds a new service to the resolver list
        /// </summary>
        /// <param name="type">Service Type (typically an interface)</param>
        /// <param name="value">Object that implements service</param>
        void Add(Type type, object value);

        /// <summary>
        /// This adds a new service to the resolver list.
        /// </summary>
        /// <typeparam name="T">Type of the service</typeparam>
        /// <param name="value">Value</param>
        void Add<T>(T value);

        /// <summary>
        /// Remove a service
        /// </summary>
        /// <param name="type">Type to remove</param>
        void Remove(Type type);

        /// <summary>
        /// This resolves a service type and returns the implementation. Note that this
        /// assumes the key used to register the object is of the appropriate type or
        /// this method will throw an InvalidCastException!
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>Implementation</returns>
        T Resolve<T>();
    }
}