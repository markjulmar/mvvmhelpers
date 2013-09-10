using JulMar.Windows.Controls;
using JulMar.Windows.Services;
using JulMar.Windows.UI;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace PopupTest
{
    [ExportFlyout("Flyout1", typeof(SettingsPage))]
    public sealed partial class SettingsPage : FlyoutPage
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }
    }
}
