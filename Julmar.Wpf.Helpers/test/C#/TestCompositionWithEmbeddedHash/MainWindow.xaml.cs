using JulMar.Core.Services;
using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
        }
    }

    [ExportViewModel("Test")]
    public sealed class MyViewModel : ViewModel
    {
        [Import]
        private IUIVisualizer _uiVisualizer = null;

        private string _text;

        public string Text
        {
            get { return _text; }
            set { SetPropertyValue(ref _text, value, () => Text); }
        }

        public MyViewModel()
        {
            StringBuilder sb = new StringBuilder();

            if (_uiVisualizer != null)
                sb.AppendLine("Found [Import] IUIVisualizer");
            else
                sb.AppendLine("Missing [Import] IUIVisualizer");

            IUIVisualizer vis = ServiceLocator.Instance.Resolve<IUIVisualizer>();
            if (vis != null)
                sb.AppendLine("Found ServiceLocator.Resolve IUIVisualizer");
            else
                sb.AppendLine("Missing ServiceLocator.Resolve IUIVisualizer");

            if (!ReferenceEquals(vis, _uiVisualizer))
            {
                sb.AppendLine("Resolved version is different than imported version!");
            }
            else
            {
                sb.AppendLine("Found same reference.");
            }
                 
            Text = sb.ToString();
        }
    }
}
