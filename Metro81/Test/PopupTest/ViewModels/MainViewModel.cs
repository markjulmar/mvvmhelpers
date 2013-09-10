using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;

namespace PopupTest.ViewModels
{
    [ExportViewModel("MainVm")]
    public sealed class MainViewModel : ViewModel
    {
        private bool _useAdvancedMode;
        private string _background;
        public IDelegateCommand ShowFlyout { get; private set; }

        public bool UseAdvancedMode
        {
            get { return _useAdvancedMode; }
            set { SetPropertyValue(ref _useAdvancedMode, value); }
        }

        public string Background
        {
            get { return _background; }
            set { SetPropertyValue(ref _background, value); }
        }

        public MainViewModel()
        {
            ShowFlyout = new DelegateCommand(OnShowFlyout);
            Background = "DarkBlue";
        }

        void OnShowFlyout()
        {
            IFlyoutVisualizer flyoutVisualizer = Resolve<IFlyoutVisualizer>();

            if (UseAdvancedMode)
            {
                var newVM = new SettingsViewModel();
                newVM.SelectedSearchType = newVM.ListSearchType[1];
                flyoutVisualizer.Show("Flyout1", newVM, OnOpen, OnClosed);
            }
            else
                flyoutVisualizer.Show("Flyout1");
        }

        private void OnClosed()
        {
            Background = "DarkBlue";
            UseAdvancedMode = false;
        }

        private void OnOpen()
        {
            Background = "Yellow";
        }
    }
}
