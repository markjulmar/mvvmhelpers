﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using System.Windows.Threading;

// Taken from mvvmhelpers.codeplex.com (JulMar.Behaviors.dll) 4.08

namespace JulMar.Windows.Interactivity
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
            get { return (bool) GetValue(EnableInertiaProperty); }
            set { SetValue(EnableInertiaProperty, value); }
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
            get { return (bool) GetValue(EnablePageSwipeProperty); }
            set { SetValue(EnablePageSwipeProperty, value); }
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
            AssociatedObject.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseMove += OnPreviewMouseMove;
            AssociatedObject.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
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
            AssociatedObject.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseMove -= OnPreviewMouseMove;
            AssociatedObject.PreviewMouseLeftButtonUp -= OnPreviewMouseLeftButtonUp;

            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }
        }

        /// <summary>
        /// This is invoked when the mouse button is clicked on the ScrollViewer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }

            _verticalOffset = AssociatedObject.VerticalOffset;
            _horizontalOffset = AssociatedObject.HorizontalOffset;
            _startingPoint = e.GetPosition(AssociatedObject);
            _isCaptured = true;
            _startDrag = DateTime.Now;

            AssociatedObject.CaptureMouse();
        }

        /// <summary>
        /// This is called when the mouse is moved.  It scrolls the contents based on mouse movement.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isCaptured || e.LeftButton != MouseButtonState.Pressed)
                return;

            Point point = e.GetPosition(AssociatedObject);

            double dx = point.X - _startingPoint.X;
            if (AssociatedObject.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled)
                AssociatedObject.ScrollToHorizontalOffset(_horizontalOffset - dx);

            if (!EnablePageSwipe)
            {
                double dy = point.Y - _startingPoint.Y;
                if (AssociatedObject.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled)
                    AssociatedObject.ScrollToVerticalOffset(_verticalOffset - dy);
            }

            _endingPoint = point;
        }

        /// <summary>
        /// This is called when the mouse is released.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AssociatedObject.ReleaseMouseCapture();
            _isCaptured = false;

            if (EnablePageSwipe)
            {
                if (AssociatedObject.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled)
                {
                    if (_endingPoint.Y > _startingPoint.Y)
                    {
                        AssociatedObject.PageUp();
                    }
                    else if (_endingPoint.Y < _startingPoint.Y)
                    {
                        AssociatedObject.PageDown();
                    }
                }
                
            }
            else if (EnableInertia)
            {
                if ((DateTime.Now - _startDrag).TotalMilliseconds < 200)
                {
                    _inertiaY = _inertiaX = 0;
                    if (AssociatedObject.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled)
                    {
                        _inertiaY = _endingPoint.Y - _startingPoint.Y;
                    }
                    if (AssociatedObject.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled)
                    {
                        _inertiaX = _endingPoint.X - _startingPoint.X;
                    }

                    bool doFling = (Math.Abs(_inertiaX) > 10 || Math.Abs(_inertiaY) > 10);
                    if (doFling)
                    {
                        _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(50),
                            DispatcherPriority.Normal, OnFlingScroll, AssociatedObject.Dispatcher);
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
            if (Math.Abs(_inertiaY) > 0)
            {
                AssociatedObject.ScrollToVerticalOffset(AssociatedObject.VerticalOffset - _inertiaY);
                if (_inertiaY > 0)
                {
                    _inertiaY -= _inertiaY/2;
                    if (_inertiaY < 0)
                        _inertiaY = 0;
                }
                else
                {
                    _inertiaY += Math.Abs(_inertiaY)/2;
                    if (_inertiaY > 0)
                        _inertiaY = 0;
                }
            }

            if (Math.Abs(_inertiaX) > 0)
            {
                AssociatedObject.ScrollToHorizontalOffset(AssociatedObject.HorizontalOffset - _inertiaX);
                if (_inertiaX > 0)
                {
                    _inertiaX -= _inertiaX/2;
                    if (_inertiaX < 0)
                        _inertiaX = 0;
                }
                else
                {
                    _inertiaX += Math.Abs(_inertiaX)/2;
                    if (_inertiaX > 0)
                        _inertiaX = 0;
                }
            }

            if (Math.Abs(_inertiaX) < EPSILON && Math.Abs(_inertiaY) < EPSILON)
            {
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer = null;
                }
            }
        }
    }
}