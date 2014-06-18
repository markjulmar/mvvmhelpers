using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace JulMar.UI
{
    /// <summary>
    /// Simple Adorner to present a single FrameworkElement child in the adorner layer
    /// </summary>
    public class FrameworkElementAdorner : Adorner, IDisposable
    {
        private readonly FrameworkElement _child;

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        /// <returns>
        /// The number of visual child elements for this element.
        /// </returns>
        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)"/>, and returns a child at the specified index from a collection of child elements. 
        /// </summary>
        /// <returns>
        /// The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.
        /// </returns>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        protected override Visual GetVisualChild(int index)
        {
            return this._child;
        }

        /// <summary>
        /// Implements any custom measuring behavior for the adorner.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Windows.Size"/> object representing the amount of layout space needed by the adorner.
        /// </returns>
        /// <param name="constraint">A size to constrain the adorner to.</param>
        protected override Size MeasureOverride(Size constraint)
        {
            this._child.Measure(constraint);
            return this._child.DesiredSize;
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class. 
        /// </summary>
        /// <returns>
        /// The actual size used.
        /// </returns>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this._child.Arrange(new Rect(finalSize));
            return finalSize;
        }

        ///<summary>
        /// Constructor
        ///</summary>
        ///<param name="adornedElement">Element being adorned</param>
        ///<param name="child">Child element to place in adorner layer</param>
        public FrameworkElementAdorner(UIElement adornedElement, FrameworkElement child)
            : base(adornedElement)
        {
            this._child = child;
            this.AddVisualChild(this._child);
        }

        /// <summary>
        /// This static method creates and places the adorner - it is returned so it may
        /// be disposed when not needed anymore.
        /// </summary>
        /// <param name="adornedElement">Element being adorned</param>
        /// <param name="child">Child element to place in adorner layer</param>
        /// <returns>Created adorner</returns>
        public static FrameworkElementAdorner CreateElementAdorner(UIElement adornedElement, FrameworkElement child)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(adornedElement);
            if (layer != null)
            {
                var elementAdorner = new FrameworkElementAdorner(adornedElement, child);
                layer.Add(elementAdorner);
                return elementAdorner;
            }
            // No layer present - possibly too early in app cycle, or layer missing from
            // visual tree.
            return null;
        }

        #region Implementation of IDisposable

        /// <summary>
        /// Removes the adorner from the layer.
        /// </summary>
        public void Dispose()
        {
            try
            {
                var layer = AdornerLayer.GetAdornerLayer(this.AdornedElement);
                if (layer != null)
                    layer.Remove(this);
            }
            catch
            {
            }
        }

        #endregion
    }
}
