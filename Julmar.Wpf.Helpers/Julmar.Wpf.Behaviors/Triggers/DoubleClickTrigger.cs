using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using JulMar.Core;

namespace JulMar.Windows.Interactivity
{
    /// <summary>
    /// This is a Blend/VS.NET behavior which drives a DoubleClick event to 
    /// some interactive action
    /// </summary>
    public class DoubleClickTrigger : TriggerBase<UIElement>
    {
        private uint _lastClick;

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.AddHandler(Mouse.MouseUpEvent, new MouseButtonEventHandler(HandleButtonUp), true);
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
            this.AssociatedObject.RemoveHandler(Mouse.MouseUpEvent, new MouseButtonEventHandler(HandleButtonUp));
        }

        /// <summary>
        /// This handles the UIElement.LeftButtonDown event to test for a double-click event.
        /// </summary>
        /// <param name="sender">UIElement</param>
        /// <param name="mouseEventArgs">EventArgs</param>
        private void HandleButtonUp(object sender, MouseButtonEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.ChangedButton == MouseButton.Left &&
                ((uint)mouseEventArgs.Timestamp - _lastClick) < SystemInfo.DoubleClickTime)
            {
                this.InvokeActions(mouseEventArgs);
                _lastClick = 0; // Require 2 clicks again
            }
            else 
                _lastClick = (uint) mouseEventArgs.Timestamp;
        }
    }
}
