using System;
using System.Globalization;
using Windows.ApplicationModel;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Microsoft.Xaml.Interactivity;

namespace BouncingBalls
{
    /// <summary>
    /// This class displays a frame counter at the top left corner of the window.
    /// </summary>
    public class FrameRateCounterBehavior : DependencyObject, IBehavior
    {
        private DateTime _lastSnap;
        private TextBlock _associatedObject;
        private int _frameCount;
        private bool _eventsHooked;

        /// <summary>
        /// Backing storage for IsEnabled
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(FrameRateCounterBehavior), new PropertyMetadata(true, OnIsEnabledChanged));

        /// <summary>
        /// Turns the frame counter on and off
        /// </summary>
        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        /// <summary>
        /// Property change notification method for the IsEnabled boolean state.
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="e"></param>
        private static void OnIsEnabledChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            FrameRateCounterBehavior fc = (FrameRateCounterBehavior)dpo;

            if ((bool)e.NewValue == true)
            {
                fc.Enable();
            }
            else if ((bool)e.NewValue == false)
            {
                fc.Disable();
            }
        }

        /// <summary>
        /// Backing storage for the DisableWhenNotActive property
        /// </summary>
        public static readonly DependencyProperty DisableWhenNotActiveProperty =
            DependencyProperty.Register("DisableWhenNotActive", typeof(bool), typeof(FrameRateCounterBehavior), new PropertyMetadata(default(bool)));

        /// <summary>
        /// Turns the timer on and off based on window activation
        /// </summary>
        public bool DisableWhenNotActive
        {
            get { return (bool)GetValue(DisableWhenNotActiveProperty); }
            set { SetValue(DisableWhenNotActiveProperty, value); }
        }

        /// <summary>
        /// This is called on each frame render
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void CompositionTargetOnRendering(object sender, object eventArgs)
        {
            _frameCount++;
            DateTime now = DateTime.Now;
            TimeSpan diff = (now - _lastSnap);
            if (diff.TotalSeconds >= 1)
            {
                _associatedObject.Text = (_frameCount / diff.TotalSeconds).ToString(CultureInfo.InvariantCulture) + " fps";
                _lastSnap = now;
                _frameCount = 0;
            }
        }

        /// <summary>
        /// Disables the frame counter timer
        /// </summary>
        private void Disable()
        {
            CompositionTarget.Rendering -= CompositionTargetOnRendering;
            _associatedObject.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// This is called when the window is activated and has focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnWindowActivationChanged(object sender, WindowActivatedEventArgs eventArgs)
        {
            if (eventArgs.WindowActivationState != CoreWindowActivationState.Deactivated)
            {
                _lastSnap = DateTime.Now;
                if (IsEnabled)
                {
                    if (DisableWhenNotActive || !_eventsHooked)
                    {
                        Enable();
                    }
                }
            }
            else
            {
                if (DisableWhenNotActive && IsEnabled)
                {
                    Disable();
                }
            }
        }

        /// <summary>
        /// Enable the frame rate counter
        /// </summary>
        private void Enable()
        {
            CompositionTarget.Rendering += CompositionTargetOnRendering;
            _eventsHooked = true;
            if (_associatedObject != null)
                _associatedObject.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Attaches to the specified object.
        /// </summary>
        /// <param name="associatedObject">The <see cref="T:Windows.UI.Xaml.DependencyObject"/> to which the <seealso cref="T:Microsoft.Xaml.Interactivity.IBehavior"/> will be attached.</param>
        public void Attach(DependencyObject associatedObject)
        {
            if ((associatedObject != AssociatedObject) && !DesignMode.DesignModeEnabled)
            {
                if (_associatedObject != null)
                    throw new InvalidOperationException("Cannot attach behavior multiple times.");

                _associatedObject = associatedObject as TextBlock;
                if (_associatedObject == null)
                    throw new InvalidOperationException("Can only attach FrameRateCounterBehavior to TextBlock control.");
            }

            Window.Current.Activated += OnWindowActivationChanged;
        }

        /// <summary>
        /// Detaches this instance from its associated object.
        /// </summary>
        public void Detach()
        {
            Window.Current.Activated -= OnWindowActivationChanged;
            CompositionTarget.Rendering -= CompositionTargetOnRendering;

        }

        /// <summary>
        /// Gets the <see cref="T:Windows.UI.Xaml.DependencyObject"/> to which the <seealso cref="T:Microsoft.Xaml.Interactivity.IBehavior"/> is attached.
        /// </summary>
        public DependencyObject AssociatedObject { get { return _associatedObject;  } }
    }
}
