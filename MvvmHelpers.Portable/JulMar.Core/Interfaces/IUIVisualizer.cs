using System;
using System.Threading.Tasks;

namespace JulMar.Interfaces
{
    /// <summary>
    /// This interface defines a UI controller which can be used to display dialogs
    /// in either modal or modeless form from a ViewModel.
    /// </summary>
    public interface IUIVisualizer
    {
        /// <summary>
        /// Registers a type through a key.
        /// </summary>
        /// <param name="key">Key for the UI dialog</param>
        /// <param name="winType">Type which implements dialog</param>
        void Add(string key, Type winType);

        /// <summary>
        /// This unregisters a type and removes it from the mapping
        /// </summary>
        /// <param name="key">Key to remove</param>
        /// <returns>True/False success</returns>
        bool Remove(string key);

        /// <summary>
        /// This method displays a modeless dialog associated with the given key.  The associated
        /// VM is not connected but must be supplied through some other means.
        /// </summary>
        /// <param name="key">Key previously registered with the UI controller.</param>
        /// <param name="setOwner">Set the owner of the window</param>
        /// <returns>True/False if UI is displayed</returns>
        Task ShowAsync(string key, bool setOwner);

        /// <summary>
        /// This method displays a modal dialog associated with the given key.
        /// </summary>
        /// <param name="key">Key previously registered with the UI controller.</param>
        /// <returns>True/False if UI is displayed.</returns>
        Task<bool?> ShowDialogAsync(string key);

        /// <summary>
        /// This method displays a modeless dialog associated with the given key.
        /// </summary>
        /// <param name="key">Key previously registered with the UI controller.</param>
        /// <param name="state">Object state to associate with the dialog</param>
        /// <param name="setOwner">Set the owner of the window</param>
        /// <returns>True/False if UI is displayed</returns>
        Task ShowAsync(string key, object state, bool setOwner);

        /// <summary>
        /// This method displays a modal dialog associated with the given key.
        /// </summary>
        /// <param name="key">Key previously registered with the UI controller.</param>
        /// <param name="state">Object state to associate with the dialog</param>
        /// <returns>True/False if UI is displayed.</returns>
        Task<bool?> ShowDialogAsync(string key, object state);
    }
}