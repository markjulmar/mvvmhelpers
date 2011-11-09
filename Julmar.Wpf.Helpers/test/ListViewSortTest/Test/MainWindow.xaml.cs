using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Expression.Blend.SampleData.SampleDataSource;
using JulMar.Core.Extensions;

namespace Test
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			this.InitializeComponent();

			// Insert code required on object creation below this point.
		}

        private void OnPerformManualSort(object sender, JulMar.Windows.Interactivity.SortHeaderEventArgs e)
        {
            SampleDataSource ds = (SampleDataSource) FindResource("SampleDataSource");

            // Clear any applied sort
            ICollectionView view = CollectionViewSource.GetDefaultView(ds.Collection);
            if (view != null && view.CanSort)
                view.SortDescriptions.Clear();

            // Do a manual sort
            if (e.Column.Header.ToString() == "Property #1")
            {
                ds.Collection.BubbleSort((i1, i2) => i1.Property1.CompareTo(i2.Property1),
                                         e.SortDirection == ListSortDirection.Descending);
            }
            else if (e.Column.Header.ToString() == "Property #2")
            {
                ds.Collection.BubbleSort((i1, i2) => i1.Property2.CompareTo(i2.Property2),
                                         e.SortDirection == ListSortDirection.Descending);
            }
            else
                e.Canceled = true;
        }
	}
}