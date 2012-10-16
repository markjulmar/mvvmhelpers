using System;
using System.Reflection;
using JulMar.Core.Internal;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using JulMar.Core.Services;
using JulMar.Windows.Interfaces;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.Concurrent;
using Windows.UI.Xaml.Navigation;

namespace JulMar.Windows.Services
{
    /// <summary>
    /// Class used to populate metadata used to identify views which 
    /// may be navigated to/from.
    /// </summary>
    public sealed class PageViewMetadata
    {
        /// <summary>
        /// Key used to export the view.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Type used to instantiate the view.
        /// </summary>
        public Type Type { get; set; }
    }

    /// <summary>
    /// This is used to decorate a specific page in the UI and associate
    /// it to a string key.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ExportPageAttribute : ExportAttribute
    {
        /// <summary>
        /// Key used to export the view.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Type used to instantiate the view.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">Key used to locate page</param>
        /// <param name="type">Page type</param>
        public ExportPageAttribute(string key, Type type)
            : base(PageNavigator.MefLocatorKey, typeof(Page))
        {
            Key = key;
            Type = type;
        }
    }

    /// <summary>
    /// Class used to locate views but keep property hidden
    /// </summary>
    internal sealed class PageViewData
    {
        /// <summary>
        /// Located view models
        /// </summary>
        [ImportMany(PageNavigator.MefLocatorKey)]
        public IList<Lazy<Page, PageViewMetadata>> LocatedPages { get; set; }
    }

    /// <summary>
    /// Page navigation service
    /// </summary>
    [DefaultExport(typeof (IPageNavigator))]
    internal sealed class PageNavigator : IPageNavigator
    {
        internal const string MefLocatorKey = "JulMar.PageView.Export";

        private readonly IDictionary<string, Type> _registeredPages = new ConcurrentDictionary<string, Type>();
        private bool _autoLocatedPages;

        /// <summary>
        /// The root Frame 
        /// </summary>
        private Frame RootFrame
        {
            get { return Window.Current.Content as Frame; }
        }

        /// <summary>
        /// Returns the current page key (if known)
        /// </summary>
        public string CurrentPageKey
        {
            get
            {
                Type page = RootFrame.SourcePageType;
                if (page != null)
                {
                    var export = page.GetTypeInfo().GetCustomAttribute<ExportPageAttribute>();
                    if (export != null)
                        return export.Key;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Used to register a page with a key through code
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        public void RegisterPage(string key, Type type)
        {
            if (!_registeredPages.ContainsKey(key))
                _registeredPages.Add(key, type);
            else
            {
                _registeredPages.Remove(key);
                _registeredPages.Add(key, type);
            }
        }

        /// <summary>
        /// Unregister a page.
        /// </summary>
        /// <param name="key">Page key to remove</param>
        public bool UnregisterPage(string key)
        {
            return _registeredPages.Remove(key);
        }

        /// <summary>
        /// Navigate to a specific page
        /// </summary>
        /// <param name="pageKey">Page key</param>
        public bool NavigateTo(string pageKey)
        {
            return NavigateTo(pageKey, null);
        }

        /// <summary>
        /// Navigate to a specific page, passing parameters
        /// </summary>
        /// <param name="pageKey">Page key</param>
        /// <param name="argument">Argument to pass (primitive type, may be null)</param>
        public bool NavigateTo(string pageKey, object argument)
        {
            return NavigateTo(pageKey, argument, null);
        }

        /// <summary>
        /// Navigate to a specific page, passing parameters
        /// </summary>
        /// <param name="pageKey">Page key</param>
        /// <param name="argument">Argument to pass (primitive type, may be null)</param>
        /// <param name="viewModel">ViewModel to assign (may be null)</param>
        public bool NavigateTo(string pageKey, object argument, object viewModel)
        {
            if (string.IsNullOrEmpty(pageKey))
                throw new ArgumentNullException("pageKey");

            if (_autoLocatedPages == false)
            {
                var locatedPageViews = GatherPageViewData().ToList();
                if (locatedPageViews != null && locatedPageViews.Count > 0)
                {
                    foreach (var item in locatedPageViews)
                        RegisterPage(item.Metadata.Key, item.Metadata.Type);
                }
                _autoLocatedPages = true;
            }

            Type entry;
            return _registeredPages.TryGetValue(pageKey, out entry) 
                && NavigateTo(entry, argument, viewModel);
        }

        /// <summary>
        /// Navigate to a specific page
        /// </summary>
        /// <param name="pageType">Page Type</param>
        public bool NavigateTo(Type pageType)
        {
            return NavigateTo(pageType, null, null);
        }

        /// <summary>
        /// Navigate to a specific page
        /// </summary>
        /// <param name="pageType">Page Type</param>
        /// <param name="argument">Argument to pass (primitive type, may be null)</param>
        public bool NavigateTo(Type pageType, object argument)
        {
            return NavigateTo(pageType, null, null);
        }

        /// <summary>
        /// Navigate to a specific page, passing parameters
        /// </summary>
        /// <param name="pageType">Page Type</param>
        /// <param name="argument">Argument to pass (primitive type, may be null)</param>
        /// <param name="viewModel">ViewModel to assign (may be null)</param>
        public bool NavigateTo(Type pageType, object argument, object viewModel)
        {
            if (pageType == null)
                throw new ArgumentNullException("pageType");

            if (viewModel != null)
            {
                NavigatedEventHandler connectViewModel = (s, e) =>
                {
                    if (e.SourcePageType == pageType)
                    {
                        var fe = e.Content as FrameworkElement;
                        if (fe != null)
                            fe.DataContext = viewModel;
                    }
                };

                RootFrame.Navigated += connectViewModel;
                bool rc = RootFrame.Navigate(pageType, argument);
                RootFrame.Navigated -= connectViewModel;
                return rc;
            }

            // Otherwise, just navigate
            return RootFrame.Navigate(pageType, argument);
        }

        /// <summary>
        /// Go backward in the navigation chain
        /// </summary>
        public bool GoBack()
        {
            if (!CanGoBack)
                return false;
            RootFrame.GoBack();
            return true;
        }

        /// <summary>
        /// Return whether there is a page behind the current page in the navigation service
        /// </summary>
        public bool CanGoBack
        {
            get { return RootFrame.CanGoBack; }
        }

        /// <summary>
        /// Go forward in the navigation chain
        /// </summary>
        public bool GoForward()
        {
            if (!CanGoForward)
                return false;
            RootFrame.GoForward();
            return true;
        }

        /// <summary>
        /// Return whether there is a page ahead the current page in the navigation service
        /// </summary>
        public bool CanGoForward { get { return RootFrame.CanGoForward; } }

        /// <summary>
        /// This method uses an internal object to gather the list of ViewModels based
        /// on the ExportViewModel attribute.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Lazy<Page, PageViewMetadata>> GatherPageViewData()
        {
            var data = new PageViewData();
            DynamicComposer.Instance.Compose(data);
            return data.LocatedPages;
        }
    }
}
