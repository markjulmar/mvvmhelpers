using System.Collections.Concurrent;
using System.Composition;
using JulMar.Core.Internal;
using JulMar.Core.Services;
using JulMar.Windows.Controls;
using JulMar.Windows.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace JulMar.Windows.UI
{
    /// <summary>
    /// Class used to populate metadata used to identify flyouts.
    /// </summary>
    public sealed class FlyoutPageMetadata
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
    /// This is used to decorate a flyout page and associate it to a string key.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ExportFlyoutAttribute : ExportAttribute
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
        public ExportFlyoutAttribute(string key, Type type)
            : base(FlyoutVisualizer.MefLocatorKey, typeof(FlyoutPage))
        {
            Key = key;
            Type = type;
        }
    }

    /// <summary>
    /// Class used to locate views but keep property hidden
    /// </summary>
    internal sealed class FlyoutPageData
    {
        /// <summary>
        /// Located view models
        /// </summary>
        [ImportMany(FlyoutVisualizer.MefLocatorKey)]
        public IList<Lazy<FlyoutPage, FlyoutPageMetadata>> LocatedFlyouts { get; set; }
    }

    /// <summary>
    /// Page navigation service
    /// </summary>
    [DefaultExport(typeof(IFlyoutVisualizer))]
    internal sealed class FlyoutVisualizer : IFlyoutVisualizer
    {
        internal const string MefLocatorKey = "JulMar.FlyoutPage.Export";
        private readonly IDictionary<string, Type> _registeredFlyouts = new ConcurrentDictionary<string, Type>();
        private bool _autoLocatedFlyouts, _mustUnsnap;
        private readonly Dictionary<FlyoutPage, Tuple<Action,Action>> _callbacks = new Dictionary<FlyoutPage, Tuple<Action, Action>>();

        private string _key;
        private object _dataContext;
        private Action _opened;
        private Action _closed;

        /// <summary>
        /// Constructor
        /// </summary>
        public FlyoutVisualizer()
        {
            Window.Current.SizeChanged += WindowOnSizeChanged;
        }

        /// <summary>
        /// Used to activate a flyout when the original view was snapped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowOnSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            if (_mustUnsnap && ApplicationView.Value != ApplicationViewState.Snapped)
            {
                _mustUnsnap = false;

                Show(_key, _dataContext, _opened, _closed);

                _key = null;
                _dataContext = null;
                _opened = null;
                _closed = null;
            }
        }

        /// <summary>
        /// Show the flyout associated with the given key
        /// </summary>
        /// <param name="key"></param>
        public void Show(string key)
        {
            Show(key, null, null, null);
        }

        /// <summary>
        /// Show the flyout associated with the given key, using the passed DataContext
        /// with optional open/close actions.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataContext"></param>
        /// <param name="opened"></param>
        /// <param name="closed"></param>
        public void Show(string key, object dataContext, Action opened, Action closed)
        {
            // First, load up our flyouts.
            if (_autoLocatedFlyouts == false)
            {
                var locatedFlyouts = GatherFlyoutPages().ToList();
                if (locatedFlyouts != null && locatedFlyouts.Count > 0)
                {
                    foreach (var item in locatedFlyouts)
                        RegisterFlyout(item.Metadata.Key, item.Metadata.Type);
                }
                _autoLocatedFlyouts = true;
            }

            // Locate the flyout
            Type entry;
            if (_registeredFlyouts.TryGetValue(key, out entry))
            {
                if (ApplicationView.Value == ApplicationViewState.Snapped)
                {
                    _mustUnsnap = true;
                    _key = key;
                    _dataContext = dataContext;
                    _opened = opened;
                    _closed = closed;
                    _mustUnsnap = true;
    
                    if (!ApplicationView.TryUnsnap())
                    {
                        _mustUnsnap = false;

                        _key = null;
                        _dataContext = null;
                        _opened = null;
                        _closed = null;
                    }

                    return;
                }
                
                // Create a new page.
                FlyoutPage flyoutPage = Activator.CreateInstance(entry) as FlyoutPage;
                if (flyoutPage != null)
                {
                    if (dataContext != null)
                    {
                        flyoutPage.DataContext = dataContext;
                    }

                    if (opened != null || closed != null)
                    {
                        _callbacks.Add(flyoutPage, Tuple.Create(opened, closed));
                    }

                    flyoutPage.FlyoutOpened += FlyoutPageOnFlyoutOpened;
                    flyoutPage.FlyoutClosed += FlyoutPageOnFlyoutClosed;

                    // Display the flyout
                    flyoutPage.Show();
                }
            }
            else
            {
                throw new ArgumentException("No Flyout registered as " + key, "key");
            }
        }

        private void FlyoutPageOnFlyoutClosed(object sender, EventArgs eventArgs)
        {
            FlyoutPage flyoutPage = (FlyoutPage) sender;
            flyoutPage.FlyoutOpened -= FlyoutPageOnFlyoutOpened;
            flyoutPage.FlyoutClosed -= FlyoutPageOnFlyoutClosed;

            Tuple<Action, Action> callback;
            if (_callbacks.TryGetValue(flyoutPage, out callback))
            {
                if (callback.Item2 != null)
                {
                    callback.Item2();
                }
            }

            _callbacks.Remove(flyoutPage);
        }

        private void FlyoutPageOnFlyoutOpened(object sender, EventArgs eventArgs)
        {
            FlyoutPage flyoutPage = (FlyoutPage)sender;

            Tuple<Action, Action> callback;
            if (_callbacks.TryGetValue(flyoutPage, out callback))
            {
                if (callback.Item1 != null)
                {
                    callback.Item1();
                }
            }
        }

        /// <summary>
        /// Used to register a flyout with a key through code
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        public void RegisterFlyout(string key, Type type)
        {
            if (!_registeredFlyouts.ContainsKey(key))
                _registeredFlyouts.Add(key, type);
            else
            {
                _registeredFlyouts.Remove(key);
                _registeredFlyouts.Add(key, type);
            }
        }

        /// <summary>
        /// Unregister a flyout.
        /// </summary>
        /// <param name="key">Page key to remove</param>
        public bool UnregisterFlyout(string key)
        {
            return _registeredFlyouts.Remove(key);
        }

        /// <summary>
        /// This method uses an internal object to gather the list of flyouts.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Lazy<FlyoutPage, FlyoutPageMetadata>> GatherFlyoutPages()
        {
            var data = new FlyoutPageData();
            DynamicComposer.Instance.Compose(data);
            return data.LocatedFlyouts;
        }
    }
}
