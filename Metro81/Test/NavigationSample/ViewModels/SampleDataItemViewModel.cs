namespace NavigationSample.ViewModels
{
    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItemViewModel : BaseViewModel
    {
        public SampleDataItemViewModel(string id, string title, string subtitle, string imagePath, string description, string content, SampleDataGroupViewModel group)
            : base(id, title, subtitle, imagePath, description)
        {
            _content = content;
            _group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return _content; }
            set { SetPropertyValue(ref _content, value); }
        }

        private SampleDataGroupViewModel _group;
        public SampleDataGroupViewModel Group
        {
            get { return _group; }
            set { SetPropertyValue(ref _group, value); }
        }
    }
}