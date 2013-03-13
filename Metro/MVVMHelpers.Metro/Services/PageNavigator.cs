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
using System.Threading.Tasks;

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
    [DefaultExport(typeof (IPageNavigator)), Shared]
    internal sealed class PageNavigator : IPageNavigator
    {
        internal const string MefLocatorKey = "JulMar.PageView.Export";
        internal const string PageKeyPrefix = "page-";
        internal const string ViewModelKeyPrefix = "pagevm-";

        private readonly IDictionary<string, Type> _registeredPages = new ConcurrentDictionary<string, Type>();
        private bool _autoLocatedPages, _initialized;
        private object _watchForViewModel;
        private IStateManager _stateManager;

        /// <summary>
        /// The root Frame 
        /// </summary>
        private Frame RootFrame
        {
            get { return Window.Current.Content as Frame; }
        }

        private void Initialize()
        {
            if (_initialized || RootFrame == null)
                return;

            _initialized = true;

            RootFrame.Navigating += RootFrameOnNavigating;
            RootFrame.Navigated += RootFrameOnNavigated;
        }

        /// <summary>
        /// This handles the Navigated TO event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootFrameOnNavigated(object sender, NavigationEventArgs e)
        {
            FrameworkElement fe = e.Content as FrameworkElement;
            object viewModel = _watchForViewModel;
            _watchForViewModel = null;

            if (fe != null)
            {
                if (viewModel == null)
                {
                    viewModel = fe.DataContext;
                }
                else
                {
                    fe.DataContext = viewModel;
                }
            }

            // Clear any forward navigation when adding a new page
            if (e.NavigationMode == NavigationMode.New)
            {
                ClearForwardHistory();
            }

            HandleOnNavigatingTo(fe, viewModel, e.NavigationMode, e.Parameter);
        }

        /// <summary>
        /// Handles the navigation when we are moving TO a new view/viewmodel.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="viewModel"></param>
        /// <param name="navMode"></param>
        /// <param name="parameter"></param>
        private void HandleOnNavigatingTo(FrameworkElement view, object viewModel, NavigationMode navMode, object parameter)
        {
            // Load the object state
            IDictionary<string, object> stateDictionary = null;
            if (viewModel != null)
            {
                if (StateManager != null)
                {
                    string key = GenerateStateKey(ViewModelKeyPrefix);
                    StateManager.LoadObject(key, viewModel);
                    stateDictionary = StateManager.GetDictionary(key, false);
                }

                // See if the ViewModel participates in navigation-aware services.
                INavigationAware navm = viewModel as INavigationAware;
                if (navm != null)
                    navm.OnNavigatedTo(new NavigatedToEventArgs(navMode, parameter, stateDictionary));
            }

            // See if the VIEW participates in navigation-aware services.
            INavigationAware naView = view as INavigationAware;
            if (naView != null)
            {
                stateDictionary = null;
                if (StateManager != null)
                {
                    string key = GenerateStateKey(PageKeyPrefix);
                    stateDictionary = StateManager.GetDictionary(key, false);
                }
                naView.OnNavigatedTo(new NavigatedToEventArgs(navMode, parameter, stateDictionary));
            }
        }

        /// <summary>
        /// This is called when navigating FROM a page to a new page and handles the OnNavigatingFrom event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootFrameOnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            Frame frame = (Frame)sender;
            bool cancel = e.Cancel;
            HandleOnNavigatingFrom(frame, e.NavigationMode, ref cancel, false);
            e.Cancel = cancel;
        }

        /// <summary>
        /// Handles the OnNavigatingFrom event which notifies a page/ViewModel that we are
        /// moving AWAY from that view.
        /// </summary>
        /// <param name="rootFrame"></param>
        /// <param name="navMode"></param>
        /// <param name="cancel"></param>
        /// <param name="suspending"></param>
        private void HandleOnNavigatingFrom(Frame rootFrame, NavigationMode navMode, ref bool cancel, bool suspending)
        {
            if (rootFrame.Content != null)
            {
                IDictionary<string, object> stateDictionary = null;
                FrameworkElement fe = rootFrame.Content as FrameworkElement;
                if (fe != null)
                {
                    if (fe.DataContext != null)
                    {
                        // Attempt to save the object off.
                        if (StateManager != null)
                        {
                            string key = GenerateStateKey(ViewModelKeyPrefix);
                            StateManager.SaveObject(key, fe.DataContext);
                            stateDictionary = StateManager.GetDictionary(key, true);
                        }

                        // See if the ViewModel participates in navigation-aware services.
                        INavigationAware navm = fe.DataContext as INavigationAware;
                        if (navm != null)
                        {
                            var e = (suspending)
                                        ? new NavigatingFromEventArgs(navMode, cancel, stateDictionary)
                                        : new NavigatingFromEventArgs(stateDictionary);
                            navm.OnNavigatingFrom(e);
                            cancel = e.Cancel;
                        }
                    }

                    // See if the VIEW participates in navigation-aware services.
                    INavigationAware naView = fe as INavigationAware;
                    if (naView != null)
                    {
                        // Save off the VIEW
                        stateDictionary = null;
                        if (StateManager != null)
                        {
                            string key = GenerateStateKey(PageKeyPrefix);
                            stateDictionary = StateManager.GetDictionary(key, false);
                        }

                        var e = (suspending)
                                    ? new NavigatingFromEventArgs(navMode, cancel, stateDictionary)
                                    : new NavigatingFromEventArgs(stateDictionary);
                        naView.OnNavigatingFrom(e);
                        cancel = e.Cancel;
                    }
                }
            }
        }

        /// <summary>
        /// Optional state manager, if not supplied pulls the default.
        /// </summary>
        public IStateManager StateManager
        {
            get { return _stateManager ?? (_stateManager = ServiceLocator.Instance.Resolve<IStateManager>()); }
            set
            {
                if (_stateManager != null)
                    throw new InvalidOperationException("Cannot assign StateManager once it has been used.");

                _stateManager = value;
            }
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
            Initialize();

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
        /// Generates the state key name
        /// </summary>
        /// <returns></returns>
        private string GenerateStateKey(string keyPrefix)
        {
            return keyPrefix + RootFrame.BackStackDepth;
        }

        /// <summary>
        /// Navigate to a specific page, passing parameters
        /// </summary>
        /// <param name="pageType">Page Type</param>
        /// <param name="argument">Argument to pass (primitive type, may be null)</param>
        /// <param name="viewModel">ViewModel to assign (may be null)</param>
        public bool NavigateTo(Type pageType, object argument, object viewModel)
        {
            Initialize();

            if (pageType == null)
                throw new ArgumentNullException("pageType");

            try
            {
                _watchForViewModel = viewModel;
                return RootFrame.Navigate(pageType, argument);
            }
            finally
            {
                _watchForViewModel = null;
            }
        }

        /// <summary>
        /// Go backward in the navigation chain
        /// </summary>
        public bool GoBack()
        {
            Initialize();
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
            Initialize();
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
        /// Clear the back history
        /// </summary>
        private void ClearForwardHistory()
        {
            if (StateManager == null)
                return;

            // Remove ViewModel keys
            var nextPageKey = GenerateStateKey(ViewModelKeyPrefix);
            int nextPageIndex = this.BackStackDepth;
            while (StateManager.RemoveDictionary(nextPageKey))
            {
                nextPageIndex++;
                nextPageKey = ViewModelKeyPrefix + nextPageIndex;
            }

            // Remove Page keys
            nextPageKey = GenerateStateKey(PageKeyPrefix);
            nextPageIndex = this.BackStackDepth;
            while (StateManager.RemoveDictionary(nextPageKey))
            {
                nextPageIndex++;
                nextPageKey = PageKeyPrefix + nextPageIndex;
            }
        }

        /// <summary>
        /// This is used to process a suspension
        /// </summary>
        private void ProcessSuspend()
        {
            bool cancel = false;
            HandleOnNavigatingFrom(RootFrame, NavigationMode.Refresh, ref cancel, true);
        }

        /// <summary>
        /// Restore the current VM state after suspension
        /// </summary>
        private void ProcessRestore()
        {
            var currentView = RootFrame.Content as FrameworkElement;
            if (currentView != null)
            {
                HandleOnNavigatingTo(currentView, currentView.DataContext, NavigationMode.Refresh, null);
            }
        }

        /// <summary>
        /// Used to save the navigation stack and should be called in the suspending event.
        /// </summary>
        public async Task SaveAsync()
        {
            if (StateManager != null)
            {
                ProcessSuspend();

                var frameDictionary = StateManager.GetDictionary("vmFrame", true);
                frameDictionary["Navigation"] = RootFrame.GetNavigationState();

                await StateManager.SaveAsync();
            }
        }

        /// <summary>
        /// Restore the navigation stack
        /// </summary>
        public async Task<bool> LoadAsync()
        {
            if (RootFrame == null)
            {
                throw new InvalidOperationException("Must set root Frame prior to calling PageNavigator.LoadAsync");
            }

            if (StateManager != null)
            {
                bool loaded = await StateManager.LoadAsync();
                if (loaded)
                {
                    var frameDictionary = StateManager.GetDictionary("vmFrame", false);
                    if (frameDictionary != null)
                    {
                        if (frameDictionary.ContainsKey("Navigation"))
                        {
                            RootFrame.SetNavigationState((string) frameDictionary["Navigation"]);
                            ProcessRestore();
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <value>
        /// The back stack depth.
        /// </value>
        public int BackStackDepth { get { return RootFrame.BackStackDepth; } }

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
