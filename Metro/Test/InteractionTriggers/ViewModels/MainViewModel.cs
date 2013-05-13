using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;
using System.Collections.Generic;
using System.Linq;

namespace TestApp.ViewModels
{
    [ExportViewModel("MainVm")]
    public sealed class MainViewModel : ViewModel
    {
        private bool _searchCostCenterPopUpIsOpen;
        private string _selectedSearchByForCostCenter;
        private string _searchKeywordForCostCenter;
        private string _selectedSearchCostCenter;
        private int _counter;

        public int Counter
        {
            get { return _counter; }
            set { SetPropertyValue(ref _counter, value); }
        }

        public bool SearchCostCenterPopUpIsOpen
        {
            get { return _searchCostCenterPopUpIsOpen; }
            set { SetPropertyValue(ref _searchCostCenterPopUpIsOpen, value); }
        }

        public IList<string> ListSearchByForCostCenter { get; private set; }
        public string SelectedSearchByForCostCenter
        {
            get { return _selectedSearchByForCostCenter; }
            set { SetPropertyValue(ref _selectedSearchByForCostCenter, value); }
        }

        public string SearchKeywordForCostCenter
        {
            get { return _searchKeywordForCostCenter; }
            set { SetPropertyValue(ref _searchKeywordForCostCenter, value); }
        }

        public IDelegateCommand SearchCommand { get; private set; }
        public IDelegateCommand CancelCommand { get; private set; }
        public IDelegateCommand SelectionChangedCommand { get; private set; }
        public IDelegateCommand GotoPage2 { get; private set; }

        public IList<string> ListSearchCostCenter { get; private set; }
        public string SelectedSearchCostCenter
        {
            get { return _selectedSearchCostCenter; }
            set { SetPropertyValue(ref _selectedSearchCostCenter, value); }
        }

        public MainViewModel()
        {
            ListSearchByForCostCenter = new List<string>
                {
                    "Nearest", "Furthest", "Across The Street", "Around the Way"
                };
            SelectedSearchByForCostCenter = ListSearchByForCostCenter[0];

            ListSearchCostCenter = new List<string>(Enumerable.Range(0, 26).Select(n => ('A' + n).ToString()));
            SelectedSearchCostCenter = ListSearchCostCenter[0];

            SearchCommand = new DelegateCommand(() => { });
            CancelCommand = new DelegateCommand(() => SearchCostCenterPopUpIsOpen = false);
            SelectionChangedCommand = new DelegateCommand(OnSelectedChanged);
            GotoPage2 = new DelegateCommand(OnGotoPage2);
        }

        private void OnGotoPage2(object obj)
        {
            IPageNavigator pageNavigator = Resolve<IPageNavigator>();
            pageNavigator.NavigateTo("Page2");
        }

        private void OnSelectedChanged()
        {
            Counter++;
        }
    }
}
