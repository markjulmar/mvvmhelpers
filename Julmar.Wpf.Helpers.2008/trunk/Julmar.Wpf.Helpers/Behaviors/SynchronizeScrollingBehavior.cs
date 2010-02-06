using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using JulMar.Windows.Extensions;

namespace JulMar.Windows.Behaviors
{
    /// <summary>
    /// This class provides synchronized scrolling behavior for two controls which use ScrollViewer.
    /// The first scroll viewer found in the Visual Tree is used.
    /// </summary>
    public static class SynchronizeScrollingBehavior
    {
        /// <summary>
        /// This dependency property holds the horizontal adjustment factor when two scrollviewer instances do not have the
        /// same size or elements.
        /// </summary>
        public static readonly DependencyProperty HorizontalAdjustmentProperty = DependencyProperty.RegisterAttached("HorizontalAdjustment", typeof(double), typeof(SynchronizeScrollingBehavior), new UIPropertyMetadata(0.0));

        /// <summary>
        /// This dependency property holds the vertical adjustment factor when two scrollviewer instances do not have the
        /// same size or elements.
        /// </summary>
        public static readonly DependencyProperty VerticalAdjustmentProperty = DependencyProperty.RegisterAttached("VerticalAdjustment", typeof(double), typeof(SynchronizeScrollingBehavior), new UIPropertyMetadata(0.0));

        /// <summary>
        /// This holds the target to synchronize to.
        /// </summary>
        public static readonly DependencyProperty TargetProperty = DependencyProperty.RegisterAttached("Target", typeof(UIElement), typeof(SynchronizeScrollingBehavior), new UIPropertyMetadata(null, OnTargetChanged));

        /// <summary>
        /// This holds the source to synchronize to.
        /// </summary>
        private static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached("_Source", typeof(UIElement), typeof(SynchronizeScrollingBehavior), new UIPropertyMetadata(null));

        /// <summary>
        /// Property getter for the Target property
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static UIElement GetTarget(DependencyObject source)
        {
            return (UIElement) source.GetValue(TargetProperty);
        }

        /// <summary>
        /// Property setter for the Target property
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void SetTarget(DependencyObject source, UIElement target)
        {
            source.SetValue(TargetProperty, target);
        }

        /// <summary>
        /// Property getter for the HorizontalAdjustment property
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double GetHorizontalAdjustment(DependencyObject source)
        {
            return (double)source.GetValue(HorizontalAdjustmentProperty);
        }

        /// <summary>
        /// Property setter for the HorizontalAdjustment property
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        public static void SetHorizontalAdjustment(DependencyObject source, double value)
        {
            source.SetValue(HorizontalAdjustmentProperty, value);
        }

        /// <summary>
        /// Property getter for the VerticalAdjustment property
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double GetVerticalAdjustment(DependencyObject source)
        {
            return (double)source.GetValue(VerticalAdjustmentProperty);
        }

        /// <summary>
        /// Property setter for the VerticalAdjustment property
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        public static void SetVerticalAdjustment(DependencyObject source, double value)
        {
            source.SetValue(VerticalAdjustmentProperty, value);
        }

        /// <summary>
        /// This method is called when the target property is changed.
        /// </summary>
        /// <param name="dpo">Dependency object</param>
        /// <param name="e">Event Args</param>
        static void OnTargetChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            var source = dpo as UIElement;
            if (source == null)
                return;

            source.RemoveHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(OnSourceScroll));

            var target = e.OldValue as UIElement;
            if (target != null)
            {
                target.SetValue(SourceProperty, null);
                target.RemoveHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(OnTargetScroll));
            }

            target = e.NewValue as UIElement;
            if (target != null)
            {
                target.SetValue(SourceProperty, source);
                source.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(OnSourceScroll));
                target.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(OnTargetScroll));
            }
        }

        /// <summary>
        /// This handles the synchronization when the source list is scrolled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnSourceScroll(object sender, ScrollChangedEventArgs e)
        {
            var uiElem = (UIElement) sender;
            Debug.Assert(uiElem != null);
            var target = GetTarget(uiElem);
            Debug.Assert(target != null);
            var sv = target.FindVisualChild<ScrollViewer>();
            Debug.Assert(sv != null);

            AdjustScrollPosition(sv, e, -1 * GetHorizontalAdjustment(uiElem), -1 * GetVerticalAdjustment(uiElem));
        }

        /// <summary>
        /// This handles the synchronization when the target list is scrolled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnTargetScroll(object sender, ScrollChangedEventArgs e)
        {
            var uiElem = (UIElement)sender;
            Debug.Assert(uiElem != null);
            var srcElem = uiElem.GetValue(SourceProperty) as UIElement;
            Debug.Assert(srcElem != null);
            var sv = srcElem.FindVisualChild<ScrollViewer>();
            Debug.Assert(sv != null);

            AdjustScrollPosition(sv, e, GetHorizontalAdjustment(srcElem), GetVerticalAdjustment(srcElem));
        }

        /// <summary>
        /// This is the command scroll adjustment code which synchronizes two ScrollViewer instances.
        /// </summary>
        /// <param name="sv">ScrollViewer to adjust</param>
        /// <param name="e">Change in the source</param>
        /// <param name="hadjust">Horizontal adjustment</param>
        /// <param name="vadjust">Vertical adjustment</param>
        private static void AdjustScrollPosition(ScrollViewer sv, ScrollChangedEventArgs e, double hadjust, double vadjust)
        {
            if (e.HorizontalChange != 0 || e.ExtentWidthChange != 0)
            {
                if (e.HorizontalOffset == 0)
                    sv.ScrollToLeftEnd();
                else if (e.HorizontalOffset >= e.ExtentWidth-5)
                    sv.ScrollToRightEnd();
                else if (e.ExtentWidth + hadjust == sv.ExtentWidth)
                    sv.ScrollToHorizontalOffset(e.HorizontalOffset + hadjust);
                else
                    sv.ScrollToHorizontalOffset((sv.ExtentWidth * (e.HorizontalOffset / e.ExtentWidth)) + hadjust);
            }
            if (e.VerticalChange != 0 || e.ExtentHeightChange != 0)
            {
                if (e.VerticalOffset == 0)
                    sv.ScrollToTop();
                else if (e.VerticalOffset >= e.ExtentHeight-5)
                    sv.ScrollToBottom();
                else if (e.ExtentHeight + vadjust == sv.ExtentHeight)
                    sv.ScrollToVerticalOffset(e.VerticalOffset + vadjust);
                else
                    sv.ScrollToVerticalOffset((sv.ExtentHeight * (e.VerticalOffset / e.ExtentHeight)) + vadjust);
            }
        }
    }
}
