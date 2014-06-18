using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Threading;

namespace JulMar.Interactivity
{
    /// <summary>
    /// This behavior sizes the ListView columns using another attached behavior for logic.
    /// </summary>
    public class ListViewColumnAutoSizeBehavior : Behavior<ListView>
    {
        /// <summary>
        /// Backing storage for Width property assigned to each GridViewColumn
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.RegisterAttached("Width", typeof(string), typeof(ListViewColumnAutoSizeBehavior));

        /// <summary>
        /// Getter for GridViewColumn attached property for width
        /// </summary>
        /// <param name="gvc">Column</param>
        /// <returns>Width</returns>
        public static string GetWidth(GridViewColumn gvc)
        {
            return (string)gvc.GetValue(WidthProperty);
        }

        /// <summary>
        /// Setter for GridViewColumn attached property for width
        /// </summary>
        /// <param name="gvc">Column</param>
        /// <param name="value">Value to assign</param>
        public static void SetWidth(GridViewColumn gvc, string value)
        {
            gvc.SetValue(WidthProperty, value);
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
            this.AssociatedObject.SizeChanged += this.OnSizeChanged;
            this.AssociatedObject.Dispatcher.BeginInvoke(((Action)this.OnResizeColumns), DispatcherPriority.Background);
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
            this.AssociatedObject.SizeChanged -= this.OnSizeChanged;
        }

        /// <summary>
        /// Called when the ListView size changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
                this.AssociatedObject.Dispatcher.BeginInvoke(((Action)this.OnResizeColumns), DispatcherPriority.Background);
        }

        /// <summary>
        /// This method performs the column resize. It is used when the ListView loads and also
        /// when the ListView changes size.
        /// </summary>
        private void OnResizeColumns()
        {
            // Ignore size change messages while we fiddle with column sizes.
            this.AssociatedObject.SizeChanged -= this.OnSizeChanged;

            try
            {
                double margin = 10; // Margin for spacing
                if (ScrollViewer.GetVerticalScrollBarVisibility(this.AssociatedObject) == ScrollBarVisibility.Visible
                    || ScrollViewer.GetVerticalScrollBarVisibility(this.AssociatedObject) == ScrollBarVisibility.Auto)
                    margin += SystemParameters.VerticalScrollBarWidth;

                double totalWidth = this.AssociatedObject.ActualWidth;
                if (totalWidth < 1)
                    return;

                var gv = this.AssociatedObject.View as GridView;
                if (gv != null)
                {
                    double allowedSpace = totalWidth - this.GetAllocatedSpace(gv) - margin;
                    if (allowedSpace < 0)
                        return;

                    double totalPercentage = gv.Columns.Select(GetWidth).Where(s => !string.IsNullOrEmpty(s)).Sum(s => this.ProportionalSizedColumn(s) * 100.0);
                    foreach (GridViewColumn column in gv.Columns)
                        this.SetColumnWidth(column, allowedSpace, totalPercentage);
                }
            }
            finally
            {
                // Turn back on size change notification
                this.AssociatedObject.SizeChanged += this.OnSizeChanged;
            }
        }

        /// <summary>
        /// This method is used to adjust the width of a GridViewColumn
        /// </summary>
        /// <param name="column">Column to adjust</param>
        /// <param name="totalSpace">Available space</param>
        /// <param name="totalPercentage">Total used percentage</param>
        private void SetColumnWidth(GridViewColumn column, double totalSpace, double totalPercentage)
        {
            string requestedWidth = GetWidth(column);
            if (string.IsNullOrEmpty(requestedWidth))
                return;

            if (this.IsAutoSizedColumn(requestedWidth))
            {
                if (!double.IsNaN(column.ActualWidth))
                    column.Width = column.ActualWidth;
                column.Width = Double.NaN;
            }
            else
            {
                double staticWidth = this.StaticSizedColumn(requestedWidth);
                if (!Double.IsNaN(staticWidth))
                    column.Width = staticWidth;
                else
                {
                    double width = totalSpace * ((this.ProportionalSizedColumn(requestedWidth) * 100.0) / totalPercentage);
                    column.Width = width;
                }
            }
        }

        /// <summary>
        /// This calculates reserved (non-proportional) space.
        /// </summary>
        /// <param name="gv"></param>
        /// <returns></returns>
        private double GetAllocatedSpace(GridView gv)
        {
            double totalWidth = 0;
            foreach (GridViewColumn t in gv.Columns)
            {
                string width = GetWidth(t);
                if (!string.IsNullOrEmpty(width))
                {
                    if (this.IsAutoSizedColumn(width))
                        totalWidth += t.ActualWidth;
                    else
                    {
                        double dw = this.StaticSizedColumn(width);
                        if (!double.IsNaN(dw))
                            totalWidth += dw;
                    }
                }
                else
                {
                    totalWidth += t.ActualWidth;
                }
            }
            return totalWidth;
        }

        /// <summary>
        /// Returns true if the specified column is an auto-sized column
        /// </summary>
        /// <param name="size">Requested size</param>
        /// <returns>True if auto</returns>
        private bool IsAutoSizedColumn(string size)
        {
            return string.Compare(size, "auto", true) == 0;
        }

        /// <summary>
        /// Returns the proper static size for a column if it's a number.
        /// </summary>
        /// <param name="size">Requested size</param>
        /// <returns>Size if known, or NaN if not a size.</returns>
        private double StaticSizedColumn(string size)
        {
            double result;
            return double.TryParse(size, out result) ? result : Double.NaN;
        }

        /// <summary>
        /// Returns the proportion specified for a column
        /// </summary>
        /// <param name="size">Requested size</param>
        /// <returns>Proportion (1.0, 2.0, etc.)</returns>
        private double ProportionalSizedColumn(string size)
        {
            if (size == "*" || size == "1*")
                return 1;
            if (size.EndsWith("*"))
            {
                double proportion;
                if (double.TryParse(size.Substring(0, size.Length - 1), out proportion))
                    return proportion;
            }
            return 0;
        }
    }
}
