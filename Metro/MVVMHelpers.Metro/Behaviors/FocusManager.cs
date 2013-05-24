using JulMar.Windows.Extensions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JulMar.Windows.Behaviors
{
    /// <summary>
    /// Focus Scope class which allows you to set focus initially to a Control
    /// </summary>
    public static class FocusScope
    {
        /// <summary>
        /// Focus property - set this on a UI element to change focus when loaded.
        /// </summary>
        public static readonly DependencyProperty InitialFocusProperty =
            DependencyProperty.RegisterAttached("InitialFocus", typeof (bool), typeof (FocusScope), new PropertyMetadata(default(bool), OnSetFocusChanged));

        /// <summary>
        /// Get the initial focus property from a Control
        /// </summary>
        /// <param name="theControl">Control</param>
        /// <returns>Current setting</returns>
        public static bool GetInitialFocus(Control theControl)
        {
            return (bool) theControl.GetValue(InitialFocusProperty);
        }

        /// <summary>
        /// Set the initial focus property onto a control.
        /// </summary>
        /// <param name="theControl">Control</param>
        /// <param name="value">New value</param>
        public static void SetInitialFocus(Control theControl, bool value)
        {
            theControl.SetValue(InitialFocusProperty, value);
        }

        /// <summary>
        /// Called when the focus state property changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnSetFocusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // Can only set focus to control elements.
            Control theControl = sender as Control;
            if (theControl == null)
                return;

            if ((bool) e.NewValue == true)
            {
                theControl.Loaded += TheControlOnLoaded;
                TrySetFocus(theControl);
            }
            else
            {
                theControl.Loaded -= TheControlOnLoaded;
            }
        }

        /// <summary>
        /// This attempts to programmatically set focus to the given control
        /// </summary>
        /// <param name="theControl">Control to set focus to</param>
        private static void TrySetFocus(Control theControl)
        {
            if (!Designer.InDesignMode)
            {
                theControl.Focus(FocusState.Programmatic);
            }
        }

        /// <summary>
        /// This is called when the associated control is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void TheControlOnLoaded(object sender, RoutedEventArgs e)
        {
            Control theControl = sender as Control;
            if (theControl != null)
                TrySetFocus(theControl);
        }
    }
}
