using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using JulMar.Windows.Mvvm;
using System.Diagnostics;

namespace DeferredBindTest
{
    [ExportViewModel("Main")]
    public class MainViewModel : ViewModel
    {
        private readonly Stopwatch _stopwatch;
        private Timer _timer;

        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value; OnPropertyChanged(() => Text); }
        }

        private double _value;
        public double Value
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged(() => Value); }
        }

        public string Countdown
        {
            get { return _stopwatch.Elapsed.ToString(@"hh\:mm\:ss"); }
        }

        public MainViewModel()
        {
            _stopwatch = Stopwatch.StartNew();
            _timer = new Timer(o => OnPropertyChanged(() => Countdown), null, 0, 1000);
        }
    }
}
