using JulMar.Windows.Interfaces;
using JulMar.Windows.Services;

namespace AutoSerializingNavigationTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [ExportPage("OneColorView", typeof(OneColorPage))]
    public sealed partial class OneColorPage : INavigationAware
    {
        public OneColorPage()
        {
            this.InitializeComponent();
        }

        public void OnNavigatingFrom(NavigatingFromEventArgs e)
        {
            e.State["aTextBox"] = aTextBox.Text;
        }

        public void OnNavigatedTo(NavigatedToEventArgs e)
        {
            if (e.State != null && e.State.ContainsKey("aTextBox"))
                aTextBox.Text = (string) e.State["aTextBox"];
        }
    }
}
