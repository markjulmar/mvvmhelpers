using System.Collections.Generic;
using System.Threading.Tasks;

namespace JulMar.Windows.Interfaces
{
    /// <summary>
    /// State manager service.
    /// </summary>
    public interface IStateManager
    {
        /// <summary>
        /// Filename to use for storage
        /// </summary>
        string Filename { get; set; }

        /// <summary>
        /// Retrieve the state dictionary for a given key
        /// </summary>
        IDictionary<string, object> GetDictionary(string key, bool create = true);

        /// <summary>
        /// Removes a state dictionary for a given key
        /// </summary>
        /// <param name="key"></param>
        bool RemoveDictionary(string key);

        /// <summary>
        /// Loads an object's state from a given key in the dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="viewModel"></param>
        void LoadObject(string key, object viewModel);

        /// <summary>
        /// Saves an object's state into the dictionary using a given key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="viewModel"></param>
        void SaveObject(string key, object viewModel);

        /// <summary>
        /// Restore the state from persistent storage
        /// </summary>
        /// <returns></returns>
        Task<bool> LoadAsync();

        /// <summary>
        /// Save the state to persistent storage
        /// </summary>
        /// <returns></returns>
        Task SaveAsync();
    }
}