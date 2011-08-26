using System;
using System.Reflection;
using System.Threading;
using System.Windows.Media;
using JulMar.Windows.Mvvm;

namespace MTObservableTest.ViewModels
{
    public class UnitOfWork : SimpleViewModel
    {
        private static int _counter = 1;
        private static readonly Random RNG = new Random();
        private static readonly PropertyInfo[] AvailableColors = typeof(Colors).GetProperties();
        
        private int _percent;
        public int PercentComplete
        {
            get { return _percent; }
            set { _percent = value; OnPropertyChanged(() => PercentComplete); }
        }

        public string Color { get; private set; }
        public int Id { get; private set; }
        public int ThreadId { get; private set; }

        public UnitOfWork()
        {
            Id = Interlocked.Increment(ref _counter);
            ThreadId = Thread.CurrentThread.ManagedThreadId;
            Color = AvailableColors[RNG.Next(AvailableColors.Length - 1)].GetValue(null, null).ToString();
        }

        public override string ToString()
        {
            return string.Format("T{0}", Id);
        }

        public void Run()
        {
            PercentComplete = 0;
            int runtime = RNG.Next(5000);
            for (int i = 0; i < 20; i++)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(runtime / 100.0));
                PercentComplete++;
            }
        }
    }
}
