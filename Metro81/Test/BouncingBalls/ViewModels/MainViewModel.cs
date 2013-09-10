using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using JulMar.Windows.Mvvm;
using Windows.UI.Xaml;

namespace BouncingBalls.ViewModels
{
    public class MainViewModel : SimpleViewModel
    {
        private readonly Random _rng = new Random();
        private readonly object _guard = new object();
        private DispatcherTimer _timer;

        public IList<BallViewModel> Balls { get; private set; }

        public double Width { get; set; }
        public double Height { get; set; }

        public ICommand Add { get; private set; }

        public MainViewModel()
        {
            Balls = new ObservableCollection<BallViewModel>();
            Add = new DelegateCommand(OnAddMoreBalls);
            Run();
        }

        private void Run()
        {
            OnAddMoreBalls();

            _timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(10)};
            _timer.Tick += OnAnimateBalls;
            _timer.Start();
        }

        private void OnAnimateBalls(object sender, object e)
        {
            List<BallViewModel> balls = null;
            lock (_guard)
            {
                balls = Balls.ToList();
            }

            foreach (var ball in balls)
            {
                double ax = ball.AccelerationX, ay = ball.AccelerationY;
                double x = ball.X, y = ball.Y;

                x += ax;
                y += ay;

                if (x + ball.Radius >= Width)
                {
                    ax *= -1;
                    x += ax;
                }
                else if (x <= 0)
                {
                    ax *= -1;
                    x += ax;
                }

                if (y + ball.Radius >= Height)
                {
                    ay *= -1;
                    y += ay;
                }
                else if (y <= 0)
                {
                    ay *= -1;
                    y += ay;
                }

                ball.AccelerationX = ax;
                ball.AccelerationY = ay;
                ball.X = x;
                ball.Y = y;
            }
        }

        private void OnAddMoreBalls()
        {
            for (int i = 0; i < 5; i++)
            {
                BallViewModel bvm = new BallViewModel
                {
                    AccelerationX = _rng.NextDouble()*50, 
                    AccelerationY = _rng.NextDouble()*50,
                    Radius = 10 + _rng.NextDouble()*100,
                };
                lock (_guard)
                    Balls.Add(bvm);
            }
        }
    }
}
