using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Xaml.Behaviors;
using System.Windows.Threading;
using JulMar.Windows.Extensions;

namespace JulMar.Windows.Interactivity
{
    /// <summary>
    /// Displays a ToolTip next to the ScrollBar thumb while it is being dragged.  
    /// The original idea and code was taken from an MSDN sample - see http://code.msdn.microsoft.com/getwpfcode/Release/ProjectReleases.aspx?ReleaseId=1445
    /// for the original source code and project.
    /// </summary>
    public class ScrollbarPreviewBehavior : Behavior<ScrollBar>
    {
        #region ScrollingPreviewTemplate
        /// <summary>
        /// Specifies a ContentTemplate for a ToolTip that will appear next to the ScrollBar while dragging the thumb.
        /// </summary>
        public static readonly DependencyProperty ScrollingPreviewTemplateProperty = DependencyProperty.Register("ScrollingPreviewTemplate", typeof(DataTemplate), typeof(ScrollbarPreviewBehavior), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets/Sets the vertical scrolling preview template
        /// </summary>
        public DataTemplate ScrollingPreviewTemplate
        {
            get { return (DataTemplate) GetValue(ScrollingPreviewTemplateProperty); }
            set { SetValue(ScrollingPreviewTemplateProperty, value); }
        }
        #endregion

        /// <summary>
        /// Holds the ToolTip when it is being used.
        /// </summary>
        private ToolTip _previewToolTip;

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();
            if (ScrollingPreviewTemplate != null)
            {
                Dispatcher.BeginInvoke(new Action(AttachToScrollBar),
                    DispatcherPriority.Loaded);
            }
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

            AssociatedObject.SizeChanged -= ScrollBar_SizeChanged;
            Track track = AssociatedObject.Track ?? AssociatedObject.FindTemplateName<Track>("PART_Track");
            if (track != null)
            {
                Thumb thumb = track.Thumb;
                if (thumb != null)
                {
                    thumb.DragStarted -= ThumbDragStarted;
                    thumb.DragDelta -= ThumbDragDelta;
                    thumb.DragCompleted -= ThumbDragCompleted;
                    AssociatedObject.Scroll -= ScrollBar_Scroll;
                }
            }
        }

        private void AttachToScrollBar()
        {
            Track track = AssociatedObject.Track ?? AssociatedObject.FindTemplateName<Track>("PART_Track");
            if (track == null)
            {
                if (AssociatedObject.Visibility != Visibility.Visible)
                {
                    AssociatedObject.SizeChanged += ScrollBar_SizeChanged;
                }
            }
            else // if (track != null)
            {
                Thumb thumb = track.Thumb;
                if (thumb != null)
                {
                    thumb.DragStarted += ThumbDragStarted;
                    thumb.DragDelta += ThumbDragDelta;
                    thumb.DragCompleted += ThumbDragCompleted;
                    AssociatedObject.Scroll += ScrollBar_Scroll;
                }
            }
        }

        void ScrollBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScrollBar scrollBar = (ScrollBar)sender;
            if (scrollBar.Track != null)
            {
                scrollBar.SizeChanged -= ScrollBar_SizeChanged;
                AttachToScrollBar();
            }
        }

        void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            var toolTip = _previewToolTip;
            if (toolTip != null && toolTip.Content != null)
            {
                // The ScrollBar's value isn't updated quite yet, so wait until Input priority
                AssociatedObject.Dispatcher.BeginInvoke((Action)(() => ((ScrollingPreviewData) toolTip.Content).UpdateScrollingValues(AssociatedObject)), DispatcherPriority.Input);
            }
        }

        void ThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            _previewToolTip = new ToolTip();
            ScrollingPreviewData data = new ScrollingPreviewData();
            _previewToolTip.Content = data;

            // Update the content in the ToolTip
            data.UpdateScrollingValues(AssociatedObject);

            // Set the Placement and the PlacementTarget
            _previewToolTip.PlacementTarget = (UIElement)sender;
            _previewToolTip.Placement = AssociatedObject.Orientation == Orientation.Vertical ? PlacementMode.Left : PlacementMode.Top;

            _previewToolTip.VerticalOffset = 0.0;
            _previewToolTip.HorizontalOffset = 0.0;

            _previewToolTip.ContentTemplate = ScrollingPreviewTemplate;
            _previewToolTip.IsOpen = true;
        }
        
        void ThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            // Check that we're within the range of the ScrollBar
            if ((_previewToolTip != null) &&
                (AssociatedObject.Value > AssociatedObject.Minimum) &&
                (AssociatedObject.Value < AssociatedObject.Maximum))
            {
                // This is a little trick to cause the ToolTip to update its position next to the Thumb
                if (AssociatedObject.Orientation == Orientation.Vertical)
                    _previewToolTip.VerticalOffset = _previewToolTip.VerticalOffset == 0.0 ? 0.001 : 0.0;
                else
                    _previewToolTip.HorizontalOffset = _previewToolTip.HorizontalOffset == 0.0 ? 0.001 : 0.0;
            }
        }

        void ThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (_previewToolTip != null)
            {
                _previewToolTip.IsOpen = false;
                _previewToolTip = null;
            }
        }

        /// <summary>
        ///     Provides data that should be useful to templates displaying
        ///     a preview.
        /// </summary>
        internal class ScrollingPreviewData : INotifyPropertyChanged
        {
            private double _offset, _viewport, _extent, _value;

            /// <summary>
            /// The Scrollbar's current value
            /// </summary>
            public double Value
            {
                get { return _value; }
                set { _value = value; OnPropertyChanged("Value"); }
            }

            /// <summary>
            ///     The ScrollBar's offset.
            /// </summary>
            public double Offset
            {
                get
                {
                    return _offset;
                }
                internal set
                {
                    _offset = value;
                    OnPropertyChanged("Offset");
                }
            }

            /// <summary>
            ///     The size of the current viewport.
            /// </summary>
            public double Viewport
            {
                get
                {
                    return _viewport;
                }
                internal set
                {
                    _viewport = value;
                    OnPropertyChanged("Viewport");
                }
            }

            /// <summary>
            ///     The entire scrollable range.
            /// </summary>
            public double Extent
            {
                get
                {
                    return _extent;
                }
                internal set
                {
                    _extent = value;
                    OnPropertyChanged("Extent");
                }
            }

            /// <summary>
            ///     Updates Offset, Viewport, and Extent.
            /// </summary>
            internal void UpdateScrollingValues(ScrollBar scrollBar)
            {
                Offset = scrollBar.Value;
                Viewport = scrollBar.ViewportSize;
                Value = scrollBar.Value;
                Extent = scrollBar.Maximum - scrollBar.Minimum;
            }

            #region INotifyPropertyChanged Members

            /// <summary>
            ///     Notifies listeners of changes to properties on this object.
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            ///     Raises the PropertyChanged event.
            /// </summary>
            /// <param name="name">The name of the property.</param>
            protected void OnPropertyChanged(string name)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
            }

            #endregion
        }
    }
}