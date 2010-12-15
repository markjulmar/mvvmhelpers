using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using JulMar.Core;
using JulMar.Core.Interfaces;
using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;

namespace JulMar.Windows.UI
{
    /// <summary>
    /// Interface used to populate metadata we use for services.
    /// </summary>
    public interface IUIVisualizerMetadata
    {
        /// <summary>
        /// Key used to export the UI - registered with the UIVisualizer.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// The type being exported
        /// </summary>
        string ExportTypeIdentity { get; }
    }

    /// <summary>
    /// This attribute is used to decorate all "auto-located" services.
    /// MEF is used to locate and bind each service with this attribute decoration.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ExportUIVisualizerAttribute : ExportAttribute
    {
        /// <summary>
        /// Key used to export the View/ViewModel
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ExportUIVisualizerAttribute(string key)
            : base(UIVisualizer.MefLocatorKey)
        {
            Key = key;
        }
    }


    /// <summary>
    /// This class implements the IUIVisualizer for WPF.
    /// </summary>
    [ExportService(typeof(IUIVisualizer))]
    sealed class UIVisualizer : IUIVisualizer
    {
        /// <summary>
        /// Key used to lookup visualizations with MEF.
        /// </summary>
        internal const string MefLocatorKey = "JulMar.UIVisualizer.Export";

        /// <summary>
        /// Registered UI windows
        /// </summary>
        private readonly Dictionary<string, Type> _registeredWindows;

        #pragma warning disable 649
        /// <summary>
        /// MEF registered views
        /// </summary>
        [ImportMany(MefLocatorKey, AllowRecomposition = true)]
        private IEnumerable<Lazy<object, IUIVisualizerMetadata>> _locatedVisuals;
        #pragma warning restore 649

        /// <summary>
        /// Set to true once we have loaded any dynamic visuals.
        /// </summary>
        private bool _haveLoadedVisuals;

        /// <summary>
        /// Constructor
        /// </summary>
        public UIVisualizer()
        {
            _registeredWindows = new Dictionary<string, Type>();
        }

        /// <summary>
        /// Locates a type in a loaded assembly.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        private static Type FindType(string typeName)
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(asm => asm.GetType(typeName, false))
                .FirstOrDefault(type => type != null);
        }

        /// <summary>
        /// Registers a collection of entries
        /// </summary>
        /// <param name="startupData"></param>
        public void Register(Dictionary<string, Type> startupData)
        {
            foreach (var entry in startupData)
                Register(entry.Key, entry.Value);
        }

        /// <summary>
        /// Registers a type through a key.
        /// </summary>
        /// <param name="key">Key for the UI dialog</param>
        /// <param name="winType">Type which implements dialog</param>
        public void Register(string key, Type winType)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");
            if (winType == null)
                throw new ArgumentNullException("winType");
            if (!typeof(Window).IsAssignableFrom(winType))
                throw new ArgumentException("winType must be of type Window");

