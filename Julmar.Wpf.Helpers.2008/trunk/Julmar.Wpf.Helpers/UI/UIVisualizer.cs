using System;
using System.Collections.Generic;
using System.Windows;
using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;
using System.Windows.Threading;

namespace JulMar.Windows.UI
{
    /// <summary>
    /// This class implements the IUIVisualizer for WPF.
    /// </summary>
    public class UIVisualizer : IUIVisualizer
    {
        private readonly Dictionary<string, Type> _registeredWindows;

        /// <summary>
        /// Constructor
        /// </summary>
        public UIVisualizer()
        {
            _registeredWindows = new Dictionary<string, Type>();
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
            bool setOwner = false;
            try
            {
                setOwner = (Application.Current != null &&
                              Application.Current.MainWindow != null &&
                              Dispatcher.CurrentDispatcher == Application.Current.MainWindow.Dispatcher);
            }
            catch(InvalidOperationException)
            {
                // Wrong thread.
            }

            Window win = CreateWindow(key, state, setOwner, null, true);
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

            Type winType;
            lock (_registeredWindows)
            {
                if (!_registeredWindows.TryGetValue(key, out winType))
                    return null;
            }

            var win = (Window) Activator.CreateInstance(winType);
            win.DataContext = dataContext;
            if (setOwner)
                win.Owner = Application.Current.MainWindow;

            if (dataContext != null)
            {
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
    }
}