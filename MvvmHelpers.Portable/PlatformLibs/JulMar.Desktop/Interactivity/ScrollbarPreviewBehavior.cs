using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;
using System.Windows.Threading;

using JulMar.Extensions;

namespace JulMar.Interactivity
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
            get { return (DataTemplate) this.GetValue(ScrollingPreviewTemplateProperty); }
            set { this.SetValue(ScrollingPreviewTemplateProperty, value); }
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
            if (this.ScrollingPreviewTemplate != null)
            {
                this.Dispatcher.BeginInvoke(new Action(this.AttachToScrollBar),
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

            this.AssociatedObject.SizeChanged -= this.ScrollBar_SizeChanged;
            Track track = this.AssociatedObject.Track ?? this.AssociatedObject.FindTemplateName<Track>("PART_Track");
            if (track != null)
            {
                Thumb thumb = track.Thumb;
                if (thumb != null)
                {
                    thumb.DragStarted -= this.ThumbDragStarted;
                    thumb.DragDelta -= this.ThumbDragDelta;
                    thumb.DragCompleted -= this.ThumbDragCompleted;
                    this.AssociatedObject.Scroll -= this.ScrollBar_Scroll;
                }
            }
        }

        private void AttachToScrollBar()
        {
            Track track = this.AssociatedObject.Track ?? this.AssociatedObject.FindTemplateName<Track>("PART_Track");
            if (track == null)
            {
                if (this.AssociatedObject.Visibility != Visibility.Visible)
                {
                    this.AssociatedObject.SizeChanged += this.ScrollBar_SizeChanged;
                }
            }
            else // if (track != null)
            {
                Thumb thumb = track.Thumb;
                if (thumb != null)
                {
                    thumb.DragStarted += this.ThumbDragStarted;
                    thumb.DragDelta += this.ThumbDragDelta;
                    thumb.DragCompleted += this.ThumbDragCompleted;
                    this.AssociatedObject.Scroll += this.ScrollBar_Scroll;
                }
            }
        }

        void ScrollBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScrollBar scrollBar = (ScrollBar)sender;
            if (scrollBar.Track != null)
            {
                scrollBar.SizeChanged -= this.ScrollBar_SizeChanged;
                this.AttachToScrollBar();
            }
        }

        void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            var toolTip = this._previewToolTip;
            if (toolTip != null && toolTip.Content != null)
            {
                // The ScrollBar's value isn't updated quite yet, so wait until Input priority
                this.AssociatedObject.Dispatcher.BeginInvoke((Action)(() => ((ScrollingPreviewData) toolTip.Content).UpdateScrollingValues(this.AssociatedObject)), DispatcherPriority.Input);
            }
        }

        void ThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            this._previewToolTip = new ToolTip();
            ScrollingPreviewData data = new ScrollingPreviewData();
            this._previewToolTip.Content = data;

            // Update the content in the ToolTip
            data.UpdateScrollingValues(this.AssociatedObject);

            // Set the Placement and the PlacementTarget
            this._previewToolTip.PlacementTarget = (UIElement)sender;
            this._previewToolTip.Placement = this.AssociatedObject.Orientation == Orientation.Vertical ? PlacementMode.Left : PlacementMode.Top;

            this._previewToolTip.VerticalOffset = 0.0;
            this._previewToolTip.HorizontalOffset = 0.0;

            this._previewToolTip.ContentTemplate = this.ScrollingPreviewTemplate;
            this._previewToolTip.IsOpen = true;
        }
        
        void ThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            // Check that we're within the range of the ScrollBar
            if ((this._previewToolTip != null) &&
                (this.AssociatedObject.Value > this.AssociatedObject.Minimum) &&
                (this.AssociatedObject.Value < this.AssociatedObject.Maximum))
            {
                // This is a little trick to cause the ToolTip to update its position next to the Thumb
                if (this.AssociatedObject.Orientation == Orientation.Vertical)
                    this._previewToolTip.VerticalOffset = this._previewToolTip.VerticalOffset == 0.0 ? 0.001 : 0.0;
                else
                    this._previewToolTip.HorizontalOffset = this._previewToolTip.HorizontalOffset == 0.0 ? 0.001 : 0.0;
            }
        }

        void ThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (this._previewToolTip != null)
            {
                this._previewToolTip.IsOpen = false;
                this._previewToolTip = null;
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
                get { return this._value; }
                set { this._value = value; this.OnPropertyChanged("Value"); }
            }

            /// <summary>
            ///     The ScrollBar's offset.
            /// </summary>
            public double Offset
            {
                get
                {
                    return this._offset;
                }
                internal set
                {
                    this._offset = value;
                    this.OnPropertyChanged("Offset");
                }
            }

            /// <summary>
            ///     The size of the current viewport.
            /// </summary>
            public double Viewport
            {
                get
                {
                    return this._viewport;
                }
                internal set
                {
                    this._viewport = value;
                    this.OnPropertyChanged("Viewport");
                }
            }

            /// <summary>
            ///     The entire scrollable range.
            /// </summary>
            public double Extent
            {
                get
                {
                    return this._extent;
                }
                internal set
                {
                    this._extent = value;
                    this.OnPropertyChanged("Extent");
                }
            }

            /// <summary>
            ///     Updates Offset, Viewport, and Extent.
            /// </summary>
            internal void UpdateScrollingValues(ScrollBar scrollBar)
            {
                this.Offset = scrollBar.Value;
                this.Viewport = scrollBar.ViewportSize;
                this.Value = scrollBar.Value;
                this.Extent = scrollBar.Maximum - scrollBar.Minimum;
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
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
            }

            #endregion
        }
    }
}