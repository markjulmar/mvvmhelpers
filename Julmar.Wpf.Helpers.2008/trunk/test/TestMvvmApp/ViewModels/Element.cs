using System.ComponentModel;

namespace TestMvvm.ViewModels
{
    /// <summary>
    /// This object is a simple data holder which is placed into the items control.
    /// </summary>
    public class Element : INotifyPropertyChanged
    {
        private string _color;
        public string Color
        {
            get { return _color; }
            set { _color = value; OnPropertyChanged("Color"); }
        }

        public double X { get; set; }
        public double Y { get; set; }

        private double _width;
        public double Width
        {
            get { return _width; }
            set { _width = value; OnPropertyChanged("Width"); }
        }

        private double _height;
        public double Height
        {
            get { return _height; }
            set { _height = value; OnPropertyChanged("Height"); }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private void OnPropertyChanged(string propName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        #endregion
    }
}