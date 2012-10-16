using System;

namespace JulMar.Windows.Interfaces
{
    /// <summary>
    /// This interface is used as the navigation services to switch pages in and out
    /// </summary>
    public interface IPageNavigator
    {
        /// <summary>
        /// Returns the current page key (if known)
        /// </summary>
        string CurrentPageKey { get; }

        /// <summary>
        /// Used to register a page with a key through code
        /// </summary>
        /// <param name="key">Page key</param>
        /// <param name="type">Page type</param>
        void RegisterPage(string key, Type type);

        /// <summary>
        /// Un-register a page.
        /// </summary>
        /// <param name="key">Page key to remove</param>
        bool UnregisterPage(string key);

        /// <summary>
        /// Navigate to a specific page
        /// </summary>
        /// <param name="pageKey">Page key</param>
        bool NavigateTo(string pageKey);

        /// <summary>
        /// Navigate to a specific page
        /// </summary>
        /// <param name="pageKey">Page key</param>
        /// <param name="argument">Argument to pass (primitive type, may be null)</param>
        bool NavigateTo(string pageKey, object argument);

        /// <summary>
        /// Navigate to a specific page, passing parameters
        /// </summary>
        /// <param name="pageKey">Page key</param>
        /// <param name="argument">Argument to pass (primitive type, may be null)</param>
        /// <param name="viewModel">ViewModel to assign (may be null)</param>
        bool NavigateTo(string pageKey, object argument, object viewModel);

        /// <summary>
        /// Navigate to a specific page
        /// </summary>
        /// <param name="pageType">Page Type</param>
        bool NavigateTo(Type pageType);

        /// <summary>
        /// Navigate to a specific page
        /// </summary>
        /// <param name="pageType">Page Type</param>
        /// <param name="argument">Argument to pass (primitive type, may be null)</param>
        bool NavigateTo(Type pageType, object argument);

        /// <summary>
        /// Navigate to a specific page, passing parameters
        /// </summary>
        /// <param name="pageType">Page Type</param>
        /// <param name="argument">Argument to pass (primitive type, may be null)</param>
        /// <param name="viewModel">ViewModel to assign (may be null)</param>
        bool NavigateTo(Type pageType, object argument, object viewModel);

        /// <summary>
        /// Go backward in the navigation chain
        /// </summary>
        bool GoBack();
        
        /// <summary>
        /// Return whether there is a page behind the current page in the navigation service
        /// </summary>
        bool CanGoBack { get; }

        /// <summary>
        /// Go forward in the navigation chain
        /// </summary>
        bool GoForward();

        /// <summary>
        /// Return whether there is a page ahead the current page in the navigation service
        /// </summary>
        bool CanGoForward { get; }
    }
}
