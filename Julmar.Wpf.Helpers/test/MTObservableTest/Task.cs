using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Windows.Media;

namespace MTObservableTest
{
    public class Task : INotifyPropertyChanged
    {
        private static int _counter = 1;
        private static readonly Random _rnd = new Random();
        
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propName)
        {
            var pc = PropertyChanged;
            if (pc != null)
                pc.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private int _percent;
        public int PercentComplete
        {
            get { return _percent; }
            set { _percent = value; OnPropertyChanged("PercentComplete"); }
        }

        private static readonly PropertyInfo[] colorProps = typeof(Colors).GetProperties();
        public string Color
        {
            get
            {
                return colorProps[_rnd.Next(colorProps.Length-1)].GetValue(null, null).ToString();
            }
        }

        private readonly int _id;
        public int Id
        {
            get { return _id; }
        }

        public Task()
        {
            _id = Interlocked.Increment(ref _counter);
        }

        public override string ToString()
        {
            return string.Format("T{0}", Id);
        }

        public void RunTask(Action<Task> workComplete)
        {
            PercentComplete = 0;

            Action work = DoWork;
            work.BeginInvoke(ia => 
            {
                 work.EndInvoke(ia);
                 workComplete(this);
             }, null);
        }

        private void DoWork()
        {
            int runtime = _rnd.Next(100);
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(runtime / 100.0));
                PercentComplete++;                
            }
        }
    }
}
