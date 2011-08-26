using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JulMar.Windows.Mvvm;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainViewModel();
            InitializeComponent();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ((MainViewModel) DataContext).Data.Add("Codebehind.OnTextChanged");
        }
    }

    public class MainViewModel : SimpleViewModel
    {
        public IList<string> Data { get; private set; }
        public ICommand ShowText { get; private set; }

        public MainViewModel()
        {
            Data = new ObservableCollection<string>();
            ShowText = new DelegatingCommand(() => Data.Add("MainViewModel.ShowText"));
        }

        
    }
}
