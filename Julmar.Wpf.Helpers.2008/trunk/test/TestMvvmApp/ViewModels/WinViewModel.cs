using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;
using JulMar.Windows.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace TestMvvm.ViewModels
{
    /// <summary>
    /// The ViewModel for the application
    /// </summary>
    [ExportViewModel("MainWindow")]
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

        private string _color;
        public string BackgroundColor
        {
            get { return _color;  }
            set { _color = value; OnPropertyChanged("BackgroundColor"); }
        }

        private bool _showText;
        public bool ShowText
        {
            get { return _showText; }
            set { _showText = value; OnPropertyChanged("ShowText"); }
        }

        public ObservableCollection<Element> Elements { get; private set; }
        #endregion

        #region Commands
        public ICommand ActivatedCommand { get; private set; }
        public ICommand DeactivatedCommand { get; private set; }
        public ICommand LoadedCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public ICommand ExitCommand { get; private set; }
        public ICommand ShowColorDialogCommand { get; private set; }
        public ICommand ShowPropertiesCommand { get; private set; }
        public ICommand MouseEnterLeaveCommand { get; private set; }
        public ICommand ChangeBackground { get; private set; }
        #endregion

        public WinViewModel()
        {
            Title = "MVVM Test";
            BackgroundColor = "White";
            Elements = new ObservableCollection<Element>();

            ActivatedCommand = new DelegatingCommand(() => IsActive = true);
            DeactivatedCommand = new DelegatingCommand(() => IsActive = false);
            LoadedCommand = new DelegatingCommand(OnLoaded);
            CloseCommand = new DelegatingCommand(OnClosed, OnCheckClose);
            ExitCommand = new DelegatingCommand(RaiseCloseRequest);
            ShowColorDialogCommand = new DelegatingCommand<Element>(ShowColorDialog);
            ShowPropertiesCommand = new DelegatingCommand(ShowPropertyDialog);
            MouseEnterLeaveCommand = new DelegatingCommand<EventParameters>(OnMouseEnterLeave);
            ChangeBackground = new DelegatingCommand<string>(OnChangeBackground);
        }

        private void OnLoaded()
        {
            CreateShapes(10);
        }

        private void CreateShapes(int count)
        {
            Elements.Clear();

            Random rnd = new Random();
            for (int i = 0; i < count; i++)
            {
                Elements.Add(new Element { Color = typeof(Colors).GetProperties()[rnd.Next(50)].Name, Width = rnd.Next(100), Height = rnd.Next(100), X = rnd.Next(200), Y = rnd.Next(200) });
            }
        }

        private void ShowColorDialog(Element parameter)
        {
            Resolve<IUIVisualizer>().Show("ColorDialogVisual", new ColorDialogViewModel(parameter), true, null);
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

        private void OnMouseEnterLeave(EventParameters ep)
        {
            if (ep.CommandParameter != null)
            {
                RoutedEventArgs re = (RoutedEventArgs) ep.EventArgs;
                ItemsControl fe = (ItemsControl) ep.CommandParameter;
                Title = re.RoutedEvent == UIElement.MouseEnterEvent ? "MVVM Test (Active)" : "MVVM Test (Inactive)";
            }
        }

        private void OnChangeBackground(string color)
        {
            BackgroundColor = color;    
        }

        private void ShowPropertyDialog()
        {
            var propViewModel = new AppPropertyViewModel {Title = this.Title, ShapeCount = Elements.Count};
            bool? result = Resolve<IUIVisualizer>().ShowDialog("ShowProperties", propViewModel);
            if (result.HasValue && result.Value)
            {
                Title = propViewModel.Title;
                CreateShapes(propViewModel.ShapeCount);
            }
        }
    }
}