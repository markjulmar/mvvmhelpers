using System;
using System.Threading;
using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;

namespace VMTriggers.ViewModels
{
    public enum StoplightState
    {
        Red, Green, Blue, Yellow,
    }

    public sealed class MainViewModel : SimpleViewModel
    {
        private Timer _timer;
        private StoplightState _lightState;
        private bool _useBindingTrigger;

        public bool UseBindingTrigger
        {
            get { return this._useBindingTrigger; }
            set { SetPropertyValue(ref _useBindingTrigger, value); }
        }

        public StoplightState LightState
        {
            get { return _lightState;  }
            set
            {
                if (_useBindingTrigger)
                {
                    SetPropertyValue(ref _lightState, value);
                }
                else
                {
                    _lightState = value;
                }
            }
        }

        public event Action<object> ChangeLight = delegate { };

        public IDelegateCommand StartStop { get; private set; }
        public MainViewModel()
        {
            StartStop = new DelegateCommand<string>(OnStartStop);
        }

        private void OnStartStop(string active)
        {
            if (string.Compare(active, "Start", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (_timer == null)
                {
                    _timer = new Timer(OnChangeLight, null, 1000, 500);
                }
            }
            else
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }
            }
        }

        private void OnChangeLight(object state)
        {
            switch (_lightState)
            {
                case StoplightState.Red:
                    LightState = StoplightState.Green;
                    break;
                case StoplightState.Green:
                    LightState = StoplightState.Blue;
                    break;
                case StoplightState.Blue:
                    LightState = StoplightState.Yellow;
                    break;
                case StoplightState.Yellow:
                    LightState = StoplightState.Red;
                    break;
            }

            if (!_useBindingTrigger)
            {
                ChangeLight(_lightState.ToString());
            }
        }
    }
}
