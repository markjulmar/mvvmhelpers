using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using JulMar.Core.Internal;
using JulMar.Windows.Interfaces;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage;

namespace JulMar.Windows.Services
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
    [DefaultExport(typeof(IStateManager))]
    public sealed class StateManager : IStateManager
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public StateManager()
        {
            _values = new Dictionary<string, Dictionary<string,object>>();
            KnownTypes = new List<Type>();
            Filename = "_appState.xml";
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
            if (!_values.TryGetValue(key, out values))
            {
                if (create) { 
                    values = new Dictionary<string, object>();    
                    _values.Add(key, values);
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
            return _values.Remove(key);
        }

        /// <summary>
        /// Loads an object's state from a given key in the dictionary
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="viewModel">ViewModel object</param>
        public void LoadObject(string key, object viewModel)
        {
            Load(GetDictionary(key, false), viewModel);
        }

        /// <summary>
        /// Saves an object's state into the dictionary using a given key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="viewModel"></param>
        public void SaveObject(string key, object viewModel)
        {
            Save(GetDictionary(key), viewModel);
        }

        /// <summary>
        /// Restore the state from persistent storage
        /// </summary>
        /// <returns></returns>
        public async Task<bool> LoadAsync()
        {
            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(Filename);
                using (var fs = await file.OpenSequentialReadAsync())
                {
                    var ms = new MemoryStream();
                    var provider = new DataProtectionProvider("LOCAL=user");
                    await provider.UnprotectStreamAsync(fs, ms.AsOutputStream());
                    ms.Seek(0, SeekOrigin.Begin);

                    var knownTypes = KnownTypes ?? Enumerable.Empty<Type>();
                    var serializer = new DataContractSerializer(_values.GetType(), knownTypes);

                    XDocument doc = XDocument.Load(ms);
                    _values = (Dictionary<string, Dictionary<string, object>>)serializer.ReadObject(doc.Root.CreateReader());
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
                var knownTypes = KnownTypes ?? Enumerable.Empty<Type>();
                var serializer = new DataContractSerializer(_values.GetType(), knownTypes);

                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(Filename, CreationCollisionOption.ReplaceExisting);
                using (var sw = new MemoryStream())
                {
                    serializer.WriteObject(sw, _values);
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
        private static void Load(IDictionary<string, object> stateDictionary, object viewModel)
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
            foreach (PropertyInfo propertyInfo in viewModel.GetType().GetRuntimeProperties()
                    .Where(c => c.GetCustomAttribute(typeof(ViewModelStateAttribute)) != null))
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
        private static void Save(IDictionary<string, object> stateDictionary, object viewModel)
        {
            if (stateDictionary == null || viewModel == null)
                return;

            var vmsm = viewModel as IViewModelStateManagement;
            if (vmsm != null && vmsm.SaveState(stateDictionary) == false)
            {
                // Loaded by interface implementation
                return;
            }

            foreach (PropertyInfo propertyInfo in viewModel.GetType().GetRuntimeProperties().Where(c => c.GetCustomAttribute(typeof(ViewModelStateAttribute)) != null))
            {
                stateDictionary.Add(propertyInfo.Name, propertyInfo.GetValue(viewModel));
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
