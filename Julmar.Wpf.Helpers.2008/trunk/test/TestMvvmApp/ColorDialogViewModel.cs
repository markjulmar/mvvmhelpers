using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using JulMar.Windows.Mvvm;

namespace TestMvvm
{
    public class ColorDialogViewModel : ViewModel
    {
        private readonly Element _element;

        public ICommand ViewLoadedCommand { get; private set; }

        private IList<string> _colorList;
        public IList<string> Colors
        {
            get { return _colorList; }
            private set 
            { 
                _colorList = value;
                OnPropertyChanged("Colors"); 
            }
        }

        public string Title { get { return "Shape Properties Dialog";  } }

        public string SelectedColor
        {
            get { return _element.Color; }
            set { _element.Color = value; OnPropertyChanged("SelectedColor");}
        }

        public double Width
        {
            get { return _element.Width; }   
            set { _element.Width = value; OnPropertyChanged("Width");}
        }

        public double Height
        {
            get { return _element.Height; }
            set { _element.Height = value; OnPropertyChanged("Height"); }
        }

        public ColorDialogViewModel(Element element)
        {
            _element = element;
            ViewLoadedCommand = new DelegatingCommand(OnLoad);

            Colors = (from color in typeof(System.Windows.Media.Colors).GetProperties()
                      select color.Name).ToList();
        }

        private void OnLoad()
        {
            Debug.WriteLine("ColorDialogViewModel.OnLoad");
        }
    }
}
