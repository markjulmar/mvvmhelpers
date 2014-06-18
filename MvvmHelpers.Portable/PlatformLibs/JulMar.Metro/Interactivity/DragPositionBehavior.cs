using System;
using System.Linq;

using Windows.ApplicationModel;
using Windows.Devices.Input;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

using Microsoft.Xaml.Interactivity;

namespace JulMar.Interactivity
{
    /// <summary>
    /// This Blend behavior provides positional translation for elements through a 
    /// RenderTransform using Drag/Drop semantics.
    /// </summary>
    public class DragPositionBehavior : DependencyObject, IBehavior
    {
        #region IsEnabledProperty
        /// <summary>
        /// This property allows the behavior to be used as a traditional
        /// attached property behavior.
        /// </summary>
        public static DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(DragPositionBehavior),
                new PropertyMetadata(false, OnIsEnabledChanged));

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
            FrameworkElement uie = dpo as FrameworkElement;
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
        internal class ElementMouseDrag
        {
            private PointerPoint _startPos;
            private bool _adjustCanvasCoordinates, _movedItem;
            private TranslateTransform _translatePos;

            public void OnPointerDown(FrameworkElement uie, PointerRoutedEventArgs e)
            {
                FrameworkElement parent = VisualTreeHelper.GetParent(uie) as FrameworkElement;
                this._adjustCanvasCoordinates = (parent != null && parent.GetType() == typeof(Canvas));
                this._startPos = e.GetCurrentPoint(null);

                uie.PointerReleased += this.OnPointerReleased;
                uie.PointerMoved += this.OnPointerMoved;
                uie.PointerCaptureLost += this.OnLostCapture;
                uie.CapturePointer(e.Pointer);
            }

            private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
            {
                FrameworkElement uie = (FrameworkElement)sender;
                uie.ReleasePointerCapture(e.Pointer);
                if (this._movedItem)
                    e.Handled = true;
            }

            private void OnLostCapture(object sender, PointerRoutedEventArgs e)
            {
                FrameworkElement uie = (FrameworkElement)sender;
                uie.PointerReleased -= this.OnPointerReleased;
                uie.PointerMoved -= this.OnPointerMoved;
            }

            public void OnPointerMoved(object sender, PointerRoutedEventArgs e)
            {
                FrameworkElement uie = (FrameworkElement)sender;

                PointerPoint currentPosition = e.GetCurrentPoint(null);

                if (!this._movedItem)
                {
                    if (Math.Abs(currentPosition.Position.X - this._startPos.Position.X) <= 5 &&
                        Math.Abs(currentPosition.Position.Y - this._startPos.Position.Y) <= 5)
                        return;
                }

                this._movedItem = true;
                e.Handled = true;

                if (this._adjustCanvasCoordinates)
                {
                    double adjustX = currentPosition.Position.X - this._startPos.Position.X;
                    double adjustY = currentPosition.Position.Y - this._startPos.Position.Y;

                    Canvas.SetLeft(uie, Canvas.GetLeft(uie) + adjustX);
                    Canvas.SetTop(uie, Canvas.GetTop(uie) + adjustY);

                    this._startPos = currentPosition;
                }
                else // Not in a Canvas - use a RenderTransform instead.
                {
                    if (this._translatePos != null)
                    {
                        this._translatePos.X = currentPosition.Position.X - this._startPos.Position.X;
                        this._translatePos.Y = currentPosition.Position.Y - this._startPos.Position.Y;
                    }
                    else
                    {
                        this._translatePos = new TranslateTransform { X = currentPosition.Position.X - this._startPos.Position.X, Y = currentPosition.Position.Y - this._startPos.Position.Y };

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
        /// Handles the MouseDown event
        /// </summary>
        /// <param name="sender">UIElement</param>
        /// <param name="e">Event arguments</param>
        private static void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // Begin the drag operation
            FrameworkElement uie = (FrameworkElement) sender;
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                // If it's not the left button, then exit.
                if (!e.GetCurrentPoint(uie).Properties.IsLeftButtonPressed)
                    return;
            }

            var currentDragInfo = new ElementMouseDrag();
            currentDragInfo.OnPointerDown(uie, e);
        }

        /// <summary>
        /// Attaches to the specified object.
        /// </summary>
        /// <param name="associatedObject">The <see cref="T:Windows.UI.Xaml.DependencyObject"/> to which the <seealso cref="T:Microsoft.Xaml.Interactivity.IBehavior"/> will be attached.</param>
        public void Attach(DependencyObject associatedObject)
        {
            if ((associatedObject != this.AssociatedObject) && !DesignMode.DesignModeEnabled)
            {
                if (this.AssociatedObject != null)
                    throw new InvalidOperationException("Cannot attach behavior multiple times.");

                this.AssociatedObject = associatedObject;
                FrameworkElement fe = this.AssociatedObject as FrameworkElement;
                if (fe != null)
                {
                    fe.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
                }
            }
        }

        /// <summary>
        /// Detaches this instance from its associated object.
        /// </summary>
        public void Detach()
        {
            FrameworkElement fe = this.AssociatedObject as FrameworkElement;
            if (fe != null)
            {
                fe.RemoveHandler(UIElement.PointerPressedEvent, new PointerEventHandler(OnPointerPressed));
            }

            this.AssociatedObject = null;
        }

        /// <summary>
        /// Gets the <see cref="T:Windows.UI.Xaml.DependencyObject"/> to which the <seealso cref="T:Microsoft.Xaml.Interactivity.IBehavior"/> is attached.
        /// </summary>
        public DependencyObject AssociatedObject { get; private set; }
    }
}