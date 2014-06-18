using System;

namespace JulMar.Interfaces
{
    /// <summary>
    /// ViewModel locater interfaces
    /// </summary>
    public interface IViewModelLocater
    {
        /// <summary>
        /// Add a ViewModel
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="vm">ViewModel</param>
        void Add(string key, object vm);

        /// <summary>
        /// Add a type for a ViewModel
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="vmType">Type to create</param>
        void Add(string key, Type vmType);

        /// <summary>
        /// Add a type for a ViewModel.
        /// </summary>
        /// <typeparam name="T">View Model Type</typeparam>
        /// <param name="key">Key</param>
        void Add<T>(string key) where T : new();

        /// <summary>
        /// Remove a key
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);

        /// <summary>
        /// Operator to retrieve view models.
        /// </summary>
        /// <returns>Read-only version of view model collection</returns>
        object this[string key] { get; }

        /// <summary>
        /// Finds the VM based on the key.
        /// </summary>
        /// <param name="key">Key to search for</param>
        /// <returns>Located view model or null</returns>
        object Locate(string key);

        /// <summary>
        /// Finds the VM based on the key.
        /// </summary>
        /// <param name="key">Key to search for</param>
        /// <param name="returnValue">Located view model or null</param>
        /// <returns>true/false if VM was found</returns>
        bool TryLocate(string key, out object returnValue);
    }
}