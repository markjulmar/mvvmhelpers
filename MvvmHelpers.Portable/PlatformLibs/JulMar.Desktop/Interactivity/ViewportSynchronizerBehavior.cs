using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

using JulMar.Extensions;

namespace JulMar.Interactivity
{
    /// <summary>
    /// This behavior allows a ViewModel to monitor and control the scrolling position of any ScrollViewer or 
    /// control with a ScrollViewer in the template.  It can also be used to synchronize two scrolling items
    /// against a single property in a ViewModel.
    /// </summary>
    public class ViewportSynchronizerBehavior : Behavior<Control>
    {
        private ScrollViewer _scrollViewer;

        /// <summary>
        /// Vertical offset of the scroll viewer
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty = 
            DependencyProperty.Register("VerticalOffset", typeof(double),
                typeof(ViewportSynchronizerBehavior), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnVerticalOffsetChanged));

        /// <summary>
        /// The vertical offset
        /// </summary>
        public double VerticalOffset
        {
            get { return (double) this.GetValue(VerticalOffsetProperty);  }    
            set { this.SetValue(VerticalOffsetProperty, value);}
        }

        /// <summary>
        /// Vertical height of the scroll viewer
        /// </summary>
        public static readonly DependencyProperty ViewportHeightProperty = DependencyProperty.Register("ViewportHeight", typeof (double), typeof (ViewportSynchronizerBehavior),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// The vertical height of the scrollviewer
        /// </summary>
        public double ViewportHeight
        {
            get { return (double) this.GetValue(ViewportHeightProperty); }
            set { this.SetValue(ViewportHeightProperty, value); }
        }

        /// <summary>
        /// Horizontal width of the scroll viewer
        /// </summary>
        public static readonly DependencyProperty ViewportWidthProperty = DependencyProperty.Register("ViewportWidth", typeof(double), typeof(ViewportSynchronizerBehavior),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// The horizontal width of the scrollviewer
        /// </summary>
        public double ViewportWidth
        {
            get { return (double)this.GetValue(ViewportWidthProperty); }
            set { this.SetValue(ViewportWidthProperty, value); }
        }

        /// <summary>
        /// Horizontal offset of the scroll viewer
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.Register("HorizontalOffset", typeof(double),
                typeof(ViewportSynchronizerBehavior), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnHorizontalOffsetChanged));

        /// <summary>
        /// The horizontal offset
        /// </summary>
        public double HorizontalOffset
        {
            get { return (double)this.GetValue(HorizontalOffsetProperty); }
            set { this.SetValue(HorizontalOffsetProperty, value); }
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

            if (!this.AssociatedObject.IsLoaded)
            {
                RoutedEventHandler loadAction = null;
                loadAction = (s,e) => { this.AssociatedObject.Loaded -= loadAction; this.FindAndAttachToScrollViewer(); };
                this.AssociatedObject.Loaded += loadAction;
            }
            else this.FindAndAttachToScrollViewer();
        }

        /// <summary>
        /// Attaches to the scrollviewer.
        /// </summary>
        private bool FindAndAttachToScrollViewer()
        {
            // Find the first scroll viewer in the visual tree.
            this._scrollViewer = this.AssociatedObject as ScrollViewer ?? this.AssociatedObject.FindVisualChild<ScrollViewer>();
            if (this._scrollViewer != null)
            {
                this.ViewportHeight = this._scrollViewer.ViewportHeight;
                this.ViewportWidth = this._scrollViewer.ViewportWidth;

                // Position the scrollbar based on the current values.
                this._scrollViewer.ScrollToVerticalOffset(this.VerticalOffset);
                this._scrollViewer.ScrollToHorizontalOffset(this.HorizontalOffset);

                this._scrollViewer.ScrollChanged += this.ScrollViewerScrollChanged;
                this._scrollViewer.SizeChanged += this.ScrollViewerSizeChanged;
                return true;
            }

            // Possibly hasn't been instantiated yet?
            if (this.AssociatedObject.Visibility == Visibility.Collapsed)
            {
                this.AssociatedObject.SizeChanged += this.AssociatedObjectSizeChanged;
            }

            return false;
        }

        /// <summary>
        /// This is called when the associated object's size changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AssociatedObjectSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize != Size.Empty)
            {
                if (this.FindAndAttachToScrollViewer())
                    this.AssociatedObject.SizeChanged -= this.AssociatedObjectSizeChanged;
            }
        }

        /// <summary>
        /// Called when the scrollviewer changes the size
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ScrollViewerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.ViewportHeight = this._scrollViewer.ViewportHeight;
            this.ViewportWidth = this._scrollViewer.ViewportWidth;
        }

        /// <summary>
        /// This method is called when the scroll viewer is scrolled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.HorizontalChange != 0)
                this.HorizontalOffset = e.HorizontalOffset;
            if (e.VerticalChange != 0)
                this.VerticalOffset = e.VerticalOffset;
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>
        /// Override this to unhook functionality from the AssociatedObject.
        /// </remarks>
        protected override void OnDetaching()
        {
            if (this._scrollViewer != null)
            {
                this._scrollViewer.ScrollChanged -= this.ScrollViewerScrollChanged;
                this._scrollViewer.SizeChanged += this.ScrollViewerSizeChanged;
            }
            base.OnDetaching();
        }

        /// <summary>
        /// This method is called when the VerticalOffset property is changed.
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="e"></param>
        private static void OnVerticalOffsetChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            var vsb = (ViewportSynchronizerBehavior) dpo;
            if (vsb._scrollViewer != null)
                vsb._scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
        }

        /// <summary>
        /// This method is called when HorizontalOffset property is changed.
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="e"></param>
        private static void OnHorizontalOffsetChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            var vsb = (ViewportSynchronizerBehavior)dpo;
            if (vsb._scrollViewer != null)
                vsb._scrollViewer.ScrollToHorizontalOffset((double)e.NewValue);
        }
    }
}
