using JulMar.Windows.Mvvm;

namespace MultiSelectCollectionTest.ViewModels
{
    public class FolderViewModel : SimpleViewModel
    {
        private readonly string _name;
        public string Name
        {
            get { return _name + " (" + GetHashCode() + ")"; }
        }

        public FolderViewModel(string name)
        {
            _name = name;
        }
    }
}
