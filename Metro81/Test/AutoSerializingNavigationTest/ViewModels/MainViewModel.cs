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
    /// <summary>
    /// ViewModel decorated with DataContract - so we need to be explicit
    /// about which properties are serialized through [DataMember].
    /// Also, constructor is not called in this case - so we must use
    /// [OnDeserializing].
    /// </summary>
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
