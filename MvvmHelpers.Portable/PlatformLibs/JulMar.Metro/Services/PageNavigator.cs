using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using JulMar.Core;
using JulMar.Interfaces;
using JulMar.Services;

[assembly: ExportService(typeof(IPageNavigator), typeof(PageNavigator), IsFallback = true)]

namespace JulMar.Services
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
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class ExportPageAttribute : Attribute
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
        {
            this.Key = key;
            this.Type = type;
        }
    }

    /// <summary>
    /// Page navigation service
    /// </summary>
    public sealed class PageNavigator : IPageNavigator
    {
        internal const string PageKeyPrefix = "page-";
        internal const string DefaultFrameKey = "vmFrameKey";
        internal const string NavigationStackKey = "NavigationStack";
        internal const string ViewModelKeyPrefix = "pagevm-";

        private readonly IDictionary<string, Type> _registeredPages = new ConcurrentDictionary<string, Type>();
        private bool _initialized;
        private object _watchForViewModel;
        private Frame _frame;
        private IStateManager _stateManager;
        private string _frameKey;

        /// <summary>
        /// The root Frame 
        /// </summary>
        private Frame NavigationFrame
        {
            get { return this._frame ?? Window.Current.Content as Frame; }
        }

        private void Initialize()
        {
            if (this._initialized || this.NavigationFrame == null)
                return;

            this._initialized = true;

            // Register all the pages
            var assemblies = PlatformServices.GetAssemblies();
            foreach (var asm in assemblies)
            {
                try
                {
                    // Look for assembly level attributes.
                    var attributes = asm.GetCustomAttributes<ExportPageAttribute>().ToList();
                    foreach (var att in attributes)
                    {
                        this.RegisterPage(att.Key, att.Type);
                    }
                }
                catch
                {
                    // Skip.
                }
            }


            this.NavigationFrame.Navigating += this.RootFrameOnNavigating;
            this.NavigationFrame.Navigated += this.RootFrameOnNavigated;
        }

        /// <summary>
        /// This handles the Navigated TO event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootFrameOnNavigated(object sender, NavigationEventArgs e)
        {
            FrameworkElement fe = e.Content as FrameworkElement;
            object viewModel = this._watchForViewModel;
            this._watchForViewModel = null;

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
                this.ClearForwardHistory();
            }

            this.HandleOnNavigatingTo(fe, viewModel, e.NavigationMode, e.Parameter);
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
                if (this.StateManager != null)
                {
                    string key = this.GenerateStateKey(ViewModelKeyPrefix);
                    this.StateManager.LoadObject(key, viewModel);
                    stateDictionary = this.StateManager.GetDictionary(key, false);
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
                if (this.StateManager != null)
                {
                    string key = this.GenerateStateKey(PageKeyPrefix);
                    stateDictionary = this.StateManager.GetDictionary(key, false);
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
            this.HandleOnNavigatingFrom(frame, e.NavigationMode, ref cancel, false);
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
                        if (this.StateManager != null)
                        {
                            string key = this.GenerateStateKey(ViewModelKeyPrefix);
                            this.StateManager.SaveObject(key, fe.DataContext);
                            stateDictionary = this.StateManager.GetDictionary(key, true);
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
                        if (this.StateManager != null)
                        {
                            string key = this.GenerateStateKey(PageKeyPrefix);
                            stateDictionary = this.StateManager.GetDictionary(key, false);
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
            get { return this._stateManager ?? (this._stateManager = ServiceLocater.Instance.Resolve<IStateManager>()); }
            set
            {
                if (this._stateManager != null)
                    throw new InvalidOperationException("Cannot assign StateManager once it has been used.");

                this._stateManager = value;
            }
        }

        /// <summary>
        /// Returns the current page key (if known)
        /// </summary>
        public string CurrentPageKey
        {
            get
            {
                Type page = this.NavigationFrame.SourcePageType;
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
            this.Initialize();

            if (!this._registeredPages.ContainsKey(key))
                this._registeredPages.Add(key, type);
            else
            {
                this._registeredPages.Remove(key);
                this._registeredPages.Add(key, type);
            }
        }

        /// <summary>
        /// Unregister a page.
        /// </summary>
        /// <param name="key">Page key to remove</param>
        public bool UnregisterPage(string key)
        {
            return this._registeredPages.Remove(key);
        }

        /// <summary>
        /// Navigate to a specific page
        /// </summary>
        /// <param name="pageKey">Page key</param>
        public bool NavigateTo(string pageKey)
        {
            return this.NavigateTo(pageKey, null);
        }

        /// <summary>
        /// Navigate to a specific page, passing parameters
        /// </summary>
        /// <param name="pageKey">Page key</param>
        /// <param name="argument">Argument to pass (primitive type, may be null)</param>
        public bool NavigateTo(string pageKey, object argument)
        {
            return this.NavigateTo(pageKey, argument, null);
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

            Type entry;
            return this._registeredPages.TryGetValue(pageKey, out entry) 
                && this.NavigateTo(entry, argument, viewModel);
        }

        /// <summary>
        /// Navigate to a specific page
        /// </summary>
        /// <param name="pageType">Page Type</param>
        public bool NavigateTo(Type pageType)
        {
            return this.NavigateTo(pageType, null, null);
        }

        /// <summary>
        /// Navigate to a specific page
        /// </summary>
        /// <param name="pageType">Page Type</param>
        /// <param name="argument">Argument to pass (primitive type, may be null)</param>
        public bool NavigateTo(Type pageType, object argument)
        {
            return this.NavigateTo(pageType, argument, null);
        }

        /// <summary>
        /// Generates the state key name
        /// </summary>
        /// <returns></returns>
        private string GenerateStateKey(string keyPrefix)
        {
            return keyPrefix + this.NavigationFrame.BackStackDepth;
        }

        /// <summary>
        /// Navigate to a specific page, passing parameters
        /// </summary>
        /// <param name="pageType">Page Type</param>
        /// <param name="argument">Argument to pass (primitive type, may be null)</param>
        /// <param name="viewModel">ViewModel to assign (may be null)</param>
        public bool NavigateTo(Type pageType, object argument, object viewModel)
        {
            this.Initialize();

            if (pageType == null)
                throw new ArgumentNullException("pageType");

            try
            {
                this._watchForViewModel = viewModel;
                return this.NavigationFrame.Navigate(pageType, argument);
            }
            finally
            {
                this._watchForViewModel = null;
            }
        }

        /// <summary>
        /// Go backward in the navigation chain
        /// </summary>
        public bool GoBack()
        {
            this.Initialize();
            if (!this.CanGoBack)
                return false;
            this.NavigationFrame.GoBack();
            return true;
        }

        /// <summary>
        /// Return whether there is a page behind the current page in the navigation service
        /// </summary>
        public bool CanGoBack
        {
            get { return this.NavigationFrame.CanGoBack; }
        }

        /// <summary>
        /// Go forward in the navigation chain
        /// </summary>
        public bool GoForward()
        {
            this.Initialize();
            if (!this.CanGoForward)
                return false;
            this.NavigationFrame.GoForward();
            return true;
        }

        /// <summary>
        /// Return whether there is a page ahead the current page in the navigation service
        /// </summary>
        public bool CanGoForward { get { return this.NavigationFrame.CanGoForward; } }

        /// <summary>
        /// Clear the back history
        /// </summary>
        private void ClearForwardHistory()
        {
            if (this.StateManager == null)
                return;

            // Remove ViewModel keys
            var nextPageKey = this.GenerateStateKey(ViewModelKeyPrefix);
            int nextPageIndex = this.BackStackDepth;
            while (this.StateManager.RemoveDictionary(nextPageKey))
            {
                nextPageIndex++;
                nextPageKey = ViewModelKeyPrefix + nextPageIndex;
            }

            // Remove Page keys
            nextPageKey = this.GenerateStateKey(PageKeyPrefix);
            nextPageIndex = this.BackStackDepth;
            while (this.StateManager.RemoveDictionary(nextPageKey))
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
            this.HandleOnNavigatingFrom(this.NavigationFrame, NavigationMode.Refresh, ref cancel, true);
        }

        /// <summary>
        /// Restore the current VM state after suspension
        /// </summary>
        private void ProcessRestore()
        {
            var currentView = this.NavigationFrame.Content as FrameworkElement;
            if (currentView != null)
            {
                this.HandleOnNavigatingTo(currentView, currentView.DataContext, NavigationMode.Refresh, null);
            }
        }

        /// <summary>
        /// Used to save the navigation stack and should be called in the suspending event.
        /// </summary>
        public async Task SaveAsync()
        {
            if (this.StateManager != null)
            {
                this.ProcessSuspend();

                string frameKey = string.IsNullOrEmpty(this._frameKey) ? DefaultFrameKey : this._frameKey;
                var frameDictionary = this.StateManager.GetDictionary(frameKey, true);
                frameDictionary[NavigationStackKey] = this.NavigationFrame.GetNavigationState();

                await this.StateManager.SaveAsync();
            }
        }

        /// <summary>
        /// Restore the navigation stack
        /// </summary>
        public async Task<bool> LoadAsync()
        {
            if (this.NavigationFrame == null)
            {
                throw new InvalidOperationException("Must set root Frame prior to calling PageNavigator.LoadAsync");
            }

            if (this.StateManager != null)
            {
                bool loaded = await this.StateManager.LoadAsync();
                if (loaded)
                {
                    string frameKey = string.IsNullOrEmpty(this._frameKey) ? DefaultFrameKey : this._frameKey;

                    var frameDictionary = this.StateManager.GetDictionary(frameKey, false);
                    if (frameDictionary != null)
                    {
                        if (frameDictionary.ContainsKey(NavigationStackKey))
                        {
                            this.NavigationFrame.SetNavigationState((string)frameDictionary[NavigationStackKey]);
                            this.ProcessRestore();
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// This method can be used to change the frame which is being used for navigation
        /// for this specific navigator.
        /// </summary>
        /// <param name="frame">Frame to use</param>
        /// <param name="frameKey">Suspension key</param>
        public void SetNavigationTarget(Frame frame, string frameKey = "")
        {
            if (this._initialized)
                throw new InvalidOperationException("Must call PageNavigator.SetNavigationTarget prior to using any page navigation services (Frame already set).");

            this._frame = frame;
            this._frameKey = frameKey;

            this.Initialize();
        }

        /// <value>
        /// The back stack depth.
        /// </value>
        public int BackStackDepth { get { return this.NavigationFrame.BackStackDepth; } }
    }
}
