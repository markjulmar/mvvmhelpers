using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using JulMar.Interfaces;
using JulMar.Serialization;

namespace JulMar.Services
{
    /// <summary>
    /// Page navigation service with auto-serialization support (JSon)
    /// </summary>
    /// <example>
    /// Replace default navigator in App.xaml.cs constructor:
    ///      ServiceLocator.Instance.Add(typeof(IPageNavigator), new AutoSerializingPageNavigator());
    /// </example>
    /// <remarks>
    /// Note that view model must all be serializable and use OnDeserialized initializers.
    /// </remarks>
    public sealed class AutoSerializingPageNavigator : IPageNavigator
    {
        private readonly IDictionary<string, Type> _registeredPages = new ConcurrentDictionary<string, Type>();
        private bool _initialized;
        private Frame _frame;
        private string _frameKey;
        private IStateManager _stateManager;

        /// <summary>
        /// The root Frame 
        /// </summary>
        private Frame NavigationFrame
        {
            get { return this._frame ?? Window.Current.Content as Frame; }
        }

        /// <summary>
        /// Initialize the navigator by hooking into the frame navigation events.
        /// </summary>
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
                FrameworkElement fe = rootFrame.Content as FrameworkElement;
                if (fe != null)
                {
                    // See if the ViewModel participates in navigation-aware services.
                    INavigationAware navm = fe.DataContext as INavigationAware;
                    if (navm != null)
                    {
                        IDictionary<string, object> stateDictionary = null;
                        if (this.StateManager != null)
                        {
                            string key = this.GenerateStateKey(PageNavigator.ViewModelKeyPrefix);
                            this.StateManager.SaveObject(key, fe.DataContext);
                            stateDictionary = this.StateManager.GetDictionary(key, true);
                        }


                        var e = (suspending)
                                    ? new NavigatingFromEventArgs(navMode, cancel, stateDictionary)
                                    : new NavigatingFromEventArgs(stateDictionary);
                        navm.OnNavigatingFrom(e);
                        cancel = e.Cancel;
                    }

                    INavigationAware naView = fe as INavigationAware;
                    if (naView != null)
                    {
                        IDictionary<string, object> stateDictionary = null;
                        if (this.StateManager != null)
                        {
                            string key = this.GenerateStateKey(PageNavigator.PageKeyPrefix);
                            stateDictionary = this.StateManager.GetDictionary(key, true);
                        }

                        // Save off the VIEW
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
        /// This handles the Navigated TO event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootFrameOnNavigated(object sender, NavigationEventArgs e)
        {
            object viewModel = this.InflateViewModel(e.Parameter as string);
            FrameworkElement fe = e.Content as FrameworkElement;
            if (fe != null && viewModel != null)
            {
                fe.DataContext = viewModel;
            }

            // Clear any forward navigation when adding a new page
            if (e.NavigationMode == NavigationMode.New)
            {
                this.ClearForwardHistory();
            }

            this.HandleOnNavigatingTo(fe, viewModel, e.NavigationMode, viewModel ?? e.Parameter);
        }

        /// <summary>
        /// Creates the ViewModel from a string.
        /// </summary>
        /// <param name="viewModelData"></param>
        /// <returns></returns>
        private object InflateViewModel(string viewModelData)
        {
            if (!string.IsNullOrEmpty(viewModelData) && viewModelData.StartsWith("$"))
            {
                int index = viewModelData.IndexOf('!');
                string type = viewModelData.Substring(1, index-1);
                int lindex = viewModelData.LastIndexOf('}');
                viewModelData = viewModelData.Substring(index+1, lindex-index);
                return Json.Deserialize(Type.GetType(type), viewModelData, this.StateManager.KnownTypes);
            }
            return null;
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
            // See if the ViewModel participates in navigation-aware services.
            INavigationAware navm = viewModel as INavigationAware;
            if (navm != null)
            {
                IDictionary<string, object> stateDictionary = null;
                if (this.StateManager != null)
                {
                    string key = this.GenerateStateKey(PageNavigator.ViewModelKeyPrefix);
                    this.StateManager.LoadObject(key, viewModel);
                    stateDictionary = this.StateManager.GetDictionary(key, false);
                }
                navm.OnNavigatedTo(new NavigatedToEventArgs(navMode, parameter, stateDictionary));
            }

            // See if the VIEW participates in navigation-aware services.
            INavigationAware naView = view as INavigationAware;
            if (naView != null)
            {
                IDictionary<string, object> stateDictionary = null;
                if (this.StateManager != null)
                {
                    string key = this.GenerateStateKey(PageNavigator.PageKeyPrefix);
                    stateDictionary = this.StateManager.GetDictionary(key, false);
                }
                naView.OnNavigatedTo(new NavigatedToEventArgs(navMode, parameter, stateDictionary));
            }
        }

        /// <summary>
        /// Clear the back history
        /// </summary>
        private void ClearForwardHistory()
        {
            if (this.StateManager == null)
                return;

            // Remove Page keys
            var nextPageKey = this.GenerateStateKey(PageNavigator.PageKeyPrefix);
            var nextPageIndex = this.BackStackDepth;
            while (this.StateManager.RemoveDictionary(nextPageKey))
            {
                nextPageIndex++;
                nextPageKey = PageNavigator.PageKeyPrefix + nextPageIndex;
            }

            // Remove ViewModel keys
            nextPageKey = this.GenerateStateKey(PageNavigator.ViewModelKeyPrefix);
            nextPageIndex = this.BackStackDepth;
            while (this.StateManager.RemoveDictionary(nextPageKey))
            {
                nextPageIndex++;
                nextPageKey = PageNavigator.ViewModelKeyPrefix + nextPageIndex;
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
        /// <param name="argument">Serializable view model, or argument to pass</param>
        public bool NavigateTo(string pageKey, object argument)
        {
            if (string.IsNullOrEmpty(pageKey))
                throw new ArgumentNullException("pageKey");

            Type entry;
            return this._registeredPages.TryGetValue(pageKey, out entry)
                && this.NavigateTo(entry, argument);
        }

        /// <summary>
        /// Navigate to a specific page, passing parameters, this method 
        /// is not supported by the auto serializing navigator.
        /// </summary>
        /// <param name="pageKey">Page key</param>
        /// <param name="argument">Argument to pass (primitive type, may be null)</param>
        /// <param name="viewModel">ViewModel to assign (may be null)</param>
        public bool NavigateTo(string pageKey, object argument, object viewModel)
        {
            throw new NotSupportedException("Direct view model object not supported - pass serializable ViewModel as argument instead.");
        }

        /// <summary>
        /// Navigate to a specific page
        /// </summary>
        /// <param name="pageType">Page Type</param>
        public bool NavigateTo(Type pageType)
        {
            return this.NavigateTo(pageType, null);
        }

        /// <summary>
        /// Navigate to a specific page
        /// </summary>
        /// <param name="pageType">Page Type</param>
        /// <param name="argument">Serializable view model, or argument to pass</param>
        public bool NavigateTo(Type pageType, object argument)
        {
            this.Initialize();

            if (pageType == null)
                throw new ArgumentNullException("pageType");

            string viewModelType = argument != null ? "$" + argument.GetType().AssemblyQualifiedName : null;
            if (!string.IsNullOrEmpty(viewModelType))
            {
                try
                {
                    argument = viewModelType + "!" + Json.Serialize(argument, this.StateManager.KnownTypes);
                }
                catch (Exception)
                {
                    // assume it's not serializable.
                }
            }

            return this.NavigationFrame.Navigate(pageType, argument);
        }

        /// <summary>
        /// Navigate to a specific page, passing parameters
        /// </summary>
        /// <param name="pageType">Page Type</param>
        /// <param name="argument">Argument to pass (primitive type, may be null)</param>
        /// <param name="viewModel">ViewModel to assign (may be null)</param>
        public bool NavigateTo(Type pageType, object argument, object viewModel)
        {
            throw new NotSupportedException("Direct view model object not supported - pass serializable ViewModel as argument instead.");
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
        /// Used to save the navigation stack and should be called in the suspending event.
        /// </summary>
        public async Task SaveAsync()
        {
            if (this.StateManager != null)
            {
                this.ProcessSuspend();

                string frameKey = string.IsNullOrEmpty(this._frameKey) ? PageNavigator.DefaultFrameKey : this._frameKey;
                var frameDictionary = this.StateManager.GetDictionary(frameKey, true);
                frameDictionary[PageNavigator.NavigationStackKey] = this.NavigationFrame.GetNavigationState();

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
                    string frameKey = string.IsNullOrEmpty(this._frameKey) ? PageNavigator.DefaultFrameKey : this._frameKey;

                    var frameDictionary = this.StateManager.GetDictionary(frameKey, false);
                    if (frameDictionary != null)
                    {
                        if (frameDictionary.ContainsKey(PageNavigator.NavigationStackKey))
                        {
                            // This will restore the parameter
                            string navData = (string) frameDictionary[PageNavigator.NavigationStackKey];
                            this.NavigationFrame.SetNavigationState(navData);
                            int index = navData.LastIndexOf('$');
                            if (index != -1)
                                this.ProcessRestore(navData.Substring(index));
                            return true;
                        }
                    }
                }
            }

            return false;
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
        private void ProcessRestore(string parameter)
        {
            var currentView = this.NavigationFrame.Content as FrameworkElement;
            if (currentView != null)
            {
                object viewModel = this.InflateViewModel(parameter);
                this.HandleOnNavigatingTo(currentView, viewModel, NavigationMode.Refresh, viewModel ?? parameter);
                currentView.DataContext = viewModel;
            }
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
