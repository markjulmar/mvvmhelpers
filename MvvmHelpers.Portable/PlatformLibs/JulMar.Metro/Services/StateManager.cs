using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

using Windows.Security.Cryptography.DataProtection;
using Windows.Storage;

using JulMar.Core;
using JulMar.Interfaces;
using JulMar.Services;

[assembly: ExportService(typeof(IStateManager), typeof(StateManager), IsFallback = true)]

namespace JulMar.Services
{
    /// <summary>
    /// Attribute used to mark properties which should be persisted to dictionary.
    /// The properties MUST be public and have public setters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ViewModelStateAttribute : Attribute
    {
    }

    /// <summary>
    /// State Manager class to load and save a ViewModel to a dictionary
    /// </summary>
    public sealed class StateManager : IStateManager
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public StateManager()
        {
            this._values = new Dictionary<string, Dictionary<string,object>>();
            this.KnownTypes = new List<Type>();
            this.Filename = "_appState.xml";
        }

        /// <summary>
        /// Filename to use for storage
        /// </summary>
        public string Filename
        {
            get; set;
        }

        /// <summary>
        /// Values collection
        /// </summary>
        private Dictionary<string, Dictionary<string, object>> _values;

        /// <summary>
        /// List of known types
        /// </summary>
        public IList<Type> KnownTypes { get; private set; } 

        /// <summary>
        /// Returns the state-specific dictionary for a key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        public IDictionary<string,object> GetDictionary(string key, bool create = true)
        {
            Dictionary<string, object> values = null;
            if (!this._values.TryGetValue(key, out values))
            {
                if (create) { 
                    values = new Dictionary<string, object>();    
                    this._values.Add(key, values);
                }
            }
            return values;
        }

        /// <summary>
        /// Removes the given state dictionary based on the key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Success code</returns>
        public bool RemoveDictionary(string key)
        {
            return this._values.Remove(key);
        }

        /// <summary>
        /// Loads an object's state from a given key in the dictionary
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="viewModel">ViewModel object</param>
        public void LoadObject(string key, object viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException("viewModel");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            var properties = viewModel.GetType().GetRuntimeProperties()
                .Where(c => c.GetCustomAttribute(typeof(ViewModelStateAttribute)) != null)
                .ToArray();

            // Check for the support to save
            if (viewModel is IViewModelStateManagement
                || properties.Length > 0)
                Load(this.GetDictionary(key, false), viewModel, properties);
        }

        /// <summary>
        /// Saves an object's state into the dictionary using a given key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="viewModel"></param>
        public void SaveObject(string key, object viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException("viewModel");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            var properties = viewModel.GetType().GetRuntimeProperties()
                .Where(c => c.GetCustomAttribute(typeof (ViewModelStateAttribute)) != null)
                .ToArray();

            // Check for the support to save
            if (viewModel is IViewModelStateManagement
                || properties.Length > 0)
                Save(this.GetDictionary(key), viewModel, properties);
        }

        /// <summary>
        /// Restore the state from persistent storage
        /// </summary>
        /// <returns></returns>
        public async Task<bool> LoadAsync()
        {
            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(this.Filename);
                using (var fs = await file.OpenSequentialReadAsync())
                {
                    var ms = new MemoryStream();
                    var provider = new DataProtectionProvider("LOCAL=user");
                    await provider.UnprotectStreamAsync(fs, ms.AsOutputStream());
                    ms.Seek(0, SeekOrigin.Begin);

                    var knownTypes = this.KnownTypes ?? Enumerable.Empty<Type>();
                    var serializer = new DataContractSerializer(this._values.GetType(), knownTypes);

                    XDocument doc = XDocument.Load(ms);
                    this._values = (Dictionary<string, Dictionary<string, object>>)serializer.ReadObject(doc.Root.CreateReader());
                    return true;
                }
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            catch (Exception ex)
            {
                throw new StateManagerException("Failed to load persisted state", ex);
            }
        }

        /// <summary>
        /// Encrypts and saves the state to persistent storage
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            try
            {
                var knownTypes = this.KnownTypes ?? Enumerable.Empty<Type>();
                var serializer = new DataContractSerializer(this._values.GetType(), knownTypes);

                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(this.Filename, CreationCollisionOption.ReplaceExisting);
                using (var sw = new MemoryStream())
                {
                    serializer.WriteObject(sw, this._values);
                    sw.Position = 0;
                    var provider = new DataProtectionProvider("LOCAL=user");

                    // Encrypt the session data and write it to disk.
                    using (var fs = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await provider.ProtectStreamAsync(sw.AsInputStream(), fs);
                        await fs.FlushAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new StateManagerException("Failed to save state.", ex);
            }
        }

        /// <summary>
        /// Loads the state of an object (ViewModel) from the state dictionary.
        /// </summary>
        /// <param name="stateDictionary"></param>
        /// <param name="viewModel"></param>
        /// <param name="properties"></param>
        private static void Load(IDictionary<string, object> stateDictionary, object viewModel, IEnumerable<PropertyInfo> properties)
        {
            if (stateDictionary == null || viewModel == null)
                return;

            var vmsm = viewModel as IViewModelStateManagement;
            if (vmsm != null && vmsm.RestoreState(stateDictionary) == false)
            {
                // Loaded by interface implementation
                return;
            }

            // Otherwise use reflection to pull all the properties.
            foreach (PropertyInfo propertyInfo in properties)
            {
                if (stateDictionary.ContainsKey(propertyInfo.Name))
                    propertyInfo.SetValue(viewModel, stateDictionary[propertyInfo.Name]);
            }
        }

        /// <summary>
        /// Saves a ViewModel into a child dictionary using a specific key
        /// </summary>
        /// <param name="stateDictionary"></param>
        /// <param name="viewModel"></param>
        /// <param name="properties"></param>
        private static void Save(IDictionary<string, object> stateDictionary, object viewModel, IEnumerable<PropertyInfo> properties)
        {
            if (stateDictionary == null || viewModel == null)
                return;

            var vmsm = viewModel as IViewModelStateManagement;
            if (vmsm != null && vmsm.SaveState(stateDictionary) == false)
            {
                // Loaded by interface implementation
                return;
            }

            foreach (PropertyInfo propertyInfo in properties)
            {
                stateDictionary[propertyInfo.Name] = propertyInfo.GetValue(viewModel);
            }
        }
    }

    /// <summary>
    /// Exception used when the state manager fails to load or save the settings.
    /// </summary>
    public class StateManagerException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public StateManagerException() : base("StateManager exception")
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public StateManagerException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public StateManagerException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
