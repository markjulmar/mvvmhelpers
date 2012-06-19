using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using JulMar.Core.Undo;
using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;
using JulMar.Windows.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using JulMar.Core.Interfaces;
using JulMar.Windows.UI;
using TestMvvm.Services;

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
        private string _title;
        private string _color;
        private bool _showText;
        private CollectionChangeUndoObserver _collectionObserver;
        #endregion

        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; RaisePropertyChanged("IsActive");}
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value; 
                RaisePropertyChanged("Title");
            }
        }

        public string BackgroundColor
        {
            get { return _color;  }
            set
            {
                Resolve<IUndoService>().Add(new PropertyChangeUndo(this, "BackgroundColor", _color, value));
                _color = value; 
                RaisePropertyChanged("BackgroundColor");
            }
        }

        public bool ShowText
        {
            get { return _showText; }
            set
            {
                Resolve<IUndoService>().Add(new PropertyChangeUndo(this, "ShowText", _showText, value));
                _showText = value; 
                RaisePropertyChanged("ShowText");
            }
        }

        public ObservableCollection<Element> Elements { get; private set; }

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

        // Can import services directly, or use Resolve<IUIVisualizer>() to retrieve instance JIT
        [Import] private IUIVisualizer _uiVisualizer = null;

        public WinViewModel()
        {
            _title = "MVVM Test";
            _color = "White";
            Elements = new ObservableCollection<Element>();
            _collectionObserver = new CollectionChangeUndoObserver(Elements, Resolve<IUndoService>());

            ActivatedCommand = new DelegateCommand(() => IsActive = true);
            DeactivatedCommand = new DelegateCommand(() => IsActive = false);
            LoadedCommand = new DelegateCommand(OnLoaded);
            CloseCommand = new DelegateCommand(OnClosed, OnCheckClose);
            ExitCommand = new DelegateCommand(RaiseCloseRequest);
            ShowColorDialogCommand = new DelegateCommand<Element>(ShowColorDialog);
            ShowPropertiesCommand = new DelegateCommand(ShowPropertyDialog);
            MouseEnterLeaveCommand = new DelegateCommand<EventParameters>(OnMouseEnterLeave);
            ChangeBackground = new DelegateCommand<string>(OnChangeBackground);
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
            _uiVisualizer.Show("ColorDialogVisual", new ColorDialogViewModel(parameter), true, null);
        }

        private void OnClosed()
        {
            Resolve<IErrorVisualizer>().Show("Window was closed", "The window has been closed.");
        }

        private bool OnCheckClose()
        {
            bool canClose = true;

            IMessageVisualizer messageVisualizer = Resolve<IMessageVisualizer>();
            if (messageVisualizer == null
                || messageVisualizer.GetType() != typeof(InternalMessageVisualizer))
            {
                Resolve<IErrorVisualizer>().Show("Error!", "Message Visualizer was not registered properly!");
            }
            else
                canClose = ((int)messageVisualizer.Show("Question", "Do you want to close this window?",
                                                  new MessageVisualizerOptions(UICommand.YesNo))) == 0;

            return canClose;
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
            bool? result = _uiVisualizer.ShowDialog("ShowProperties", propViewModel);
            if (result.HasValue && result.Value)
            {
                Title = propViewModel.Title;
                CreateShapes(propViewModel.ShapeCount);
            }
        }
    }
}