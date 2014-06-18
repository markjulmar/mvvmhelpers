using System;
using System.Collections.Generic;
using System.Reflection;

using JulMar.Core;
using JulMar.Interfaces;
using JulMar.Internal;
using JulMar.Services;

[assembly:ExportService(typeof(IViewModelLocater), typeof(ViewModelLocaterService), IsFallback = true)]

namespace JulMar.Services
{
    /// <summary>
    /// This class holds ViewModels that are registered with the ExportViewModelAttribute.
    /// </summary>
    sealed class ViewModelLocaterService : IViewModelLocater
    {
        private bool initialized;
        private readonly Dictionary<string,Lazy<object>> registeredViewModels = new Dictionary<string, Lazy<object>>();

        /// <summary>
        /// Add a ViewModel
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="vm">ViewModel</param>
        public void Add(string key, object vm)
        {
            if (this.registeredViewModels.ContainsKey(key))
                throw new InvalidOperationException("Key '" + key + "' already exists in ViewModel Locator.");

            this.registeredViewModels.Add(key, new Lazy<object>(() => vm));
        }

        /// <summary>
        /// Add a type for a ViewModel
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="vmType">Type to create</param>
        public void Add(string key, Type vmType)
        {
            if (this.registeredViewModels.ContainsKey(key))
                throw new InvalidOperationException("Key '" + key + "' already exists in ViewModel Locator.");

            this.registeredViewModels.Add(key, 
                new Lazy<object>(() => Activator.CreateInstance(vmType)));
        }

        /// <summary>
        /// Add a type for a ViewModel.
        /// </summary>
        /// <typeparam name="T">View Model Type</typeparam>
        /// <param name="key">Key</param>
        public void Add<T>(string key) where T : new()
        {
            if (this.registeredViewModels.ContainsKey(key))
                throw new InvalidOperationException("Key '" + key + "' already exists in ViewModel Locator.");

            this.registeredViewModels.Add(key, new Lazy<object>(() => Activator.CreateInstance<T>()));
        }

        /// <summary>
        /// Remove a key
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            this.registeredViewModels.Remove(key);
        }

        /// <summary>
        /// Operator to retrieve view models.
        /// </summary>
        /// <returns>Read-only version of view model collection</returns>
        public object this[string key]
        {
            get 
            { 
                object value;
                return this.TryLocate(key, out value) ? value : null;
            }
        }

        /// <summary>
        /// Finds the VM based on the key.
        /// </summary>
        /// <param name="key">Key to search for</param>
        /// <returns>Located view model or null</returns>
        public object Locate(string key)
        {
            object value;
            if (!this.TryLocate(key, out value))
                throw new Exception("Could not locate view model: " + key);
            
            return value;
        }

        /// <summary>
        /// Finds the VM based on the key.
        /// </summary>
        /// <param name="key">Key to search for</param>
        /// <param name="returnValue">Located view model or null</param>
        /// <returns>true/false if VM was found</returns>
        public bool TryLocate(string key, out object returnValue)
        {
            returnValue = null;

            // Populate our list the first call
            if (this.initialized == false)
            {
                this.Initialize();
                this.initialized = true;
            }

            // Locate the value.
            Lazy<object> tgValue;
            if (this.registeredViewModels.TryGetValue(key, out tgValue))
            {
                returnValue = tgValue.Value;
                return true;
            }

            return true;
        }

        /// <summary>
        /// This method uses an internal object to gather the list of ViewModels based
        /// on the ExportViewModel attribute.
        /// </summary>
        /// <returns></returns>
        private void Initialize()
        {
            var assemblies = PlatformHelpers.GetAssemblies();
            foreach (var asm in assemblies)
            {
                foreach (var att in asm.GetCustomAttributes<ExportViewModelAttribute>())
                {
                    Add(att.Key, att.ViewModelType);
                }
            }
        }
    }
}
