using JulMar.Windows.Mvvm;

namespace TestMvvm.ViewModels
{
    /// <summary>
    /// Simple view model for application properties
    /// </summary>
    public class AppPropertyViewModel : ViewModel
    {
        private string _title;
        private int _count;

        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged("Title"); }
        }

        public int ShapeCount
        {
            get { return _count; }
            set { _count = value; OnPropertyChanged("ShapeCount"); }
        }
    }
}
