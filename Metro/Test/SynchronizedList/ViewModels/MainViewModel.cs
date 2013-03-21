using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace SynchronizedList.ViewModels
{
    public sealed class MainViewModel : INotifyPropertyChanged
    {
        private readonly IList<Tuple<ListViewSelectionMode, string>> _lvsmValues;
        private readonly IList<Tuple<SelectionMode, string>> _lbsmValues;
        
        private Tuple<ListViewSelectionMode, string> _selectedListViewMode;
        private Tuple<ListViewSelectionMode, string> _selectedGridViewMode;
        private Tuple<SelectionMode, string> _selectedListBoxMode;

        public IReadOnlyList<string> Names { get; private set; }
        public IList<string> SelectedNames { get; private set; }

        public IEnumerable<Tuple<ListViewSelectionMode,string>> ListViewSelectionModes
        {
            get { return _lvsmValues; }
        }

        public Tuple<ListViewSelectionMode, string> SelectedListViewMode
        {
            get { return _selectedListViewMode; }
            set
            {
                _selectedListViewMode = value; 
                OnPropertyChanged("SelectedListViewMode");
            }
        }

        public IEnumerable<Tuple<SelectionMode, string>> ListBoxSelectionModes
        {
            get { return _lbsmValues; }
        }

        public Tuple<SelectionMode, string> SelectedListBoxMode
        {
            get { return _selectedListBoxMode; }
            set
            {
                _selectedListBoxMode = value;
                OnPropertyChanged("SelectedListBoxMode");
            }
        }

        public Tuple<ListViewSelectionMode, string> SelectedGridViewMode
        {
            get { return _selectedGridViewMode; }
            set
            {
                _selectedGridViewMode = value;
                OnPropertyChanged("SelectedGridViewMode");
            }
        }

        public MainViewModel()
        {
            Names = new List<string>
                        {
                            "Alice", "Bob", "Carol", "David", "Edgar", "Frank",
                            "Georgia", "Hank", "India", "Jack", "Karen", "Larry",
                            "Mike", "Nate", "Oscar", "Peter", "Quix", "Russ", "Steve",
                            "Tonya", "Uma", "Violet", "Walter", "Xi", "Yvonne", "Zed"
                        };
            SelectedNames = new ObservableCollection<string>();

            _lvsmValues = Enum.GetValues(typeof (ListViewSelectionMode))
                .Cast<ListViewSelectionMode>()
                .Select(lvm => Tuple.Create(lvm, Enum.GetName(typeof (ListViewSelectionMode), lvm)))
                .ToList();

            _lbsmValues = Enum.GetValues(typeof(SelectionMode))
                .Cast<SelectionMode>()
                .Select(lvm => Tuple.Create(lvm, Enum.GetName(typeof(SelectionMode), lvm)))
                .ToList();

            SelectedListViewMode = _lvsmValues.First();
            SelectedListBoxMode = _lbsmValues.First();
            SelectedGridViewMode = _lvsmValues.Last();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
