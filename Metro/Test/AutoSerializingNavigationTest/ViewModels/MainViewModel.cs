using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;
using Windows.UI;
using System.Composition;
using Windows.UI.Xaml.Controls;
using System.Runtime.Serialization;

namespace AutoSerializingNavigationTest.ViewModels
{
    [DataContract]
    public sealed class MainViewModel : ViewModel
    {
        [Import]
        public IPageNavigator PageNavigator { get; set; }
        public IList<ColorViewModel> Colors { get; private set; }
        public IDelegateCommand SelectSpecificColor { get; private set; }

        public MainViewModel()
        {
            Initialize(new StreamingContext());
        }

        [OnDeserializing]
        void Initialize(StreamingContext context)
        {
            SelectSpecificColor = new DelegateCommand<ItemClickEventArgs>(OnSelectColor);
            Colors = new ObservableCollection<ColorViewModel>(
                typeof(Colors).GetTypeInfo().DeclaredProperties.Select(pn => new ColorViewModel(pn.Name)));
        }

        private void OnSelectColor(ItemClickEventArgs e)
        {
            PageNavigator.NavigateTo("OneColorView", e.ClickedItem);
        }
    }
}
