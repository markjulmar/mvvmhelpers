using System.Collections.Generic;
using System.Collections.ObjectModel;
using JulMar.Core.Collections;

namespace NavigationSample.ViewModels
{
    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroupViewModel : BaseViewModel
    {
        public IList<SampleDataItemViewModel> Items { get; private set; }
        public IList<SampleDataItemViewModel> TopItems { get; private set; }

        public SampleDataGroupViewModel(string id, string title, string subtitle, string imagePath, string description)
            : base(id, title, subtitle, imagePath, description)
        {
            var items = new ObservableCollection<SampleDataItemViewModel>();
            Items = items;
            TopItems = new CollectionSubset<SampleDataItemViewModel>(items, 10);
        }
    }
}