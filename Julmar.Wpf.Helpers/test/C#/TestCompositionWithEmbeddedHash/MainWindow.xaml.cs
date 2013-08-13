using JulMar.Core.Services;
using JulMar.Windows.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestCompositionWithEmbeddedHash
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            IUIVisualizer uiVisualizer = ServiceLocator.Instance.Resolve<IUIVisualizer>();
            txt.Text = (uiVisualizer == null) ? "Failed" : "Success!";
        }
    }
}
