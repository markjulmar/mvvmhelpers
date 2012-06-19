using JulMar.Core.Interfaces;
using JulMar.Core.Undo;
using JulMar.Windows.Mvvm;

namespace WpfApplication1
{
    /// <summary>
    ///  Simple ViewModel to represent a person object
    /// </summary>
    public class Person : ViewModel
    {
        private string _name;
        private int _age;

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

        public string Name
        {
            get { return _name; }
            set
            {
                Resolve<IUndoService>().Add(new PropertyChangeUndo(this, "Name", _name, value));
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        public int Age
        {
            get { return _age; }
            set
            {
                Resolve<IUndoService>().Add(new PropertyChangeUndo(this, "Age", _age, value));
                _age = value;
                RaisePropertyChanged("Age");
            }
        }
    }
}