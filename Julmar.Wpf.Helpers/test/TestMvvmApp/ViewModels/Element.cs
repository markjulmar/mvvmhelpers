using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;
using JulMar.Windows.Undo;

namespace TestMvvm.ViewModels
{
    /// <summary>
    /// This object is a simple data holder which is placed into the items control.
    /// </summary>
    public class Element : ViewModel
    {
        private string _color;
        public string Color
        {
            get { return _color; }
            set
            {
                if (!string.IsNullOrEmpty(_color) && _color != value)
                    Resolve<IUndoService>().Add(new PropertyChangeUndo(this, "Color", _color, value));
                _color = value;
                OnPropertyChanged("Color");
            }
        }

        private double? _x;
        public double X
        {
            get
            {
                return _x != null ? _x.Value : 0;
            }
            set
            {
                if (_x != null && _x.Value != value)
                    Resolve<IUndoService>().Add(new PropertyChangeUndo(this, "X", _x.Value, value));
                _x = value;
                OnPropertyChanged("X");
            }
        }

        private double? _y;
        public double Y
        {
            get
            {
                return _y != null ? _y.Value : 0;
            }
            set
            {
                if (_y != null && _y.Value != value)
                    Resolve<IUndoService>().Add(new PropertyChangeUndo(this, "Y", _y.Value, value));
                _y = value;
                OnPropertyChanged("Y");
            }
        }

        private double? _width;
        public double Width
        {
            get
            {
                return _width != null ? _width.Value : 0;
            }
            set
            {
                if (_width != null && _width.Value != value)
                    Resolve<IUndoService>().Add(new PropertyChangeUndo(this, "Width", _width.Value, value));
                _width = value;
                OnPropertyChanged("Width");
            }
        }

        private double? _height;
        public double Height
        {
            get
            {
                return _height != null ? _height.Value : 0;
            }
            set
            {
                if (_height != null && _height.Value != value)
                    Resolve<IUndoService>().Add(new PropertyChangeUndo(this, "Height", _height.Value, value));
                _height = value;
                OnPropertyChanged("Height");
            }
        }
    }
}