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
using System.Windows.Shapes;
using JulMar.Windows.UI;

namespace MultiUiViewModelTest.Views
{
    /// <summary>
    /// Interaction logic for View1.xaml
    /// </summary>
    [ExportUIVisualizer("Dialog1")]
    [ExportUIVisualizer("Dialog2")]
    public partial class View1 : Window
    {
        public View1()
        {
            InitializeComponent();
        }
    }
}
