using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using JulMar.Core.Undo;
using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;
using System.ComponentModel.Composition;
using JulMar.Core.Interfaces;
using JulMar.Windows.UI;

namespace TestMvvm.ViewModels
{
    /// <summary>
    /// The ViewModel for the application
    /// </summary>
    [ExportViewModel("MainWindow")]
    public class MainViewModel : ViewModel
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
            set { SetPropertyValue(ref _isActive, value, "IsActive");}
        }

        public string Title
        {
            get { return _title; }
            set { SetPropertyValue(ref _title, value, "Title"); }
        }

        public string BackgroundColor
        {
            get { return _color;  }
            set
            {
                Resolve<IUndoService>().Add(new PropertyChangeUndo(this, "BackgroundColor", _color, value));
                SetPropertyValue(ref _color, value, "BackgroundColor");
            }
        }

        public bool ShowText
        {
            get { return _showText; }
            set
            {
                Resolve<IUndoService>().Add(new PropertyChangeUndo(this, "ShowText", _showText, value));
                SetPropertyValue(ref _showText, value, "ShowText");
            }
        }

        public ObservableCollection<Element> Elements { get; private set; }

        #region Commands
        public IDelegateCommand ActivatedCommand { get; private set; }
        public IDelegateCommand DeactivatedCommand { get; private set; }
        public IDelegateCommand LoadedCommand { get; private set; }
        public IDelegateCommand ClosingCommand { get; private set; }
        public IDelegateCommand ExitCommand { get; private set; }
        public IDelegateCommand ShowColorDialogCommand { get; private set; }
        public IDelegateCommand ShowPropertiesCommand { get; private set; }
        public IDelegateCommand MouseEnterLeaveCommand { get; private set; }
        public IDelegateCommand ChangeBackground { get; private set; }
        #endregion

        // Can import services directly, or use Resolve<IUIVisualizer>() to retrieve instance JIT
        [Import] private IUIVisualizer _uiVisualizer = null;

        public MainViewModel()
        {
            _title = "MVVM Test";
            _color = "White";
            Elements = new ObservableCollection<Element>();
            _collectionObserver = new CollectionChangeUndoObserver(Elements, Resolve<IUndoService>());

            ActivatedCommand = new DelegateCommand(() => IsActive = true);
            DeactivatedCommand = new DelegateCommand(() => IsActive = false);
            LoadedCommand = new DelegateCommand(OnLoaded);
            ClosingCommand = new DelegateCommand<CancelEventArgs>(OnClosing);
            ExitCommand = new DelegateCommand(RaiseCloseRequest);
            ShowColorDialogCommand = new DelegateCommand<Element>(ShowColorDialog);
            ShowPropertiesCommand = new DelegateCommand(ShowPropertyDialog);
            MouseEnterLeaveCommand = new DelegateCommand<RoutedEventArgs>(OnMouseEnterLeave);
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

        private void OnClosing(CancelEventArgs e)
        {
            bool canClose = Resolve<IMessageVisualizer>().Show(
                "Question", "Do you want to close this window?",
                          new MessageVisualizerOptions(UICommand.YesNo)) == UICommand.Yes;

            e.Cancel = !canClose;

        }

        private void OnMouseEnterLeave(RoutedEventArgs e)
        {
            Title = e.RoutedEvent == UIElement.MouseEnterEvent ? "MVVM Test (Active)" : "MVVM Test (Inactive)";
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