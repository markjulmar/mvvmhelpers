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
 
        /// <summary>
        /// True if we are updating the source/target internally
        /// </summary>
        private bool _isUpdatingTarget, _isUpdatingSource;

        #region Timeout

        /// <summary>
        /// Timeout Dependency Property - controls how long we wait until value is transferred.
        /// </summary>
        public static readonly DependencyProperty TimeoutProperty =
            DependencyProperty.Register("Timeout", typeof(double), typeof(DeferredBinder),
                                        new FrameworkPropertyMetadata(0.5, FrameworkPropertyMetadataOptions.None,
                                                                      OnTimeoutChanged));

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
            ((DeferredBinder)d).ResetTimer(false);
        }

        #endregion

        #region Source

        /// <summary>
        /// Source Dependency Property - this is SOURCE of the property change
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(object), typeof(DeferredBinder),
                                        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSourceChanged));

        /// <summary>
        /// Gets or sets the Source property.  
        /// </summary>
        public object Source
        {
            get { return GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Source property.
        /// </summary>
        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DeferredBinder)d).OnSourceChanged();
        }

        #endregion

        #region Target

        /// <summary>
        /// Target Dependency Property
        /// </summary>
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(object), typeof(DeferredBinder),
                                        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTargetChanged));

        /// <summary>
        /// Gets or sets the Target property.  This dependency property 
        /// indicates ....
        /// </summary>
        public object Target
        {
            get { return GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        /// <summary>
        /// Called when the target changes.
        /// </summary>
        private static void OnTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DeferredBinder)d).OnTargetChanged(e.OldValue, e.NewValue);
        }

        #endregion

        /// <summary>
        /// Backing storage for TwoWayBinding property
        /// </summary>
        public static readonly DependencyProperty TwoWayBindingProperty =
            DependencyProperty.Register("TwoWayBinding", typeof(bool), typeof(DeferredBinder), new PropertyMetadata(true));

        /// <summary>
        /// True for 2-way binding (source -> target, target -> source)
        /// </summary>
        public bool TwoWayBinding
        {
            get { return (bool)GetValue(TwoWayBindingProperty); }
            set { SetValue(TwoWayBindingProperty, value); }
        }

        /// <summary>
        /// Source has changed value - reset the timer to transfer
        /// from source -> target.
        /// </summary>
        private void OnSourceChanged()
        {
            if (!_isUpdatingSource)
            {
                ResetTimer(true);
            }
        }

        /// <summary>
        /// This resets the timer.
        /// </summary>
        private void ResetTimer(bool createTimer)
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Interval = TimeSpan.FromSeconds(Timeout);
                _timer.Start();
            }
            else if (createTimer)
                _timer = new DispatcherTimer(TimeSpan.FromSeconds(Timeout), DispatcherPriority.Normal, OnTimeout,
                                             this.Dispatcher) { IsEnabled = true };
        }

        /// <summary>
        /// Target changed value - update the source if necessary.
        /// </summary>
        private void OnTargetChanged(object oldValue, object newValue)
        {
            if (TwoWayBinding && !_isUpdatingTarget)
            {
                _isUpdatingSource = true;
                try
                {
                    Source = newValue;
                }
                finally
                {
                    _isUpdatingSource = false;
                }
            }
        }

        /// <summary>
        /// This is called when the timeout occurs and it transfers the source to the target.
        /// </summary>
        /// <param name="sender">Dispatcher Timer</param>
        /// <param name="e">Event arguments</param>
        private void OnTimeout(object sender, EventArgs e)
        {
            _isUpdatingTarget = true;
            try
            {
                this.Target = this.Source;
                _timer.IsEnabled = false;
                _timer = null;
            }
            finally
            {
                _isUpdatingTarget = false;
            }
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