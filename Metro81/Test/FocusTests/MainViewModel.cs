using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;

namespace FocusTests
{
    [ExportViewModel("MainVM")]
    public sealed class MainViewModel : SimpleViewModel
    {
        private string _text;
        public string Text
        {
            get { return this._text; }
            set { SetPropertyValue(ref _text, value); }
        }

        public event Action AddComplete = delegate { };
        public IDelegateCommand AddTag { get; private set; }
        public IList<string> Tags { get; private set; }

        public MainViewModel()
        {
            AddTag = new DelegateCommand<string>(OnAddTag, s => !string.IsNullOrEmpty(s) && !Tags.Contains(s));
            Tags = new ObservableCollection<string>();
        }

        private void OnAddTag(string newTag)
        {
            Tags.Add(newTag);
            Text = string.Empty;
            AddComplete();
        }
    }
}
