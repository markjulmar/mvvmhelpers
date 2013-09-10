using System.Collections.Generic;
using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;
using System;
using System.Threading.Tasks;

namespace VsmTest.ViewModels
{
    public sealed class MainViewModel : SimpleViewModel
    {
        private bool _moveRight;
        private string _direction;

        // Can also use Action (no parameter) in which case nothing is passed onto the
        // trigger action, but if a single parameter is passed it will be passed forward
        public event Action<string> GoLeft = delegate { };

        public bool MoveRight
        {
            get { return _moveRight; }
            set { SetPropertyValue(ref _moveRight,value); }
        }

        public IEnumerable<string> PossibleDirections
        {
            get { return new[] {"Top", "Center", "Left", "Right"}; }
        }

        public string Direction
        {
            get { return _direction; }
            set { SetPropertyValue(ref _direction, value); }
        }

        public IDelegateCommand Left { get; private set; }
        public IDelegateCommand Right { get; private set; }

        public MainViewModel()
        {
            Left = new DelegateCommand(() => GoLeft.Invoke("Left"));
            Right = new DelegateCommand(OnMoveRight);
        }

        private async void OnMoveRight(object obj)
        {
            MoveRight = true;
            await Task.Delay(1000);
            MoveRight = false;
            Direction = "Center";
        }
    }
}
