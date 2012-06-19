using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using JulMar.Windows.Mvvm;

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

    public class TestViewModel : ViewModel
    {
        private Brush _color;

        public event Action<object> ChangeColor;

        public ICommand RunAction { get; set; }

        public TestViewModel()
        {
            _color = Brushes.Red;
            RunAction = new DelegateCommand(DoRunAction);
        }

        private void DoRunAction()
        {
            _color = _color == Brushes.Red ? Brushes.Blue : Brushes.Red;

            if (ChangeColor != null)
                ChangeColor(_color);
        }
    }
}
