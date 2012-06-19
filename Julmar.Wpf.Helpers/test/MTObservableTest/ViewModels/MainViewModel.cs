using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using JulMar.Windows;
using JulMar.Windows.Collections;
using JulMar.Windows.Mvvm;
using System.Threading;

namespace MTObservableTest.ViewModels
{
    [ExportViewModel("Main")]
    class MainViewModel : SimpleViewModel
    {
        private readonly CancellationTokenSource _cancellationToken = new CancellationTokenSource();
        public MTObservableCollection<UnitOfWork> TaskList  { get; private set; }
        public MTObservableCollection<ActivityLog> Activity { get; private set; }

        private bool _isRunning;
        public bool IsRunning
        {
            get { return _isRunning; }
            private set { _isRunning = value; RaisePropertyChanged(() => IsRunning); }
        }

        public ICommand StartStop { get; private set; }
        public ICommand Clear { get; private set; }
        public ICommand AddOne { get; private set; }
        public ICommand AddTwenty { get; private set; }

        public MainViewModel()
        {
            TaskList = new MTObservableCollection<UnitOfWork>();
            Activity = new MTObservableCollection<ActivityLog>();
            StartStop = new DelegateCommand(OnStartStopWork);
            AddOne = new DelegateCommand(AddTask);
            AddTwenty = new DelegateCommand(AddRange);
            Clear = new DelegateCommand(() => Activity.Clear());
            IsRunning = false;
        }

        private void OnStartStopWork()
        {
            IsRunning = !IsRunning;

            if (IsRunning)
            {
                Task.Factory.StartNew(RunTasks, _cancellationToken.Token, 
                    TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
            else
            {
                _cancellationToken.Cancel();
            }
        }
        
        private void RunTasks()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                while (TaskList.Count < 50)
                {
                    AddTask();
                }
                Thread.Sleep(100);
            }
        }

        private void AddRange()
        {
            UnitOfWork[] workItems = new UnitOfWork[20];
            ActivityLog[] logItems = new ActivityLog[20];

            for (int i = 0; i < workItems.Length; i++)
            {
                workItems[i] = new UnitOfWork();
                logItems[i] = new ActivityLog {Id = workItems[i].Id};
            }

            Stopwatch sw1 = Stopwatch.StartNew();
            TaskList.AddRange(workItems);
            Activity.AddRange(logItems);
            sw1.Stop();
            
            for (int i = 0; i < workItems.Length; i++)
            {
                UnitOfWork task = workItems[i];
                ActivityLog log = logItems[i];

                log.AddElapsed = sw1.Elapsed;
                Task.Factory.StartNew(task.Run)
                    .ContinueWith(t =>
                    {
                        Stopwatch sw2 = Stopwatch.StartNew();
                        bool rc = TaskList.Remove(task);
                        int index2 = TaskList.IndexOf(task);
                        log.RemoveElapsed = sw2.Elapsed;
                        log.Success = rc && index2 == -1;
                    });
            }
        }

        public void AddTask()
        {
            UnitOfWork task = new UnitOfWork();
            ActivityLog log = new ActivityLog() {Id = task.Id};
            
            Stopwatch sw1 = Stopwatch.StartNew();
            TaskList.Add(task);
            Activity.Add(log);
            log.AddElapsed = sw1.Elapsed;

            Task.Factory.StartNew(task.Run, TaskCreationOptions.AttachedToParent)
                .ContinueWith(t =>
                {
                    Stopwatch sw2 = Stopwatch.StartNew();
                    bool rc = TaskList.Remove(task);
                    int index2 = TaskList.IndexOf(task);
                    log.RemoveElapsed = sw2.Elapsed;
                    log.Success = rc && index2 == -1;
                });
        }
    }
}
