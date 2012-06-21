using JulMar.Windows.Mvvm;

namespace BouncingBalls.ViewModels
{
    public class BallViewModel : SimpleViewModel
    {
        private double _x;
        public double X
        {
            get { return _x; }
            set { SetPropertyValue(ref _x, value); }
        }

        private double _y;
        public double Y
        {
            get { return _y; }
            set { SetPropertyValue(ref _y, value); }
        }

        private double _accelerationX;
        public double AccelerationX
        {
            get { return _accelerationX; }
            set { SetPropertyValue(ref _accelerationX, value); }
        }

        private double _accelerationY;
        public double AccelerationY
        {
            get { return _accelerationY; }
            set { SetPropertyValue(ref _accelerationY, value); }
        }

        public double Radius { get; set; }
        public double SmallRadius { get { return Radius/4; } }
    }
}
