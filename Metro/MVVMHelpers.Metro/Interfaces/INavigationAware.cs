using System.Collections.Generic;

namespace JulMar.Windows.Interfaces
{
    /// <summary>
    /// This interface can be implemented by a ViewModel to save/restore state
    /// during navigation.
    /// </summary>
    public interface INavigationAware
    {
        /// <summary>
        /// Save the state of the VM
        /// </summary>
        /// <param name="state">State dictionary to save state into</param>
        void SaveState(IDictionary<string, object> state);

        /// <summary>
        /// Restore the state of the VM
        /// </summary>
        /// <param name="parameter">Navigation parameter</param>
        /// <param name="state">State Dictionary to restore from (may be null!)</param>
        void LoadState(object parameter, IDictionary<string, object> state);
    }
}