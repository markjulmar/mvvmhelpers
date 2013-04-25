using System.Runtime.Serialization;
using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;

namespace AutoSerializingNavigationTest.ViewModels
{
    [DataContract]
    public sealed class ColorViewModel : ViewModel
    {
        [DataMember]
        public string Color { get; set; }

        public IDelegateCommand GoBack { get; private set; }

        public ColorViewModel()
        {
            Initialize(new StreamingContext());
        }

        [OnDeserialized]
        void Initialize(StreamingContext context)
        {
            GoBack = new DelegateCommand(() => Resolve<IPageNavigator>().GoBack(), () => Resolve<IPageNavigator>().CanGoBack);
        }

        public ColorViewModel(string color) : this()
        {
            Color = color;
        }
    }
}