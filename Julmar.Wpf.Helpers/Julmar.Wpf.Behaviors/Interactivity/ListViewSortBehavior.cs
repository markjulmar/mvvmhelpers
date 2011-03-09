using System.ComponentModel;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using JulMar.Windows.Extensions;

namespace JulMar.Windows.Interactivity
{
    /// <summary>
    /// This adorner draws the sorting arrow onto the ListView column
    /// header and provides the visual feedback for the sorting direction.
    /// </summary>
    internal class SortAdorner : Adorner
    {
        /// <summary>
        /// The direction to draw the arrow (up vs. down)
        /// </summary>
        public ListSortDirection Direction { get; set; }

        /// <summary>
        /// The color of the arrow
        /// </summary>
        public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush), typeof(SortAdorner),
                                new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// The color of the arrow
        /// </summary>
        public Brush Fill
        {
            get { return (Brush) GetValue(FillProperty); }
            set { SetValue(FillProperty, value);}
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="adornedElement">Element (ColumnHeader) to adorn</param>
        public SortAdorner(UIElement adornedElement) : base(adornedElement)
        {
        }

        /// <summary>
        /// Pen used to draw geometry (none)
        /// </summary>
        private static readonly Pen NoPen = new Pen();

        /// <summary>
        /// The geometry for the up arrow
        /// </summary>
        private static readonly Geometry UpGeometry = PathGeometry.Parse("M4,0 L0,6 L8,6 z");
        /// <summary>
        /// The geometry for the down arrow
        /// </summary>
        private static readonly Geometry DownGeometry = PathGeometry.Parse("M0,0 L8,0 L4,6 z");

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing. 
        /// </summary>
        /// <param name="dc">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext dc)
        {
            GridViewColumnHeader fe = (GridViewColumnHeader)AdornedElement;
            if (fe.ActualWidth - fe.DesiredSize.Width > 20)
            {
                dc.PushTransform(new TranslateTransform(fe.ActualWidth - 15, fe.ActualHeight / 2 - 3));
                dc.DrawGeometry(Fill, NoPen, Direction == ListSortDirection.Ascending ? UpGeometry : DownGeometry);
                dc.Pop();
            }
            base.OnRender(dc);
        }
    }

    /// <summary>
    /// Behavior to provide automatic sorting of a ListView.
    /// </summary>
    public class ListViewSortBehavior : Behavior<ListView>
    {
        private GridViewColumnHeader _sortingColumn;
        private SortAdorner _adorner;

        /// <summary>
        /// Initial column index
        /// </summary>
        public static readonly DependencyProperty InitialColumnIndexProperty =
            DependencyProperty.Register("InitialColumnIndex", typeof (int), typeof (ListViewSortBehavior),
                                        new PropertyMetadata(-1));

        /// <summary>
        /// Initial sorting column
        /// </summary>
        public int InitialColumnIndex
        {
            get { return (int) base.GetValue(InitialColumnIndexProperty); }
            set { base.SetValue(InitialColumnIndexProperty, value); }
        }

        /// <summary>
        /// Sort direction
        /// </summary>
        public static readonly DependencyProperty SortDirectionProperty =
            DependencyProperty.Register("SortDirection", typeof (ListSortDirection), typeof(ListViewSortBehavior),
                                        new PropertyMetadata(ListSortDirection.Ascending));

        /// <summary>
        /// Sort direction
        /// </summary>
        public ListSortDirection SortDirection
        {
            get { return (ListSortDirection) base.GetValue(SortDirectionProperty); }
            set { base.SetValue(SortDirectionProperty, value);}
        }

        /// <summary>
        /// The color of the arrow
        /// </summary>
        public static readonly DependencyProperty ArrowFillProperty = DependencyProperty.Register("ArrowFill", typeof(Brush), typeof(ListViewSortBehavior),
                                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// The color of the arrow
        /// </summary>
        public Brush ArrowFill
        {
            get { return (Brush)GetValue(ArrowFillProperty); }
            set { SetValue(ArrowFillProperty, value); }
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
            AssociatedObject.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnSortHeaderClick));
            AssociatedObject.Loaded += AssociatedObjectLoaded;

            // Ensure it's always called.
            if (AssociatedObject.IsLoaded)
                AssociatedObjectLoaded(AssociatedObject, null);
        }

        /// <summary>
        /// Called when the ListView has completely loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AssociatedObjectLoaded(object sender, RoutedEventArgs e)
        {
            // Bad column index?
            if (InitialColumnIndex < 0) 
                return;
            
            // Must be a GridView to see columns.
            GridView gridView = AssociatedObject.View as GridView;
            if (gridView == null)
                return;

            // Bad column index?
            if (InitialColumnIndex >= gridView.Columns.Count)
                return;

            // Get the logical column descriptor and match that to the visual column.
            GridViewColumn startingColumn = gridView.Columns[InitialColumnIndex];
            _sortingColumn = AssociatedObject.EnumerateVisualTree<GridViewColumnHeader>(gvch => gvch.Column == startingColumn).FirstOrDefault();
            if (_sortingColumn != null)
            {
                SortDirection = (SortDirection == ListSortDirection.Ascending) ? ListSortDirection.Descending : ListSortDirection.Ascending;
                SortByColumn(_sortingColumn);
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
            AssociatedObject.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnSortHeaderClick));
            AssociatedObject.Loaded -= AssociatedObjectLoaded;
        }

        /// <summary>
        /// This is called when a Button.Click event occurs inside
        /// the ListView. Here we filter to the column headers and
        /// then provide the sorting when that happens.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSortHeaderClick(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader cHeader = e.OriginalSource as GridViewColumnHeader;
            if (cHeader != null)
            {
                if (cHeader.Role == GridViewColumnHeaderRole.Normal)
                {
                    SortByColumn(cHeader);
                }
            }
        }

        /// <summary>
        /// This method provides the actual sorting behavior.
        /// </summary>
        /// <param name="sortingColumn"></param>
        private void SortByColumn(GridViewColumnHeader sortingColumn)
        {
            string sortPath = null;
            Binding binding = sortingColumn.Column.DisplayMemberBinding as Binding;
            if (binding != null)
                sortPath = binding.Path.Path;

            // No column binding?
            if (string.IsNullOrEmpty(sortPath))
                return;

            // Pickup either the data bound source, or the ListView collection itself.
            object data = AssociatedObject.ItemsSource ?? AssociatedObject.Items;
            if (data != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(data);
                if (view != null && view.CanSort)
                {
                    if (_adorner != null)
                        AdornerLayer.GetAdornerLayer(_sortingColumn).Remove(_adorner);

                    if (sortingColumn == _sortingColumn)
                    {
                        SortDirection = (SortDirection == ListSortDirection.Ascending)
                                            ? ListSortDirection.Descending
                                            : ListSortDirection.Ascending;
                    }
                    else
                    {
                        _sortingColumn = sortingColumn;
                        SortDirection = ListSortDirection.Ascending;
                    }

                    // Determine the direction and add the arrow.
                    _adorner = new SortAdorner(_sortingColumn) { Fill = ArrowFill ?? _sortingColumn.Foreground, Direction = SortDirection };
                    AdornerLayer.GetAdornerLayer(_sortingColumn).Add(_adorner);
                    view.SortDescriptions.Clear();
                    view.SortDescriptions.Add(new SortDescription(sortPath, SortDirection));
                }
            }
        }
    }
}
