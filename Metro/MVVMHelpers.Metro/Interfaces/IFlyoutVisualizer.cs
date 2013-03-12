using System;

namespace JulMar.Windows.Interfaces
{
    /// <summary>
    /// Flyout visualization service.
    /// </summary>
    public interface IFlyoutVisualizer
    {
        /// <summary>
        /// Show the flyout associated with the given key
        /// </summary>
        /// <param name="key"></param>
        void Show(string key);

        /// <summary>
        /// Show the flyout associated with the given key, using the passed DataContext
        /// with optional open/close actions.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataContext"></param>
        /// <param name="opened"></param>
        /// <param name="closed"></param>
        void Show(string key, object dataContext, Action opened, Action closed);
    }
}
