using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using JulMar.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Diagnostics;

namespace MTObservableTest
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1
    {
        public class TimeString
        {
            public Task Task { get; set; }
            public string Operation { get; set; }
            public TimeSpan Elapsed { get; set; }

            public override string ToString()
            {
                return string.Format("{0}: {1} {2}", Elapsed, Operation, Task);
            }
        }

        private DispatcherTimer _timer;
        private BackgroundWorker _worker;
        readonly MTObservableCollection<Task> _taskList = new MTObservableCollection<Task>();
        readonly MTObservableCollection<TimeString> _text = new MTObservableCollection<TimeString>();
        
        public Window1()
        {
            ThreadPool.SetMaxThreads(200, 200);
            ThreadPool.SetMinThreads(100, 100);

            InitializeComponent();

            taskList.ItemsSource = _taskList;
            debugText.ItemsSource = _text;

            ICollectionView cv = CollectionViewSource.GetDefaultView(_text);
            cv.SortDescriptions.Add(new SortDescription("Elapsed", ListSortDirection.Descending));
        }

        private void OnAddNewTask(object sender, RoutedEventArgs e)
        {
            Button btn = (Button) sender;

            if (_timer != null)
            {
                btn.Content = "Start";
                _timer.Stop();
                _timer = null;
                _worker.CancelAsync();
                _worker = null;
            }
            else
            {
                btn.Content = "Stop";
                _worker = new BackgroundWorker { WorkerSupportsCancellation = true };
                _worker.DoWork += _worker_DoWork;
                _worker.RunWorkerAsync();

                _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(50), DispatcherPriority.Normal, OnTimer,
                                             Dispatcher) {IsEnabled = true};
            }
        }

        void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = (BackgroundWorker) sender;
            while (!bw.CancellationPending)
            {
                if (_taskList.Count < 100)
                    AddTask();
                Thread.Sleep(100);
            }
        }

        private void OnTimer(object sender, EventArgs e)
        {
            AddTask();
        }

        private void OnClear(object sender, RoutedEventArgs e)
        {
            _text.Clear();
        }

        private void AddTask()
        {
            Task task = new Task();

            Stopwatch sw1 = new Stopwatch();
            sw1.Start();
            _taskList.Add(task);
            sw1.Stop();

            _text.Add(new TimeString {Elapsed = sw1.Elapsed, Task = task, Operation = "Added"});

            task.RunTask(t =>
            {
                Stopwatch sw2 = new Stopwatch();
                sw2.Start();

                Debug.Assert(t == task);
                bool rc = _taskList.Remove(t);
                Debug.Assert(rc == true);
                int index = _taskList.IndexOf(t);
                Debug.Assert(index == -1);

                sw2.Stop();
                _text.Add(new TimeString { Elapsed = sw2.Elapsed, Task = task, Operation = "Removed" });
            });
        }
    }
}
