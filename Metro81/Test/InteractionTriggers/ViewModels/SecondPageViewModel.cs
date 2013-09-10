using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;

namespace TestApp.ViewModels
{
    [ExportViewModel("Page2Vm")]
    public sealed class SecondPageViewModel : ViewModel
    {
        private string _viewState;
        public IDelegateCommand SomeCommand { get; private set; }

        public SecondPageViewModel()
        {
            SomeCommand = new DelegateCommand(OnChange);
            _viewState = "state0";
        }

        public string ViewState
        {
            get { return _viewState; }
            set { SetPropertyValue(ref _viewState, value); }
        }

        private void OnChange(object obj)
        {
            switch (ViewState)
            {
                case "state0":
                    ViewState = "state1";
                    break;
                case "state1":
                    ViewState = "state2";
                    break;
                case "state2":
                    ViewState = "state0";
                    break;
            }
        }
    }
}
