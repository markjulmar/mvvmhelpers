using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using JulMar.Core.Services;
using JulMar.Windows.Interfaces;
using JulMar.Core;
using JulMar.Windows.Mvvm;
using JulMar.Core.Interfaces;

namespace MultiWindowTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Get the IUIVisualizer from MEF
        /// </summary>
        [Import]
        private IUIVisualizer _uiVisualizer = null;

        public MainWindow()
        {
            Loaded += MainWindowLoaded;
            InitializeComponent();
        }

        void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            string ownerText = "None";
            if (Owner != null)
                ownerText = string.Format("Window #{0}", Application.Current.Windows.Cast<Window>().ToList().IndexOf(Owner) + 1);

            this.Title = string.Format("Window #: {0} - Owner: {1}", Application.Current.Windows.Count, ownerText);
            ServiceLocator.Instance.Resolve<IMessageMediator>().Register(this);

            DynamicComposer.Instance.Compose(this);
            _uiVisualizer.Register("MainWindow", typeof(MainWindow));
        }

        [MessageMediatorTarget]
        private void OnShowView(ShowViewMessage viewData)
        {
            // Only respond to our own ViewModel)
            if (viewData.ViewModel == this.Title)
            {
                _uiVisualizer.Show(viewData.ViewKey, viewData.DataContext, (viewData.SetOwner) ? this : null,
                                   viewData.ViewClosed);
            }
        }

        private void OnCreateNewModalWindow(object sender, RoutedEventArgs e)
        {
            _uiVisualizer.ShowDialog("MainWindow");
        }

        private void OnCreateNewModalessWindow(object sender, RoutedEventArgs e)
        {
            this.Background = Brushes.Red;
            ServiceLocator.Instance.Resolve<IMessageMediator>().SendMessage(
                new ShowViewMessage
                    {
                        SetOwner = false,
                        ViewModel = Title,
                        ViewClosed = (s,ev) => ClearValue(BackgroundProperty),
                        ViewKey = "MainWindow",
                    });
        }

        private void OnCreateNewModalChildWindow(object sender, RoutedEventArgs e)
        {
            _uiVisualizer.ShowDialog("MainWindow", null, this);
        }

        private void OnCreateNewModalessChildWindow(object sender, RoutedEventArgs e)
        {
            this.Background = Brushes.Green;
            ServiceLocator.Instance.Resolve<IMessageMediator>().SendMessage(
                new ShowViewMessage
                {
                    SetOwner = true,
                    ViewModel = Title,
                    ViewClosed = (s, ev) => ClearValue(BackgroundProperty),
                    ViewKey = "MainWindow",
                });
        }
    }

    /// <summary>
    /// Message used to initiate a new UI from VM to View
    /// </summary>
    public class ShowViewMessage
    {
        /// <summary>
        /// ViewModel passing this message - used to determine proper view owner
        /// </summary>
        public string ViewModel { get; set; }

        /// <summary>
        /// View we want to open
        /// </summary>
        public string ViewKey { get; set; }

        /// <summary>
        /// DataContext to use for new view
        /// </summary>
        public object DataContext { get; set; }

        /// <summary>
        /// True to set owner on new modaless view
        /// </summary>
        public bool SetOwner { get; set; }

        /// <summary>
        /// Callback to notify ViewModel that view has closed.
        /// </summary>
        public EventHandler<UICompletedEventArgs> ViewClosed { get; set; }
    }
}
