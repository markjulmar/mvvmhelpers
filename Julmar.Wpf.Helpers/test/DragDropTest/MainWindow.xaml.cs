using System.Diagnostics;
using System.Windows;
using JulMar.Windows;

namespace ItemsControlDragDropBehavior.TestApp
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            DataContext = new MainViewModel(1000);
            InitializeComponent();
        }

        private void OnDragInitiated(object sender, DragDropEventArgs e)
        {
            Product p = e.Item as Product;
            Debug.Assert(p != null);
            
            if (p.Quantity > 100)
                e.Cancel = true;

            e.AllowedEffects = DragDropEffects.Move;
        }

        private void OnDropEnter(object sender, DragDropEventArgs e)
        {
            Debug.WriteLine("DropEnter: {0}", e.Item);
        }

        private void OnDropInitiated(object sender, DragDropEventArgs e)
        {
            Debug.WriteLine("DropInitiated: {0}", e.Item);
        }
    }
}
