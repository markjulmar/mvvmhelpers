using JulMar.Windows.Services;

namespace NavigationSample.Views
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    [ExportPage("GroupedItems", typeof(GroupedItemsPage))]
    public sealed partial class GroupedItemsPage
    {
        public GroupedItemsPage()
        {
            InitializeComponent();
        }
    }
}
