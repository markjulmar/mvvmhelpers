using JulMar.Core.Interfaces;
using JulMar.Core.Services;
using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;
using System.Composition;
using System.Threading.Tasks;
using JulMar.Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ViewModelInjection
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            // Can do it in code.
            DataContext = ServiceLocator.Instance.Resolve<IViewModelLocator>().Locate("theVM");

            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MyMessageVisualizer visualizer = ServiceLocator.Instance.Resolve<IMessageVisualizer>() as MyMessageVisualizer;
            if (visualizer != null)
                visualizer.Page = this;
        }

        public async void ShowMessageAsync(string message, string title)
        {
            MessageBox.Visibility = Visibility.Visible;
            Title.Text = title;
            Message.Text = message;

            await Task.Delay(3000);

            MessageBox.Visibility = Visibility.Collapsed;
        }
    }

    [Export(typeof(IMessageVisualizer)), Shared]
    public class MyMessageVisualizer : IMessageVisualizer
    {
        public MainPage Page { get; set; }

        public System.Threading.Tasks.Task<Windows.UI.Popups.IUICommand> ShowAsync(string message, string title = "")
        {
            return ShowAsync(message, title, null);
        }

        public System.Threading.Tasks.Task<Windows.UI.Popups.IUICommand> ShowAsync(string message, string title, MessageVisualizerOptions visualizerOptions)
        {
            Page.ShowMessageAsync(message,title);
            return null;
        }
    }

}
