using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;
using System.Collections.Generic;

namespace PopupTest.ViewModels
{
    [ExportViewModel("SettingsViewModel")]
    public sealed class SettingsViewModel : SimpleViewModel
    {
        private string _selectedSearchType;
        private int _selectedCommandSearchType;
        public IList<string> ListSearchType { get; private set; }

        public string SelectedSearchType
        {
            get { return _selectedSearchType; }
            set { SetPropertyValue(ref _selectedSearchType, value); }
        }

        public int SelectedCommandSearchTypeCount
        {
            get { return _selectedCommandSearchType; }
            private set { SetPropertyValue(ref _selectedCommandSearchType, value); }
        }

        public IDelegateCommand SearchComboSelectionChangedCommand { get; private set; }

        public SettingsViewModel()
        {
            ListSearchType = new[] {"Inclusive", "Exclusive", "Random"};
            SelectedSearchType = ListSearchType[0];
            SearchComboSelectionChangedCommand = new DelegateCommand<string>(OnSearchTypeChanged);
        }

        private void OnSearchTypeChanged(string item)
        {
            if (item == "PanelSearch")
                SelectedCommandSearchTypeCount++;
        }
    }
}
