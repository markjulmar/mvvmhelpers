using JulMar.Windows.Mvvm;

namespace NavigationSample.ViewModels
{
    public abstract class BaseViewModel : ViewModel
    {
        protected BaseViewModel(string id, string title, string subtitle, string imagePath, string description)
        {
            _id = id;
            _title = title;
            _subtitle = subtitle;
            _description = description;
            _imagePath = imagePath;
        }

        private string _id = string.Empty;
        public string Id
        {
            get { return _id; }
            set { SetPropertyValue(ref _id, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return _title; }
            set { SetPropertyValue(ref _title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return _subtitle; }
            set { SetPropertyValue(ref _subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return _description; }
            set { SetPropertyValue(ref _description, value); }
        }

        private string _imagePath;
        public string Image
        {
            get { return _imagePath; }
            set { SetPropertyValue(ref _imagePath, value); }
        }

        public override string ToString()
        {
            return Title;
        }
    }
}