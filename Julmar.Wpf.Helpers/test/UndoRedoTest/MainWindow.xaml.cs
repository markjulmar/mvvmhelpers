using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using JulMar.Core.Interfaces;
using JulMar.Core.Services;
using JulMar.Core.Undo;
using JulMar.Windows.Mvvm;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly IUndoService _undoService;
        private CollectionChangeUndoObserver _ccObserver;

        public MainWindow()
        {
            // Get the undo service
            _undoService = ServiceLocator.Instance.Resolve<IUndoService>();

            // Setup Undo/Redo key bindings.  This is not done automatically since the service is in Core.
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, (s, e) => _undoService.Undo(), (s, e) => e.CanExecute = _undoService.CanUndo));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Redo, (s, e) => _undoService.Redo(), (s, e) => e.CanExecute = _undoService.CanRedo));

            Loaded += MainWindowLoaded;
            Unloaded += MainWindowUnloaded;
            InitializeComponent();
        }

        void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = new ObservableCollection<Person> 
            { 
                new Person("Mark",42),
                new Person("Julie",25),
            };

            // Setup the collection change observer
            _ccObserver = new CollectionChangeUndoObserver((IList)DataContext, _undoService);
        }

        void MainWindowUnloaded(object sender, RoutedEventArgs e)
        {
            _ccObserver.Dispose();
        }
    }
}