            lock(_registeredWindows)
            {
                _registeredWindows.Add(key, winType);
            }
        }

        /// <summary>
        /// This unregisters a type and removes it from the mapping
        /// </summary>
        /// <param name="key">Key to remove</param>
        /// <returns>True/False success</returns>
        public bool Unregister(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            lock (_registeredWindows)
            {
                return _registeredWindows.Remove(key);
            }
        }

        /// <summary>
        /// This method displays a modaless dialog associated with the given key.  The associated
        /// VM is not connected but must be supplied through some other means.
        /// </summary>
        /// <param name="key">Key previously registered with the UI controller.</param>
        /// <param name="setOwner">Set the owner of the window</param>
        /// <param name="completedProc">Callback used when UI closes (may be null)</param>
        /// <returns>True/False if UI is displayed</returns>
        public bool Show(string key, bool setOwner, EventHandler<UICompletedEventArgs> completedProc)
        {
            return Show(key, null, setOwner, completedProc);
        }

        /// <summary>
        /// This method displays a modal dialog associated with the given key.  The associated
        /// VM is not connected but must be supplied through some other means.
        /// </summary>
        /// <param name="key">Key previously registered with the UI controller.</param>
        /// <returns>True/False if UI is displayed.</returns>
        public bool? ShowDialog(string key)
        {
            return ShowDialog(key, null);
        }

        /// <summary>
        /// This method displays a modaless dialog associated with the given key.
        /// </summary>
        /// <param name="key">Key previously registered with the UI controller.</param>
        /// <param name="state">Object state to associate with the dialog</param>
        /// <param name="setOwner">Set the owner of the window</param>
        /// <param name="completedProc">Callback used when UI closes (may be null)</param>
        /// <returns>True/False if UI is displayed</returns>
        public bool Show(string key, object state, bool setOwner, EventHandler<UICompletedEventArgs> completedProc)
        {
            Window win = CreateWindow(key, state, setOwner, completedProc, false);
            if (win != null)
            {
                win.Show();
                return true;
            }
            return false;
        }

        /// <summary>
        /// This method displays a modal dialog associated with the given key.
        /// </summary>
        /// <param name="key">Key previously registered with the UI controller.</param>
        /// <param name="state">Object state to associate with the dialog</param>
        /// <returns>True/False if UI is displayed.</returns>
        public bool? ShowDialog(string key, object state)
        {
            Window win = CreateWindow(key, state, false, null, true);
            if (win != null)
                return win.ShowDialog();
            
            return false;
        }

        /// <summary>
        /// This creates the WPF window from a key.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="dataContext">DataContext (state) object</param>
        /// <param name="setOwner">True/False to set ownership to MainWindow</param>
        /// <param name="completedProc">Callback</param>
        /// <param name="isModal">True if this is a ShowDialog request</param>
        /// <returns>Success code</returns>
        private Window CreateWindow(string key, object dataContext, bool setOwner, EventHandler<UICompletedEventArgs> completedProc, bool isModal)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            // If we've not scanned for available exported views, do so now.
            if (!_haveLoadedVisuals)
                CheckForDynamicRegisters();

            Type winType;
            lock (_registeredWindows)
            {
                if (!_registeredWindows.TryGetValue(key, out winType))
                    return null;
            }

            // Create the top level window
            var win = (Window) Activator.CreateInstance(winType);
            if (setOwner)
                win.Owner = Application.Current.MainWindow;

            // Register the view with MEF to resolve any imports.
            var dynamicLoader = ViewModel.ServiceProvider.Resolve<IDynamicResolver>();
            if (dynamicLoader != null)
            {
                try
                {
                    dynamicLoader.ComposeOnce(win);
                }
                catch (CompositionException)
                {
                }
            }

            if (dataContext != null)
            {
                win.DataContext = dataContext;
                var bvm = dataContext as ViewModel;

                // Wire up the event handlers.  Go through the dispatcher in case the window
                // is being created on a secondary thread so the primary thread can invoke the
                // event handlers.
                if (bvm != null)
                {
                    if (isModal)
                    {
                        bvm.CloseRequest += ((s, e) => win.Dispatcher.Invoke((Action)(() =>
                              {
                                  try
                                  {
                                      win.DialogResult = e.Result;
                                  }
                                  catch (InvalidOperationException)
                                  {
                                      win.Close();
                                  }
                              }), null));
                    }
                    else
                    {
                        bvm.CloseRequest += ((s, e) => win.Dispatcher.Invoke((Action)(win.Close), null));
                    }

                    bvm.ActivateRequest += ((s, e) => win.Dispatcher.Invoke((Action)(() => win.Activate()), null));   
                }
            }

            if (completedProc != null)
            {
                win.Closed +=
                    (s, e) =>
                    completedProc(this,
                                  new UICompletedEventArgs { State = dataContext, Result = (isModal) ? win.DialogResult : null });

            }

            return win;
        }

        /// <summary>
        /// Initialize any MEF-located views.
        /// </summary>
        private void CheckForDynamicRegisters()
        {
            if (!_haveLoadedVisuals)
            {
                // If we have visuals, register them
                foreach (var item in _locatedVisuals)
                {
                    Type type = FindType(item.Metadata.ExportTypeIdentity);
                    if (type != null)
                        Register(item.Metadata.Key, type);
                }

                // Clear the collection so we don't process twice.
                _haveLoadedVisuals = true;
            }
        }
    }
}