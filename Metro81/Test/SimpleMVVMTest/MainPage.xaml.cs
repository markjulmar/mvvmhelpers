using JulMar.Windows.UI.Notifications.ToastContent;
using SimpleMvvmTest.ViewModels;
using System;
using Windows.UI.Notifications;

namespace SimpleMVVMTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            if (e.Parameter != null)
                ((MainViewModel) DataContext).TextToDisplay = e.Parameter.ToString();
        }

        private void OnShowToast(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var toastManager = ToastNotificationManager.CreateToastNotifier();
            if (toastManager.Setting == NotificationSetting.Enabled)
            {
                var content = ToastContentFactory.CreateToastImageAndText04();
                content.Image.Src = content.FromPackage("Assets/WideLogo.png");
                content.TextHeading.Text = "Toast heading";
                content.TextBody1.Text = "Toast Subheading #1";
                content.TextBody2.Text = "Toast Subheading #1";
                content.Launch = "Hello from the Toast!";

                toastManager.AddToSchedule(new ScheduledToastNotification(content.GetXml(), DateTimeOffset.Now.AddSeconds(5)));
            }
        }
    }
}
