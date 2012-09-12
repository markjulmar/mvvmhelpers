using JulMar.Windows.Services;

namespace NavigationSample.Views
{
    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    [ExportPage("ItemDetailsPage", typeof(ItemDetailPage))]
    public sealed partial class ItemDetailPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
        }
    }
}
