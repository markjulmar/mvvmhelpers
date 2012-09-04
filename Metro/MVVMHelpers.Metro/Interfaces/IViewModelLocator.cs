namespace JulMar.Core.Interfaces
{
    /// <summary>
    /// ViewModel locator interfaces
    /// </summary>
    public interface IViewModelLocator
    {
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