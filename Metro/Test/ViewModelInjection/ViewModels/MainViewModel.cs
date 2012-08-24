using System.Diagnostics;
using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;
using System.Composition;

namespace ViewModelInjection.ViewModels
{
    [ExportViewModel("theVM")]
    public class MainViewModel : ViewModel
    {
        private bool _canShowCommand;
        public bool CanShowCommand
        {
            get { return _canShowCommand; }
            set
            {
                if (SetPropertyValue(ref _canShowCommand, value))
                    ShowMessage.RaiseCanExecuteChanged();
            }
        }

        public IDelegateCommand ShowMessage { get; private set; }

        public MainViewModel()
        {
            ShowMessage = new DelegateCommand(OnShowCommand, () => CanShowCommand == true);
        }

        [Import]
        public IMessageVisualizer messageVisualizer { get; set; }

        private void OnShowCommand()
        {
            messageVisualizer.ShowAsync("Test Message", "Test Title");
        }
    }
}
