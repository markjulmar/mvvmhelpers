using System;
using Windows.UI.Xaml;

namespace JulMar.Windows.Interactivity
{
    /// <summary>
    /// Trigger which runs off a timer
    /// </summary>
    public sealed class TimerTrigger : System.Windows.Interactivity.EventTrigger
    {
        private object _eventArgs;
        private int _tickCount;
        private DispatcherTimer _timer;

        /// <summary>
        /// Backing storage for the counter
        /// </summary>
        public static readonly DependencyProperty MillisecondsPerTickProperty = DependencyProperty.Register("MillisecondsPerTick", typeof(double), typeof(TimerTrigger), new PropertyMetadata(1000.0));

        /// <summary>
        /// Milliseconds
        /// </summary>
        public double MillisecondsPerTick
        {
            get
            {
                return (double)base.GetValue(MillisecondsPerTickProperty);
            }
            set
            {
                base.SetValue(MillisecondsPerTickProperty, value);
            }
        }

        /// <summary>
        /// Backing storage for the total ticks counter
        /// </summary>
        public static readonly DependencyProperty TotalTicksProperty = DependencyProperty.Register("TotalTicks", typeof(int), typeof(TimerTrigger), new PropertyMetadata(-1));

        /// <summary>
        /// Total ticks elapsed
        /// </summary>
        public int TotalTicks
        {
            get
            {
                return (int)base.GetValue(TotalTicksProperty);
            }
            set
            {
                base.SetValue(TotalTicksProperty, value);
            }
        }

        /// <summary>
        /// Override called when behavior is detached
        /// </summary>
        protected override void OnDetaching()
        {
            this.StopTimer();
            base.OnDetaching();
        }

        /// <summary>
        /// This invokes the actions when the event is raised.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnEvent(object e)
        {
            this.StopTimer();
            this._eventArgs = e;
            this._tickCount = 0;
            this.StartTimer();
        }

        /// <summary>
        /// Called when the timer elapses
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimerTick(object sender, object e)
        {
            if (this.TotalTicks > 0 && ++this._tickCount >= this.TotalTicks)
            {
                this.StopTimer();
            }
            
            base.InvokeActions(this._eventArgs);
        }

        /// <summary>
        /// Called to start the timer
        /// </summary>
        internal void StartTimer()
        {
            this._timer = new DispatcherTimer {Interval = (TimeSpan.FromMilliseconds(this.MillisecondsPerTick))};
            this._timer.Tick += this.OnTimerTick;
            this._timer.Start();
        }

        /// <summary>
        /// Called to stop the timer
        /// </summary>
        internal void StopTimer()
        {
            if (this._timer != null)
            {
                this._timer.Stop();
                this._timer = null;
            }
        }
    }
}
