using System.Windows;

namespace ViewModelTriggerTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var vm = new TestViewModel();
            DataContext = vm;

            // Simple test to ensure VM works.
            //vm.ChangeColor += o =>
            //{
            //    RootLayout.Background = (Brush) o;
            //};

            InitializeComponent();
        }
    }
}
