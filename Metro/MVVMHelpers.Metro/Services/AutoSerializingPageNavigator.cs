using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using JulMar.Core.Services;
using JulMar.Windows.Interfaces;
using JulMar.Windows.Serialization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.Concurrent;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;

namespace JulMar.Windows.Services
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
        private bool _autoLocatedPages, _initialized;
        private Frame _frame;
        private string _frameKey;
        private IStateManager _stateManager;

        /// <summary>
        /// The root Frame 
        /// </summary>
        private Frame NavigationFrame
        {
            get { return _frame ?? Window.Current.Content as Frame; }
        }

        /// <summary>
        /// Initialize the navigator by hooking into the frame navigation events.
        /// </summary>
        private void Initialize()
        {
            if (_initialized || NavigationFrame == null)
                return;

            _initialized = true;

            NavigationFrame.Navigating += RootFrameOnNavigating;
            NavigationFrame.Navigated += RootFrameOnNavigated;
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
                FrameworkElement fe = rootFrame.Content as FrameworkElement;
                if (fe != null)
                {
                    // See if the ViewModel participates in navigation-aware services.
                    INavigationAware navm = fe.DataContext as INavigationAware;
                    if (navm != null)
                    {
                        IDictionary<string, object> stateDictionary = null;
                        if (StateManager != null)
                        {
                            string key = GenerateStateKey(PageNavigator.ViewModelKeyPrefix);
                            StateManager.SaveObject(key, fe.DataContext);
                            stateDictionary = StateManager.GetDictionary(key, true);
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
                        if (StateManager != null)
                        {
                            string key = GenerateStateKey(PageNavigator.PageKeyPrefix);
                            stateDictionary = StateManager.GetDictionary(key, true);
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
            object viewModel = InflateViewModel(e.Parameter as string);
            FrameworkElement fe = e.Content as FrameworkElement;
            if (fe != null && viewModel != null)
            {
                fe.DataContext = viewModel;
            }

            // Clear any forward navigation when adding a new page
            if (e.NavigationMode == NavigationMode.New)
            {
                ClearForwardHistory();
            }

            HandleOnNavigatingTo(fe, viewModel, e.NavigationMode, viewModel ?? e.Parameter);
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
                string type = viewModelData.Substring(1, index);
                viewModelData = viewModelData.Substring(index + 1);
                return Json.Deserialize(Type.GetType(type), viewModelData, StateManager.KnownTypes);
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
                if (StateManager != null)
                {
                    string key = GenerateStateKey(PageNavigator.ViewModelKeyPrefix);
                    StateManager.LoadObject(key, viewModel);
                    stateDictionary = StateManager.GetDictionary(key, false);
                }
                navm.OnNavigatedTo(new NavigatedToEventArgs(navMode, parameter, stateDictionary));
            }

            // See if the VIEW participates in navigation-aware services.
            INavigationAware naView = view as INavigationAware;
            if (naView != null)
            {
                IDictionary<string, object> stateDictionary = null;
                if (StateManager != null)
                {
                    string key = GenerateStateKey(PageNavigator.PageKeyPrefix);
                    stateDictionary = StateManager.GetDictionary(key, false);
                }
                naView.OnNavigatedTo(new NavigatedToEventArgs(navMode, parameter, stateDictionary));
            }
        }

        /// <summary>
        /// Clear the back history
        /// </summary>
        private void ClearForwardHistory()
        {
            if (StateManager == null)
                return;

            // Remove Page keys
            var nextPageKey = GenerateStateKey(PageNavigator.PageKeyPrefix);
            var nextPageIndex = this.BackStackDepth;
            while (StateManager.RemoveDictionary(nextPageKey))
            {
                nextPageIndex++;
                nextPageKey = PageNavigator.PageKeyPrefix + nextPageIndex;
            }

            // Remove ViewModel keys
            nextPageKey = GenerateStateKey(PageNavigator.ViewModelKeyPrefix);
            nextPageIndex = this.BackStackDepth;
            while (StateManager.RemoveDictionary(nextPageKey))
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
                Type page = NavigationFrame.SourcePageType;
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
        /// <param name="argument">Serializable view model, or argument to pass</param>
        public bool NavigateTo(string pageKey, object argument)
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
                && NavigateTo(entry, argument);
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
            return NavigateTo(pageType, null);
        }

        /// <summary>
        /// Navigate to a specific page
        /// </summary>
        /// <param name="pageType">Page Type</param>
        /// <param name="argument">Serializable view model, or argument to pass</param>
        public bool NavigateTo(Type pageType, object argument)
        {
            Initialize();

            if (pageType == null)
                throw new ArgumentNullException("pageType");

            string viewModelType = argument != null ? "$" + argument.GetType().AssemblyQualifiedName : null;
            if (!string.IsNullOrEmpty(viewModelType))
            {
                try
                {
                    argument = viewModelType + "!" + Json.Serialize(argument, StateManager.KnownTypes);
                }
                catch (Exception)
                {
                    // assume it's not serializable.
                }
            }

            return NavigationFrame.Navigate(pageType, argument);
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
            return keyPrefix + NavigationFrame.BackStackDepth;
        }

        /// <summary>
        /// Go backward in the navigation chain
        /// </summary>
        public bool GoBack()
        {
            Initialize();
            if (!CanGoBack)
                return false;
            NavigationFrame.GoBack();
            return true;
        }

        /// <summary>
        /// Return whether there is a page behind the current page in the navigation service
        /// </summary>
        public bool CanGoBack
        {
            get { return NavigationFrame.CanGoBack; }
        }

        /// <summary>
        /// Go forward in the navigation chain
        /// </summary>
        public bool GoForward()
        {
            Initialize();
            if (!CanGoForward)
                return false;
            NavigationFrame.GoForward();
            return true;
        }

        /// <summary>
        /// Return whether there is a page ahead the current page in the navigation service
        /// </summary>
        public bool CanGoForward { get { return NavigationFrame.CanGoForward; } }

        /// <summary>
        /// Used to save the navigation stack and should be called in the suspending event.
        /// </summary>
        public async Task SaveAsync()
        {
            if (StateManager != null)
            {
                ProcessSuspend();

                string frameKey = string.IsNullOrEmpty(_frameKey) ? PageNavigator.DefaultFrameKey : _frameKey;
                var frameDictionary = StateManager.GetDictionary(frameKey, true);
                frameDictionary[PageNavigator.NavigationStackKey] = NavigationFrame.GetNavigationState();

                await StateManager.SaveAsync();
            }
        }

        /// <summary>
        /// Restore the navigation stack
        /// </summary>
        public async Task<bool> LoadAsync()
        {
            if (NavigationFrame == null)
            {
                throw new InvalidOperationException("Must set root Frame prior to calling PageNavigator.LoadAsync");
            }

            if (StateManager != null)
            {
                bool loaded = await StateManager.LoadAsync();
                if (loaded)
                {
                    string frameKey = string.IsNullOrEmpty(_frameKey) ? PageNavigator.DefaultFrameKey : _frameKey;

                    var frameDictionary = StateManager.GetDictionary(frameKey, false);
                    if (frameDictionary != null)
                    {
                        if (frameDictionary.ContainsKey(PageNavigator.NavigationStackKey))
                        {
                            // This will restore the parameter
                            string navData = (string) frameDictionary[PageNavigator.NavigationStackKey];
                            NavigationFrame.SetNavigationState(navData);
                            int index = navData.LastIndexOf('$');
                            if (index != -1)
                            ProcessRestore(navData.Substring(index));
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
            HandleOnNavigatingFrom(NavigationFrame, NavigationMode.Refresh, ref cancel, true);
        }

        /// <summary>
        /// Restore the current VM state after suspension
        /// </summary>
        private void ProcessRestore(string parameter)
        {
            var currentView = NavigationFrame.Content as FrameworkElement;
            if (currentView != null)
            {
                object viewModel = InflateViewModel(parameter);
                HandleOnNavigatingTo(currentView, viewModel, NavigationMode.Refresh, viewModel ?? parameter);
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
            if (_initialized)
                throw new InvalidOperationException("Must call PageNavigator.SetNavigationTarget prior to using any page navigation services (Frame already set).");

            _frame = frame;
            _frameKey = frameKey;

            Initialize();
        }

        /// <value>
        /// The back stack depth.
        /// </value>
        public int BackStackDepth { get { return NavigationFrame.BackStackDepth; } }

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
