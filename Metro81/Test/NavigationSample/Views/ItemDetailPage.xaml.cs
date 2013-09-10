using JulMar.Windows.Interfaces;
using JulMar.Windows.Services;

namespace NavigationSample.Views
{
    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    [ExportPage("ItemDetailsPage", typeof(ItemDetailPage))]
    public sealed partial class ItemDetailPage : INavigationAware
    {
        public ItemDetailPage()
        {
            InitializeComponent();
        }

        public void OnNavigatingFrom(NavigatingFromEventArgs e)
        {
        }

        public void OnNavigatedTo(NavigatedToEventArgs e)
        {
        }
    }
}
