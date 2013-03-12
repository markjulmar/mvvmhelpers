using System;
using Windows.Foundation;
using Windows.UI.ApplicationSettings;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;

namespace JulMar.Windows.Controls
{
    /// <summary>
    /// A single FlyoutPage class which supports creating "flyout" panels with an animation.
    /// To use this, derive a new Page class and fill with details.
    /// </summary>
    public class FlyoutPage : Page
    {
        private const int MinFlyoutWidth = 345;
        private Popup _popupHost;

        /// <summary>
        /// Event raised when the flyout is opened
        /// </summary>
        public event EventHandler FlyoutOpened;

        /// <summary>
        /// Event raised when the flyout is closed
        /// </summary>
        public event EventHandler FlyoutClosed;

        /// <summary>
        /// Method used to show the flyout
        /// </summary>
        public void Show()
        {
            // Set a minimum width
            if (double.IsNaN(Width) || Width <= 1)
            {
                if (Content != null)
                {
                    FrameworkElement fe = Content as FrameworkElement;
                    if (fe != null)
                    {
                        fe.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                        Width = fe.DesiredSize.Width;
                    }
                }
                if (double.IsNaN(Width) || Width <= 1)
                    Width = MinFlyoutWidth;
            }

            // Wake up the entire height of the window.
            Height = Window.Current.Bounds.Height;

            // Create the popup host
            _popupHost = new Popup {IsLightDismissEnabled = true, Child = this};
            _popupHost.Opened += OnPopupOpened;
            _popupHost.Closed += OnPopupClosed;

            // Position it on the proper side.
            if (SettingsPane.Edge == SettingsEdgeLocation.Right)
                Canvas.SetLeft(_popupHost, Window.Current.Bounds.Width - Width);

            // Watch for deactivation
            Window.Current.Activated += OnWindowActivated;

            // Add animations for the panel.
            _popupHost.ChildTransitions = new TransitionCollection
            {
                new PaneThemeTransition
                    {
                        Edge = (SettingsPane.Edge == SettingsEdgeLocation.Right) 
                                ? EdgeTransitionLocation.Right
                                : EdgeTransitionLocation.Left
                    }
            };

            // Display the popup host
            _popupHost.IsOpen = true;
        }

        /// <summary>
        /// Method to close the flyout
        /// </summary>
        public void Close()
        {
            if (_popupHost != null)
                _popupHost.IsOpen = false;
        }

        /// <summary>
        /// This is called when the popup is opened.  We raise an event
        /// for external processes to catch.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPopupOpened(object sender, object e)
        {
            if (FlyoutOpened != null)
            {
                FlyoutOpened(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Monitor the Window.Activated event to close the popup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowActivated(object sender, WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == CoreWindowActivationState.Deactivated)
            {
                Close();
            }
        }

        /// <summary>
        /// Called when the popup is closed. Perform all cleanup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPopupClosed(object sender, object e)
        {
            if (FlyoutClosed != null)
            {
                FlyoutClosed(this, EventArgs.Empty);
            }

            _popupHost.Child = null;
            _popupHost = null;

            Window.Current.Activated -= OnWindowActivated;
        }
    }
}
