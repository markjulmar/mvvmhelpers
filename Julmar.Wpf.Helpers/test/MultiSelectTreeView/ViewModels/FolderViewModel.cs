using System.Collections.Generic;
using System.Collections.ObjectModel;
using JulMar.Windows.Mvvm;

namespace MultiSelectTreeView.ViewModels
{
    public class FolderViewModel : SimpleViewModel
    {
        public IList<FolderViewModel> Children { get; private set; }
        
        private readonly string _name;
        public string Name
        {
            get { return _name + " (" + GetHashCode() + ")"; }
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; RaisePropertyChanged(() => IsExpanded); }
        }

        public FolderViewModel(string name)
        {
            _name = name;
            Children = new ObservableCollection<FolderViewModel>();
        }
    }
}
