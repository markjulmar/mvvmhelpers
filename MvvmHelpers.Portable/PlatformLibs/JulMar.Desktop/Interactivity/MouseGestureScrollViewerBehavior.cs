using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Threading;

namespace JulMar.Interactivity
{
    /// <summary>
    /// This provides scrolling behavior using the mouse much like Touch events in Windows 7.
    /// </summary>
    public class MouseGestureScrollViewerBehavior : Behavior<ScrollViewer>
    {
        #region Private Data
        private bool _isCaptured;
        private Point _startingPoint, _endingPoint;
        private double _horizontalOffset, _verticalOffset, _inertiaX, _inertiaY;
        private DateTime _startDrag;
        private DispatcherTimer _timer;
        private const double EPSILON = 0.001;
        #endregion

        /// <summary>
        /// Backing storage for EnableInertia property
        /// </summary>
        public static readonly DependencyProperty EnableInertiaProperty =
            DependencyProperty.Register("EnableInertia", typeof (bool), typeof (MouseGestureScrollViewerBehavior), new PropertyMetadata(true));

        /// <summary>
        /// Enables gesture inertia (flings)
        /// </summary>
        public bool EnableInertia
        {
            get { return (bool) this.GetValue(EnableInertiaProperty); }
            set { this.SetValue(EnableInertiaProperty, value); }
        }

        /// <summary>
        /// Backing storage for EnablePageSwipe property
        /// </summary>
        public static readonly DependencyProperty EnablePageSwipeProperty =
            DependencyProperty.Register("EnablePageSwipe", typeof (bool), typeof (MouseGestureScrollViewerBehavior), new PropertyMetadata(default(bool)));

        /// <summary>
        /// True to not perform pixel movement, but instead move enter page at a time.
        /// </summary>
        public bool EnablePageSwipe
        {
            get { return (bool) this.GetValue(EnablePageSwipeProperty); }
            set { this.SetValue(EnablePageSwipeProperty, value); }
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
            this.AssociatedObject.PreviewMouseLeftButtonDown += this.OnPreviewMouseLeftButtonDown;
            this.AssociatedObject.PreviewMouseMove += this.OnPreviewMouseMove;
            this.AssociatedObject.PreviewMouseLeftButtonUp += this.OnPreviewMouseLeftButtonUp;
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
            this.AssociatedObject.PreviewMouseLeftButtonDown -= this.OnPreviewMouseLeftButtonDown;
            this.AssociatedObject.PreviewMouseMove -= this.OnPreviewMouseMove;
            this.AssociatedObject.PreviewMouseLeftButtonUp -= this.OnPreviewMouseLeftButtonUp;

            if (this._timer != null)
            {
                this._timer.Stop();
                this._timer = null;
            }
        }

        /// <summary>
        /// This is invoked when the mouse button is clicked on the ScrollViewer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this._timer != null)
            {
                this._timer.Stop();
                this._timer = null;
            }

            this._verticalOffset = this.AssociatedObject.VerticalOffset;
            this._horizontalOffset = this.AssociatedObject.HorizontalOffset;
            this._startingPoint = e.GetPosition(this.AssociatedObject);
            this._isCaptured = true;
            this._startDrag = DateTime.Now;

            this.AssociatedObject.CaptureMouse();
        }

        /// <summary>
        /// This is called when the mouse is moved.  It scrolls the contents based on mouse movement.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!this._isCaptured || e.LeftButton != MouseButtonState.Pressed)
                return;

            Point point = e.GetPosition(this.AssociatedObject);

            double dx = point.X - this._startingPoint.X;
            if (this.AssociatedObject.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled)
                this.AssociatedObject.ScrollToHorizontalOffset(this._horizontalOffset - dx);

            if (!this.EnablePageSwipe)
            {
                double dy = point.Y - this._startingPoint.Y;
                if (this.AssociatedObject.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled)
                    this.AssociatedObject.ScrollToVerticalOffset(this._verticalOffset - dy);
            }

            this._endingPoint = point;
        }

        /// <summary>
        /// This is called when the mouse is released.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.AssociatedObject.ReleaseMouseCapture();
            this._isCaptured = false;

            if (this.EnablePageSwipe)
            {
                if (this.AssociatedObject.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled)
                {
                    if (this._endingPoint.Y > this._startingPoint.Y)
                    {
                        this.AssociatedObject.PageUp();
                    }
                    else if (this._endingPoint.Y < this._startingPoint.Y)
                    {
                        this.AssociatedObject.PageDown();
                    }
                }
                
            }
            else if (this.EnableInertia)
            {
                if ((DateTime.Now - this._startDrag).TotalMilliseconds < 200)
                {
                    this._inertiaY = this._inertiaX = 0;
                    if (this.AssociatedObject.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled)
                    {
                        this._inertiaY = this._endingPoint.Y - this._startingPoint.Y;
                    }
                    if (this.AssociatedObject.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled)
                    {
                        this._inertiaX = this._endingPoint.X - this._startingPoint.X;
                    }

                    bool doFling = (Math.Abs(this._inertiaX) > 10 || Math.Abs(this._inertiaY) > 10);
                    if (doFling)
                    {
                        this._timer = new DispatcherTimer(TimeSpan.FromMilliseconds(50),
                            DispatcherPriority.Normal, this.OnFlingScroll, this.AssociatedObject.Dispatcher);
                    }
                }
            }
        }

        /// <summary>
        /// This is called on the timer when you "fling" the screen in a direction.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnFlingScroll(object sender, EventArgs e)
        {
            if (Math.Abs(this._inertiaY) > 0)
            {
                this.AssociatedObject.ScrollToVerticalOffset(this.AssociatedObject.VerticalOffset - this._inertiaY);
                if (this._inertiaY > 0)
                {
                    this._inertiaY -= this._inertiaY/2;
                    if (this._inertiaY < 0)
                        this._inertiaY = 0;
                }
                else
                {
                    this._inertiaY += Math.Abs(this._inertiaY)/2;
                    if (this._inertiaY > 0)
                        this._inertiaY = 0;
                }
            }

            if (Math.Abs(this._inertiaX) > 0)
            {
                this.AssociatedObject.ScrollToHorizontalOffset(this.AssociatedObject.HorizontalOffset - this._inertiaX);
                if (this._inertiaX > 0)
                {
                    this._inertiaX -= this._inertiaX/2;
                    if (this._inertiaX < 0)
                        this._inertiaX = 0;
                }
                else
                {
                    this._inertiaX += Math.Abs(this._inertiaX)/2;
                    if (this._inertiaX > 0)
                        this._inertiaX = 0;
                }
            }

            if (Math.Abs(this._inertiaX) < EPSILON && Math.Abs(this._inertiaY) < EPSILON)
            {
                if (this._timer != null)
                {
                    this._timer.Stop();
                    this._timer = null;
                }
            }
        }
    }
}