using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using JulMar.Windows.Mvvm;
using JulMar.Windows.Undo;
using JulMar.Windows.Interfaces;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new ObservableCollection<Person> 
            { 
                new Person("Mark",42),
                new Person("Julie",25),
            };

            InitializeComponent();

            new CollectionChangeUndoObserver((IList)DataContext);
        }

        private void OnDGRowChanged(object sender, System.EventArgs e)
        {
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
