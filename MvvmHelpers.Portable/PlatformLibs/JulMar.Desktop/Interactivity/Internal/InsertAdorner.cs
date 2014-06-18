using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace JulMar.Interactivity.Internal
{
    /// <summary>
    /// Insertion Adorner - this was taken from a sample posted by Bea Stollnitz
    /// See http://www.beacosta.com/blog/?p=53 for the original article.
    /// </summary>
    class InsertAdorner : Adorner, IDisposable
    {
        #region Data
        private readonly UIElement _parentContainer;
        private readonly bool _isSeparatorHorizontal;
        private readonly AdornerLayer _adornerLayer;
        private static readonly Pen Pen;
        private static readonly PathGeometry Triangle;
        #endregion

        public bool InTopHalf { get; set; }

        // Create the pen and triangle in a static constructor and freeze them to improve performance.
        static InsertAdorner()
        {
            Pen = new Pen { Brush = Brushes.Gray, Thickness = 2 };
            Pen.Freeze();

            LineSegment firstLine = new LineSegment(new Point(0, -5), false);
            firstLine.Freeze();
            LineSegment secondLine = new LineSegment(new Point(0, 5), false);
            secondLine.Freeze();

            PathFigure figure = new PathFigure { StartPoint = new Point(5, 0) };
            figure.Segments.Add(firstLine);
            figure.Segments.Add(secondLine);
            figure.Freeze();

            Triangle = new PathGeometry();
            Triangle.Figures.Add(figure);
            Triangle.Freeze();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="isSeparatorHorizontal">Horizontal vs. vertical separator</param>
        /// <param name="isInFirstHalf">First half of items control</param>
        /// <param name="adornedElement">Element being adorner</param>
        /// <param name="adornerLayer">Layer</param>
        /// <param name="parentContainer">Parent container to constrain drawing</param>
        public InsertAdorner(bool isInFirstHalf, bool isSeparatorHorizontal, 
                                UIElement adornedElement, AdornerLayer adornerLayer, UIElement parentContainer)
            : base(adornedElement)
        {
            this._isSeparatorHorizontal = isSeparatorHorizontal;
            this.InTopHalf = isInFirstHalf;
            this._parentContainer = parentContainer;
            this._adornerLayer = adornerLayer;
            this.IsHitTestVisible = false;

            this._adornerLayer.Add(this);
        }

        // This draws one line and two triangles at each end of the line.
        protected override void OnRender(DrawingContext drawingContext)
        {
            Point startPoint;
            Point endPoint;

            this.CalculateStartAndEndPoint(out startPoint, out endPoint);
            drawingContext.DrawLine(Pen, startPoint, endPoint);

            if (this._isSeparatorHorizontal)
            {
                DrawTriangle(drawingContext, startPoint, 0);
                DrawTriangle(drawingContext, endPoint, 180);
            }
            else
            {
                DrawTriangle(drawingContext, startPoint, 90);
                DrawTriangle(drawingContext, endPoint, -90);
            }
        }

        private static void DrawTriangle(DrawingContext drawingContext, Point origin, double angle)
        {
            drawingContext.PushTransform(new TranslateTransform(origin.X, origin.Y));
            drawingContext.PushTransform(new RotateTransform(angle));

            drawingContext.DrawGeometry(Pen.Brush, null, Triangle);

            drawingContext.Pop();
            drawingContext.Pop();
        }

        /// <summary>
        /// Calculates the top and end points
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        private void CalculateStartAndEndPoint(out Point startPoint, out Point endPoint)
        {
            startPoint = new Point();
            endPoint = new Point();

            double width = this.AdornedElement.RenderSize.Width;
            double height = this.AdornedElement.RenderSize.Height;

            if (this._isSeparatorHorizontal)
            {
                endPoint.X = width;
                if (!this.InTopHalf)
                {
                    startPoint.Y = height;
                    endPoint.Y = height;
                }
            }
            else
            {
                endPoint.Y = height;
                if (!this.InTopHalf)
                {
                    startPoint.X = width;
                    endPoint.X = width;
                }
            }

            this.BoundPointToContainer(ref startPoint);
            this.BoundPointToContainer(ref endPoint);
        }

        /// <summary>
        /// Ensures our point does not escape the parent boundaries.
        /// </summary>
        /// <param name="pt"></param>
        private void BoundPointToContainer(ref Point pt)
        {
            Point boundedPt = this.AdornedElement.TranslatePoint(pt, this._parentContainer);
            if (boundedPt.X < 0) boundedPt.X = 0;
            else if (boundedPt.X > this._parentContainer.RenderSize.Width)
                boundedPt.X = this._parentContainer.RenderSize.Width;
            if (boundedPt.Y < 0) boundedPt.Y = 0;
            else if (boundedPt.Y > this._parentContainer.RenderSize.Height)
                boundedPt.Y = this._parentContainer.RenderSize.Height;
            pt = this._parentContainer.TranslatePoint(boundedPt, this.AdornedElement);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this._adornerLayer.Remove(this);
        }
    }
}
