using JulMar.Windows.Interfaces;
using JulMar.Windows.Services;

namespace NavigationSample.Views
{
    /// <summary>
    /// A page that displays an overview of a single group, including a preview of the items
    /// within the group.
    /// </summary>
    [ExportPage("GroupDetailsPage", typeof(GroupDetailPage))]
    public sealed partial class GroupDetailPage : INavigationAware
    {
        public GroupDetailPage()
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
