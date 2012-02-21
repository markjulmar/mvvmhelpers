using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using JulMar.Windows.Mvvm;
using System.Windows.Input;
using JulMar.Windows.Interfaces;

namespace MultiUiViewModelTest.ViewModels
{
    [ExportViewModel("MainViewModel")]
    [ExportViewModel("SecondMainViewModel")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MainViewModel : ViewModel
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value; OnPropertyChanged(() => Text); }
        }

        public ICommand ShowDialog { get; private set; }
        public ICommand DuplicateMainWindow { get; private set; }

        public MainViewModel()
        {
            Text = "Some Text Here..";
            ShowDialog = new DelegatingCommand<string>(OnShowDialog);
            DuplicateMainWindow = new DelegatingCommand(OnDuplicateMainWindow);
        }

        void OnShowDialog(string dialogKey)
        {
            Resolve<IUIVisualizer>().ShowDialog(dialogKey, this);
        }

        void OnDuplicateMainWindow()
        {
            Resolve<IUIVisualizer>().Show("MainWindow", true, null);
        }
    }
}
