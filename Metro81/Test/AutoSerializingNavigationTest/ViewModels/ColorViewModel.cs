using System.Runtime.Serialization;
using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;

namespace AutoSerializingNavigationTest.ViewModels
{
    /// <summary>
    /// ViewModel not decorated with [DataContract], will still be
    /// serializable, but will automatically take every public property.
    /// So we ignore the ones we don't want to serialize in this case.
    /// </summary>
    public sealed class ColorViewModel : ViewModel, INavigationAware
    {
        private bool _isChecked;

        public string Color { get; set; }

        public bool IsChecked
        {
            get { return _isChecked; }
            set { SetPropertyValue(ref _isChecked, value); }
        }

        [IgnoreDataMember]
        public IDelegateCommand GoBack { get; private set; }

        public ColorViewModel()
        {
            GoBack = new DelegateCommand(() => Resolve<IPageNavigator>().GoBack(), () => Resolve<IPageNavigator>().CanGoBack);
        }

        public ColorViewModel(string color) : this()
        {
            Color = color;
        }

        public void OnNavigatingFrom(NavigatingFromEventArgs e)
        {
            e.State["IsChecked"] = IsChecked;
        }

        public void OnNavigatedTo(NavigatedToEventArgs e)
        {
            if (e.State != null && e.State.ContainsKey("IsChecked"))
                IsChecked = (bool) e.State["IsChecked"];
        }
    }
}