using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using JulMar.Interfaces;
using JulMar.Services;

namespace JulMar.UI
{
    /// <summary>
    /// This class implements the IUIVisualizer for WPF.
    /// </summary>
    sealed class UIVisualizer : IUIVisualizer
    {
        /// <summary>
        /// Registered UI windows
        /// </summary>
        private readonly Dictionary<string, Type> _registeredWindows = new Dictionary<string, Type>();

        /// <summary>
        /// Set to true once we have loaded any dynamic visuals.
        /// </summary>
        private bool _haveLoadedVisuals;

        /// <summary>
        /// Registers a type through a key.
        /// </summary>
        /// <param name="key">Key for the UI dialog</param>
        /// <param name="winType">Type which implements dialog</param>
        public void Add(string key, Type winType)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");
            if (winType == null)
                throw new ArgumentNullException("winType");
            if (!typeof(Window).IsAssignableFrom(winType))
                throw new ArgumentException("winType must be of type Window");

            if (!this._registeredWindows.ContainsKey(key))
                this._registeredWindows.Add(key, winType);
        }

        /// <summary>
        /// This unregisters a type and removes it from the mapping
        /// </summary>
        /// <param name="key">Key to remove</param>
        /// <returns>True/False success</returns>
        public bool Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");
            return this._registeredWindows.Remove(key);
        }

        /// <summary>
        /// This method displays a modeless dialog associated with the given key.  The associated
        /// VM is not connected but must be supplied through some other means.
        /// </summary>
        /// <param name="key">Key previously registered with the UI controller.</param>
        /// <param name="setOwner">Set the owner of the window</param>
        /// <returns>True/False if UI is displayed</returns>
        public Task ShowAsync(string key, bool setOwner)
        {
            return ShowAsync(key, null, setOwner);
        }

        /// <summary>
        /// This method displays a modal dialog associated with the given key.
        /// </summary>
        /// <param name="key">Key previously registered with the UI controller.</param>
        /// <returns>True/False if UI is displayed.</returns>
        public Task<bool?> ShowDialogAsync(string key)
        {
            return ShowDialogAsync(key, null);
        }

        /// <summary>
        /// This method displays a modeless dialog associated with the given key.
        /// </summary>
        /// <param name="key">Key previously registered with the UI controller.</param>
        /// <param name="state">Object state to associate with the dialog</param>
        /// <param name="setOwner">Set the owner of the window</param>
        /// <returns>True/False if UI is displayed</returns>
        public Task ShowAsync(string key, object state, bool setOwner)
        {
            Window win = this.CreateWindow(key, state, 
                (setOwner) ? Application.Current.MainWindow : null);

            if (win != null)
            {
                var cts = new CancellationTokenSource();
                win.Closed += (s, e) => cts.Cancel();

                return Task.Run(async () =>
                    {
                            Application.Current.Dispatcher.BeginInvoke((Action)(win.Show)).GetAwaiter();
                            await Task.Delay(TimeSpan.MaxValue, cts.Token);
                        }, cts.Token);
            }

            return null;
        }

        /// <summary>
        /// This method displays a modal dialog associated with the given key.
        /// </summary>
        /// <param name="key">Key previously registered with the UI controller.</param>
        /// <param name="state">Object state to associate with the dialog</param>
        /// <returns>True/False if UI is displayed.</returns>
        public async Task<bool?> ShowDialogAsync(string key, object state)
        {
            Window win = this.CreateWindow(key, state, Application.Current.MainWindow);

            if (win != null)
            {
                return await Task.Run(() => 
                    Application.Current.Dispatcher.Invoke((Func<bool?>)(win.ShowDialog)));
            }

            return null;
        }

        /// <summary>
        /// This creates the WPF window from a key.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="dataContext">DataContext (state) object</param>
        /// <param name="owner">Owner for the window</param>
        /// <returns>Success code</returns>
        private Window CreateWindow(string key, object dataContext, Window owner)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            // If we've not scanned for available exported views, do so now.
            if (!this._haveLoadedVisuals)
                this.Initialize();

            Type winType;
            if (!this._registeredWindows.TryGetValue(key, out winType))
                return null;

            // Create the top level window
            var win = (Window) Activator.CreateInstance(winType);
            if (owner != null)
                win.Owner = owner;

            if (dataContext != null)
                win.DataContext = dataContext;

            return win;
        }

        /// <summary>
        /// Initialize registered views.
        /// </summary>
        private void Initialize()
        {
            if (!this._haveLoadedVisuals)
            {
                var assemblies = PlatformServices.GetAssemblies();
                foreach (var asm in assemblies)
                {
                    foreach (ExportUIVisualizerAttribute attr in asm.GetCustomAttributes(typeof(ExportUIVisualizerAttribute), true))
                    {
                        this.Add(attr.Key, attr.Type);
                    }
                }

                this._haveLoadedVisuals = true;
            }
        }
    }
}