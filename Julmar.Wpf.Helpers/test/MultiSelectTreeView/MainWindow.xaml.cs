using MultiSelectTreeView.ViewModels;

namespace MultiSelectTreeView
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            DataContext = new MainViewModel();
            InitializeComponent();
        }
    }
}
