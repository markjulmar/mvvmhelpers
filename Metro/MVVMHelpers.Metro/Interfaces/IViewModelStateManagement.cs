using System.Collections.Generic;

namespace JulMar.Windows.Interfaces
{
    /// <summary>
    /// This interface allows a ViewModel to take control of serialization
    /// into the state dictionary when the StateManager below is used. This is 
    /// only used when LoadObject/SaveObject is used.
    /// </summary>
    public interface IViewModelStateManagement
    {
        /// <summary>
        /// Called to save custom state in the supplied dictionary.
        /// </summary>
        /// <param name="stateDictionary">Dictionary to save state into</param>
        /// <returns>True to continue saving object using reflection, false to stop (if all state is persisted through custom mechanism)</returns>
        bool SaveState(IDictionary<string, object> stateDictionary);

        /// <summary>
        /// Called to restore custom state from the supplied dictionary.
        /// </summary>
        /// <param name="stateDictionary">Dictionary to restore state from</param>
        /// <returns>True to continue restoring object using reflection, false to stop (if all state is persisted through custom mechanism)</returns>
        bool RestoreState(IDictionary<string, object> stateDictionary);
    }
}