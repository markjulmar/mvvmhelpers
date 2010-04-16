using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;
using System.ComponentModel;

namespace TestMvvm
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

    /// <summary>
    /// The ViewModel for the application
    /// </summary>
    public class WinViewModel : ViewModel
    {
        #region Data
        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; OnPropertyChanged("IsActive");}
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged("Title"); }
        }

        public ObservableCollection<Element> Elements { get; private set; }
        #endregion

        #region Commands
        public ICommand ActivatedCommand { get; private set; }
        public ICommand DeactivatedCommand { get; private set; }
        public ICommand LoadedCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public ICommand MouseEnterCommand { get; private set; }
        public ICommand MouseLeaveCommand { get; private set; }
        public ICommand ExitCommand { get; private set; }
        public ICommand ShowColorDialogCommand { get; private set; }
        #endregion

        public WinViewModel()
        {
            Title = "Window driven by WinViewModel";
            Elements = new ObservableCollection<Element>();

            ActivatedCommand = new DelegatingCommand(() => IsActive = true);
            DeactivatedCommand = new DelegatingCommand(() => IsActive = false);
            LoadedCommand = new DelegatingCommand(OnLoaded);
            CloseCommand = new DelegatingCommand(OnClosed, OnCheckClose);
            MouseEnterCommand = new DelegatingCommand(e => Title += " (Mouse Enter)");
            MouseLeaveCommand = new DelegatingCommand(e => Title = Title.Substring(0,14));
            ExitCommand = new DelegatingCommand(RaiseCloseRequest);
            ShowColorDialogCommand = new DelegatingCommand(ShowColorDialog);
        }

        private void OnLoaded()
        {
            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                Elements.Add(new Element
                {
                    Color = typeof(Colors).GetProperties()[rnd.Next(50)].Name,
                    Width = rnd.Next(100),
                    Height = rnd.Next(100),
                    X = rnd.Next(200),
                    Y = rnd.Next(200)
                });
            }
        }

        private void ShowColorDialog(object parameter)
        {
            IUIVisualizer vis = Resolve<IUIVisualizer>();
            if (vis != null)
            {
                var cdvm = new ColorDialogViewModel((Element)parameter);
                vis.Show(cdvm.ToString(), cdvm, true, null);
            }
        }

        private void OnClosed()
        {
            Resolve<IErrorVisualizer>().Show("Window was closed", "The window has been closed.");
        }

        private bool OnCheckClose()
        {
            return (Resolve<IMessageVisualizer>().Show("Question", "Do you want to close this window?",
                                                    MessageButtons.YesNo) == MessageResult.Yes);
        }

    }
}
