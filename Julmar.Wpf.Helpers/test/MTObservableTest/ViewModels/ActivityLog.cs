using System;
using JulMar.Windows.Mvvm;

namespace MTObservableTest.ViewModels
{
    public class ActivityLog : SimpleViewModel
    {
        public int Id { get; set; }
        private TimeSpan _addElapsed;
        public TimeSpan AddElapsed
        {
            get { return _addElapsed; }
            set
            {
                _addElapsed = value;
                Elapsed = value;
                RaisePropertyChanged(() => AddElapsed);
            }
        }

        private TimeSpan _removeElapsed;
        public TimeSpan RemoveElapsed
        {
            get { return _removeElapsed; }
            set
            {
                _removeElapsed = value; 
                RaisePropertyChanged(() => RemoveElapsed);
                if (Elapsed < value)
                    Elapsed = value;
            }
        }

        private TimeSpan _elapsed;
        public TimeSpan Elapsed
        {
            get { return _elapsed; }
            set { _elapsed = value; RaisePropertyChanged(() => Elapsed); }
        }

        private bool _success;
        public bool Success
        {
            get { return _success; }
            set { _success = value; RaisePropertyChanged(() => Success); }
        }
    }
}