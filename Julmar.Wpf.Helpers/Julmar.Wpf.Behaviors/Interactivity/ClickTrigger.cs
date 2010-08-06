using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace JulMar.Windows.Interactivity
{
    /// <summary>
    /// This is a Blend/VS.NET behavior which drives a Click event to 
    /// some interactive action
    /// </summary>
    public class ClickTrigger : TriggerBase<UIElement>
    {
        private bool _wasOverTarget;

        /// <summary>
        /// The DependencyProperty for the ClickMode property.
        /// </summary>
        public static readonly DependencyProperty ClickModeProperty = DependencyProperty.Register("ClickMode", typeof(ClickMode), typeof(ClickTrigger), new FrameworkPropertyMetadata(ClickMode.Release));

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
            this.AssociatedObject.PreviewMouseDown += HandlePreviewMouseLeftButtonDown;
            this.AssociatedObject.PreviewMouseMove += HandlePreviewMouseMove;
            this.AssociatedObject.PreviewMouseUp += HandlePreviewMouseLeftButtonUp;
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
            this.AssociatedObject.PreviewMouseDown -= HandlePreviewMouseLeftButtonDown;
            this.AssociatedObject.PreviewMouseMove -= HandlePreviewMouseMove;
            this.AssociatedObject.PreviewMouseUp -= HandlePreviewMouseLeftButtonUp;
        }

        /// <summary>
        /// Mark that a ButtonDown occurred.
        /// </summary>
        /// <param name="sender">UIElement</param>
        /// <param name="mouseEventArgs">EventArgs</param>
        private void HandlePreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.LeftButton != MouseButtonState.Pressed)
                return;

            if (ClickMode == ClickMode.Press)
            {
                this.InvokeActions(mouseEventArgs);
            }
            else
                _wasOverTarget = true;
        }

        /// <summary>
        /// Mark that a ButtonDown occurred.
        /// </summary>
        /// <param name="sender">UIElement</param>
        /// <param name="mouseEventArgs">EventArgs</param>
        private void HandlePreviewMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            if (_wasOverTarget == false)
                return;

            if (ClickMode == ClickMode.Hover)
            {
                this.InvokeActions(mouseEventArgs);
            }
        }

        /// <summary>
        /// Mark that a ButtonDown occurred.
        /// </summary>
        /// <param name="sender">UIElement</param>
        /// <param name="mouseEventArgs">EventArgs</param>
        private void HandlePreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseEventArgs)
        {
            if (_wasOverTarget && ClickMode == ClickMode.Release)
            {
                this.InvokeActions(mouseEventArgs);
            }

            _wasOverTarget = false;
        }
    }
}
