using System;
using Windows.UI.Xaml.Media;
using BouncingBalls.ViewModels;
using Windows.UI.Xaml;

namespace BouncingBalls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
        {
            DataContext = new MainViewModel();
            this.InitializeComponent();
        }

        private void OnContainerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ((MainViewModel)DataContext).Width = e.NewSize.Width;
            ((MainViewModel)DataContext).Height = e.NewSize.Height;
        }
    }
}
