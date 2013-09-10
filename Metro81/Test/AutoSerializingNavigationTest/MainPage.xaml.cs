using JulMar.Windows.Services;

namespace AutoSerializingNavigationTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [ExportPage("MainPage", typeof(MainPage))]
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
    }
}
