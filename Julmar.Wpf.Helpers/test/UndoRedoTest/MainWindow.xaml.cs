using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using JulMar.Core.Interfaces;
using JulMar.Core.Undo;
using JulMar.Windows.Mvvm;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [Import]
        private IUndoService _undoService = null;

        public MainWindow()
        {
            // Make sure we get properly composed.
            ViewModel.ServiceProvider.Resolve<IDynamicResolver>().Compose(this);

            // Setup Undo/Redo key bindings.  This is not done automatically since the service is in Core.
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, (s, e) => _undoService.Undo(), (s, e) => e.CanExecute = _undoService.CanUndo));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Redo, (s, e) => _undoService.Redo(), (s, e) => e.CanExecute = _undoService.CanRedo));

            DataContext = new ObservableCollection<Person> 
            { 
                new Person("Mark",42),
                new Person("Julie",25),
            };

            InitializeComponent();

            new CollectionChangeUndoObserver((IList)DataContext, ViewModel.ServiceProvider.Resolve<IUndoService>());
        }
    }

    public class Person : ViewModel
    {
        public Person(string name, int age)
        {
            _name = name;
            _age = age;
        }

        public Person()
        {
            _name = "Enter Name";
            _age = 0;
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                Resolve<IUndoService>().Add(new PropertyChangeUndo(this, "Name", _name, value));
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private int _age;
        public int Age
        {
            get { return _age; }
            set
            {
                Resolve<IUndoService>().Add(new PropertyChangeUndo(this, "Age", _age, value));
                _age = value;
                OnPropertyChanged("Age");
            }
        }
    }
}
