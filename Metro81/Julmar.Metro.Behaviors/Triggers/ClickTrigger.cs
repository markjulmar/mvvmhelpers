using System.Windows.Interactivity;
using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace JulMar.Windows.Interactivity
{
    /// <summary>
    /// This is a Blend/VS.NET behavior which drives a Click event to 
    /// some interactive action
    /// </summary>
    public class ClickTrigger : TriggerBase<FrameworkElement>
    {
        private bool _wasOverTarget;

        /// <summary>
        /// The DependencyProperty for the ClickMode property.
        /// </summary>
        public static readonly DependencyProperty ClickModeProperty = DependencyProperty.Register("ClickMode", typeof(ClickMode), typeof(ClickTrigger), new PropertyMetadata(ClickMode.Release));

        /// <summary> 
        /// ClickMode specify when the Click event should fire 
        /// </summary>
        public ClickMode ClickMode
        {
            get { return (ClickMode)GetValue(ClickModeProperty);} 
            set { SetValue(ClickModeProperty, value); }
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PointerPressed += HandlePointerPressed;
            this.AssociatedObject.Holding += HandlePointerHolding;
            this.AssociatedObject.PointerReleased += HandlePointerReleased;
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>
        /// Override this to unhook functionality from the AssociatedObject.
        /// </remarks>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.PointerPressed -= HandlePointerPressed;
            this.AssociatedObject.Holding -= HandlePointerHolding;
            this.AssociatedObject.PointerReleased -= HandlePointerReleased;
        }

        /// <summary>
        /// Mark that a PointerPressed event occurred.
        /// </summary>
        /// <param name="sender">UIElement</param>
        /// <param name="e">EventArgs</param>
        private void HandlePointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                // If it's not the left button, then exit.
                if (!e.GetCurrentPoint(AssociatedObject).Properties.IsLeftButtonPressed)
                    return;
            }

            if (ClickMode == ClickMode.Press)
                this.InvokeActions(e);
            else 
                _wasOverTarget = true;
        }

        /// <summary>
        /// Mark that a Holding event occurred.
        /// </summary>
        /// <param name="sender">UIElement</param>
        /// <param name="e">EventArgs</param>
        private void HandlePointerHolding(object sender, HoldingRoutedEventArgs e)
        {
            if (ClickMode == ClickMode.Hover)
            {
                this.InvokeActions(e);
            }
        }

        /// <summary>
        /// Mark that a PointerReleased occurred.
        /// </summary>
        /// <param name="sender">UIElement</param>
        /// <param name="e">EventArgs</param>
        private void HandlePointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_wasOverTarget && ClickMode == ClickMode.Release)
            {
                this.InvokeActions(e);
            }

            _wasOverTarget = false;
        }
    }
}
