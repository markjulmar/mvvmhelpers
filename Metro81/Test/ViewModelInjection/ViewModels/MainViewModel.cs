using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;

namespace ViewModelInjection.ViewModels
{
    [ExportViewModel("theVM")]
    public class MainViewModel : ViewModel
    {
        private bool _canShowCommand;
        private string _helloMessage;

        public bool CanShowCommand
        {
            get { return _canShowCommand; }
            set
            {
                if (SetPropertyValue(ref _canShowCommand, value))
                    ShowMessage.RaiseCanExecuteChanged();
            }
        }

        public string HelloMessage
        {
            get { return _helloMessage; }
            set { SetPropertyValue(ref _helloMessage, value); }
        }

        public IDelegateCommand ShowMessage { get; private set; }

        public MainViewModel()
        {
            HelloMessage = "Hello from MVVMHelpers";
            ShowMessage = new DelegateCommand(OnShowCommand, () => CanShowCommand == true);
            CanShowCommand = true;
        }

        private void OnShowCommand()
        {
            Resolve<IMessageVisualizer>().ShowAsync("Test Message", "Test Title");
        }
    }
}
