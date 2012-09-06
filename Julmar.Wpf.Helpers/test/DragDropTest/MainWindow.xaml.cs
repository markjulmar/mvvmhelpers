using System.Windows;
using JulMar.Windows;
using System.Windows.Input;

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

        private void OnListViewDropEnter(object sender, DragDropEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) 
                || Keyboard.IsKeyDown(Key.RightCtrl))
                e.AllowedEffects = DragDropEffects.Copy;
            else
                e.AllowedEffects = DragDropEffects.None;
        }
    }
}
