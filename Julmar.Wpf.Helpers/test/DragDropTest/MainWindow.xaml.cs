using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

        private void OnDragInitiated(object sender, DragDropEventArgs e)
        {
            //if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
            //{
            //    e.Cancel = true;
            //    return;
            //}

            //e.AllowedEffects = DragDropEffects.Move;
        }

        private void OnDropEnter(object sender, DragDropEventArgs e)
        {
            //if (e.Source == e.Destination)
            //    e.AllowedEffects = DragDropEffects.None;
            //else
            //{
            //    e.AllowedEffects = DragDropEffects.Move;
            //}
        }

        private void OnDropInitiated(object sender, DragDropEventArgs e)
        {
            Debug.WriteLine("DropInitiated: {0}", e.Item);
        }

        private void OnMultiSelectDropInitiated(object sender, DragDropEventArgs e)
        {
            // Dropping on self?
            if (e.Source == e.Destination)
            {
                ListBox lb = (ListBox)e.Source;
                if (lb.SelectedItems.Count > 1)
                {
                    e.Cancel = true;
                    ((MainViewModel) DataContext).RepositionItems(lb.SelectedItems.Cast<Product>().ToList(), e.DropIndex);
                }
            }
        }
    }
}
