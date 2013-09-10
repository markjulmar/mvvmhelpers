using System;
using System.Collections.Generic;
using System.Linq;
using JulMar.Core.Extensions;
using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;
using NavigationSample.DataModel;
using System.Composition;

namespace NavigationSample.ViewModels
{
    /// <summary>
    /// This ViewModel is used by all the pages - it coordinates the navigation and currently
    /// selected values across each of the views.
    /// </summary>
    [ExportViewModel("MainViewModel"), Shared]
    public class MainViewModel : ViewModel, INavigationAware
    {
        private bool _isReverseSorted = false;
        private readonly SampleDataSource _dataSource;
        private SampleDataItemViewModel _selectedItem;
        private SampleDataGroupViewModel _selectedGroup;

        /// <summary>
        /// All the available groups
        /// </summary>
        public IList<SampleDataGroupViewModel> AllGroups { get { return _dataSource.Groups; } }

        /// <summary>
        /// Display the item details; expects the parameter to be a specific item.
        /// </summary>
        public IDelegateCommand ShowItemDetails { get; private set; }

        /// <summary>
        /// Display the group details; expects the parameter to be a group item
        /// </summary>
        public IDelegateCommand ShowGroupDetails { get; private set; }

        /// <summary>
        /// Sort the groups by name
        /// </summary>
        public IDelegateCommand SortByName { get; private set; }

        /// <summary>
        /// Selected item
        /// </summary>
        public SampleDataItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set { SetPropertyValue(ref _selectedItem, value); }
        }

        /// <summary>
        /// The currently selected group
        /// </summary>
        public SampleDataGroupViewModel SelectedGroup
        {
            get { return _selectedGroup; }
            set { SetPropertyValue(ref _selectedGroup, value); }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public MainViewModel()
        {
            _dataSource = new SampleDataSource();
            
            // Commands
            ShowItemDetails = new DelegateCommand<SampleDataItemViewModel>(OnShowItemDetails, it => it != null);
            ShowGroupDetails = new DelegateCommand<SampleDataGroupViewModel>(OnShowGroupDetails, it => it != null);
            SortByName = new DelegateCommand(OnSortGroupsByName);

            SelectedItem = AllGroups[0].Items[0];
            SelectedGroup = AllGroups[0];
        }

        /// <summary>
        /// Sort the items
        /// </summary>
        private void OnSortGroupsByName()
        {
            _isReverseSorted = !_isReverseSorted;
            _dataSource.Groups.BubbleSort(_isReverseSorted, Comparer<SampleDataGroupViewModel>.Create((g1,g2) => String.Compare(g1.Title, g2.Title, StringComparison.Ordinal)));
        }

        /// <summary>
        /// Display the item details
        /// </summary>
        /// <param name="item"></param>
        private void OnShowItemDetails(SampleDataItemViewModel item)
        {
            SelectedItem = item;
            SelectedGroup = item.Group;
            Resolve<IPageNavigator>().NavigateTo("ItemDetailsPage");
        }

        /// <summary>
        /// Display the group details
        /// </summary>
        /// <param name="group"></param>
        private void OnShowGroupDetails(SampleDataGroupViewModel group)
        {
            SelectedGroup = group;
            SelectedItem = group.Items.FirstOrDefault();
            Resolve<IPageNavigator>().NavigateTo("GroupDetailsPage");
        }

        /// <summary>
        /// Returns the specified group by id.
        /// </summary>
        /// <param name="id">Id to search for</param>
        /// <returns>Matching group or null</returns>
        public SampleDataGroupViewModel GetGroup(string id)
        {
            return AllGroups.FirstOrDefault(group => group.Id.Equals(id));
        }

        /// <summary>
        /// Returns the specified item
        /// </summary>
        /// <param name="id">Id to search for</param>
        /// <returns>Item</returns>
        public SampleDataItemViewModel GetItem(string id)
        {
            return AllGroups.SelectMany(group => group.Items).FirstOrDefault(item => item.Id.Equals(id));
        }

        public void OnNavigatingFrom(NavigatingFromEventArgs e)
        {
            var state = e.State;
            if (state != null)
            {
                if (SelectedItem != null)
                    state["SelectedItem"] = SelectedItem.Id;
                if (SelectedGroup != null)
                    state["SelectedGroup"] = SelectedGroup.Id;
            }
        }

        public void OnNavigatedTo(NavigatedToEventArgs e)
        {
            var state = e.State;
            if (state != null)
            {
                object id;
                if (state.TryGetValue("SelectedItem", out id) && id != null)
                    SelectedItem = GetItem(id.ToString());
                if (state.TryGetValue("SelectedGroup", out id) && id != null)
                    SelectedGroup = GetGroup(id.ToString());
            }
        }
    }
}
