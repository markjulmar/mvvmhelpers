using System;
using System.Windows;
using System.Windows.Threading;

namespace JulMar.Windows
{
    /// <summary>
    /// This class provides a simple deferred wrapper which binds two properties together and 
    /// transfers the values on a timeout interval
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// <StackPanel>
    ///    <StackPanel.Resources>
    ///       <DeferredBinder:DeferredBinding x:Key="dbTest" Timeout="1" />
    ///    </StackPanel.Resources>
    ///    <TextBlock x:Name="tb" Text="{Binding Source={StaticResource dbTest}, Path=Target}" FontSize="48pt" HorizontalAlignment="Center" />
    ///    <Slider x:Name="slider" Margin="10" HorizontalAlignment="Center" Width="200" Minimum="0" Maximum="100" Value="{Binding Source={StaticResource dbTest}, Path=Source, Mode=OneWayToSource}" />
    /// </StackPanel>
    /// ]]>
    /// </example>
    public class DeferredBinder : Freezable
    {
        /// <summary>
        /// Timer used to track value changes
        /// </summary>
        private DispatcherTimer _timer;

        #region Timeout

        /// <summary>
        /// Timeout Dependency Property - controls how long we wait until value is transferred.
        /// </summary>
        public static readonly DependencyProperty TimeoutProperty =
            DependencyProperty.Register("Timeout", typeof(double), typeof(DeferredBinder),
                new FrameworkPropertyMetadata((double)0.5, FrameworkPropertyMetadataOptions.None, OnTimeoutChanged));

        /// <summary>
        /// Gets or sets the Timeout property.
        /// </summary>
        public double Timeout
        {
            get { return (double)GetValue(TimeoutProperty); }
            set { SetValue(TimeoutProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Timeout property.
        /// </summary>
        private static void OnTimeoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DeferredBinder)d).ResetTimer();
        }

        #endregion

        #region Source

        /// <summary>
        /// Source Dependency Property - this is SOURCE of the property change
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(object), typeof(DeferredBinder),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, OnSourceChanged));

        /// <summary>
        /// Gets or sets the Source property.  
        /// </summary>
        public object Source
        {
            get { return GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Target property.
        /// </summary>
        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DeferredBinder)d).ResetTimer();
        }

        #endregion

        #region Target

        /// <summary>
        /// Target Dependency Property
        /// </summary>
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(object), typeof(DeferredBinder),
               new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));

        /// <summary>
        /// Gets or sets the Target property.  This dependency property 
        /// indicates ....
        /// </summary>
        public object Target
        {
            get { return GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        #endregion

        /// <summary>
        /// This resets the timer.
        /// </summary>
        private void ResetTimer()
        {
            if (_timer != null)
            {
                _timer.IsEnabled = false;
                _timer.Interval = TimeSpan.FromSeconds(Timeout);
                _timer.IsEnabled = true;
            }
            else
                _timer = new DispatcherTimer(TimeSpan.FromSeconds(Timeout), DispatcherPriority.Normal, OnTimeout, this.Dispatcher) { IsEnabled = true };
        }

        /// <summary>
        /// This is called when the timeout occurs and it transfers the source to the target.
        /// </summary>
        /// <param name="sender">Dispatcher Timer</param>
        /// <param name="e">Event arguments</param>
        private void OnTimeout(object sender, EventArgs e)
        {
            this.Target = this.Source;
            _timer.IsEnabled = false;
            _timer = null;
        }

        /// <summary>
        /// When implemented in a derived class, creates a new instance of the <see cref="T:System.Windows.Freezable"/> derived class. 
        /// </summary>
        /// <returns>
        /// The new instance.
        /// </returns>
        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
    }
}
