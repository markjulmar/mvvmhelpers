using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace JulMar.Interactivity
{
    /// <summary>
    /// This Blend behavior provides positional translation for UIElements through a 
    /// RenderTransform using Drag/Drop semantics.
    /// </summary>
    public class DragPositionBehavior : Behavior<UIElement>
    {
        #region IsEnabledProperty
        /// <summary>
        /// This property allows the behavior to be used as a traditional
        /// attached property behavior.
        /// </summary>
        public static DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(DragPositionBehavior),
                new FrameworkPropertyMetadata(false, OnIsEnabledChanged));

        /// <summary>
        /// Returns whether DragPositionBehavior is enabled via attached property
        /// </summary>
        /// <param name="uie">Element</param>
        /// <returns>True/False</returns>
        public static bool GetIsEnabled(DependencyObject uie)
        {
            return (bool)uie.GetValue(IsEnabledProperty);
        }

        /// <summary>
        /// Adds DragPositionBehavior to an element
        /// </summary>
        /// <param name="uie">Element to apply</param>
        /// <param name="value">True/False</param>
        public static void SetIsEnabled(DependencyObject uie, bool value)
        {
            uie.SetValue(IsEnabledProperty, value);
        }

        /// <summary>
        /// This is called when the IsEnabled property has changed.
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="e"></param>
        private static void OnIsEnabledChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            UIElement uie = dpo as UIElement;
            if (uie != null)
            {
                var behColl = Interaction.GetBehaviors(uie);
                var existingBehavior = behColl.OfType<DragPositionBehavior>().FirstOrDefault();
                if ((bool)e.NewValue == false && existingBehavior != null)
                {
                    behColl.Remove(existingBehavior);
                }
                else if ((bool)e.NewValue == true && existingBehavior == null)
                {
                    behColl.Add(new DragPositionBehavior());
                }
            }
        }
        #endregion

        /// <summary>
        /// This class encapsulates the drag data + logic for a given element.
        /// It saves memory space as it is only allocated while the object is
        /// being dragged around.
        /// </summary>
        internal class UIElementMouseDrag
        {
            private Point _startPos;
            private bool _adjustCanvasCoordinates, _movedItem;
            private TranslateTransform _translatePos;

            public void OnMouseDown(UIElement uie, MouseButtonEventArgs e)
            {
                UIElement parent = VisualTreeHelper.GetParent(uie) as UIElement;
                this._adjustCanvasCoordinates = (parent != null && parent.GetType() == typeof (Canvas));

                this._startPos = e.GetPosition(null);

                uie.PreviewMouseUp += this.OnMouseUp;
                uie.PreviewMouseMove += this.OnMouseMove;
                uie.LostMouseCapture += this.OnLostMouseCapture;

                uie.CaptureMouse();
            }

            private void OnMouseUp(object sender, MouseButtonEventArgs e)
            {
                UIElement uie = (UIElement) sender;
                uie.ReleaseMouseCapture();
                if (this._movedItem)
                    e.Handled = true;
            }

            private void OnLostMouseCapture(object sender, MouseEventArgs e)
            {
                UIElement uie = (UIElement)sender;
                uie.PreviewMouseMove -= this.OnMouseMove;
                uie.PreviewMouseUp -= this.OnMouseUp;
            }

            public void OnMouseMove(object sender, MouseEventArgs e)
            {
                UIElement uie = (UIElement) sender;

                Point currentPosition = e.GetPosition(null);

                if (!this._movedItem)
                {
                    if (Math.Abs(currentPosition.X-this._startPos.X) <= 5 &&
                        Math.Abs(currentPosition.Y-this._startPos.Y) <= 5)
                        return;
                }

                this._movedItem = true;
                e.Handled = true;

                if (this._adjustCanvasCoordinates)
                {
                    double adjustX = currentPosition.X - this._startPos.X;
                    double adjustY = currentPosition.Y - this._startPos.Y;

                    Canvas.SetLeft(uie, Canvas.GetLeft(uie) + adjustX);
                    Canvas.SetTop(uie, Canvas.GetTop(uie) + adjustY);

                    this._startPos = currentPosition;
                }
                else // Not in a Canvas - use a RenderTransform instead.
                {
                    if (this._translatePos != null)
                    {
                        this._translatePos.X = currentPosition.X - this._startPos.X;
                        this._translatePos.Y = currentPosition.Y - this._startPos.Y;
                    }
                    else
                    {
                        this._translatePos = new TranslateTransform { X = currentPosition.X - this._startPos.X, Y = currentPosition.Y - this._startPos.Y };

                        // Replace existing transform if it exists.
                        TransformGroup transformGroup = new TransformGroup();
                        if (uie.RenderTransform != null)
                            transformGroup.Children.Add(uie.RenderTransform);
                        transformGroup.Children.Add(this._translatePos);
                        uie.RenderTransform = transformGroup;
                    }
                }
            }
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
            this.AssociatedObject.AddHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseDown), true);
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
            this.AssociatedObject.RemoveHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseDown));
        }

        /// <summary>
        /// Handles the MouseDown event
        /// </summary>
        /// <param name="sender">UIElement</param>
        /// <param name="e">Mouse eventargs</param>
        private static void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && 
                e.LeftButton == MouseButtonState.Pressed)
            {
                // Begin the drag operation
                UIElement uie = (UIElement)sender;
                var currentDragInfo = new UIElementMouseDrag();
                currentDragInfo.OnMouseDown(uie, e);
            }
        }
    }
}