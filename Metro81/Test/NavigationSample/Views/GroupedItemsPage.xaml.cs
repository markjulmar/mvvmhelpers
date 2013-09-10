using JulMar.Windows.Interfaces;
using JulMar.Windows.Services;

namespace NavigationSample.Views
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    [ExportPage("GroupedItems", typeof(GroupedItemsPage))]
    public sealed partial class GroupedItemsPage : INavigationAware
    {
        public GroupedItemsPage()
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
