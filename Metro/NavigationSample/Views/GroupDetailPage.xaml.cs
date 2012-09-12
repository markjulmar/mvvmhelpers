using JulMar.Windows.Services;

namespace NavigationSample.Views
{
    /// <summary>
    /// A page that displays an overview of a single group, including a preview of the items
    /// within the group.
    /// </summary>
    [ExportPage("GroupDetailsPage", typeof(GroupDetailPage))]
    public sealed partial class GroupDetailPage
    {
        public GroupDetailPage()
        {
            InitializeComponent();
        }
    }
}
