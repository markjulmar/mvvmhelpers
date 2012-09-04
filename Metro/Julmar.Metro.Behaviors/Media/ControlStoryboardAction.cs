using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace JulMar.Windows.Interactivity.Media
{
    /// <summary>
    /// Storyboard action control
    /// </summary>
    public class ControlStoryboardAction : StoryboardAction
    {
        /// <summary>
        /// Backing storage for the ControlStoryboard property
        /// </summary>
        public static readonly DependencyProperty ControlStoryboardProperty = DependencyProperty.Register("ControlStoryboardOption", 
            typeof(ControlStoryboardOption), typeof(ControlStoryboardAction), null);
        
        /// <summary>
        /// Current play progress
        /// </summary>
        private bool _isPaused;

        /// <summary>
        /// Method called when storyboard changes.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStoryboardChanged(DependencyPropertyChangedEventArgs args)
        {
            this._isPaused = false;
        }

        /// <summary>
        /// Type of control to perform on storyboard
        /// </summary>
        public ControlStoryboardOption ControlStoryboardOption
        {
            get
            {
                return (ControlStoryboardOption)base.GetValue(ControlStoryboardProperty);
            }
            set
            {
                base.SetValue(ControlStoryboardProperty, value);
            }
        }

        /// <summary>
        /// Invoke method - must be overridden
        /// </summary>
        /// <param name="parameter"></param>
        protected override void Invoke(object parameter)
        {
            if ((base.AssociatedObject != null) && (base.Storyboard != null))
            {
                switch (this.ControlStoryboardOption)
                {
                    case ControlStoryboardOption.Play:
                        base.Storyboard.Begin();
                        return;

                    case ControlStoryboardOption.Stop:
                        base.Storyboard.Stop();
                        return;

                    case ControlStoryboardOption.TogglePlayPause:
                        {
                            ClockState currentState = ClockState.Stopped;
                            
                            try
                            {
                                currentState = base.Storyboard.GetCurrentState();
                            }
                            catch (InvalidOperationException)
                            {
                            }

                            if (currentState == ClockState.Stopped)
                            {
                                _isPaused = false;
                                base.Storyboard.Begin();
                                return;
                            }
                            
                            if (_isPaused)
                            {
                                _isPaused = false;
                                base.Storyboard.Resume();
                                return;
                            }

                            _isPaused = true;
                            base.Storyboard.Pause();
                            return;
                        }
                    case ControlStoryboardOption.Pause:
                        base.Storyboard.Pause();
                        return;

                    case ControlStoryboardOption.Resume:
                        base.Storyboard.Resume();
                        return;

                    case ControlStoryboardOption.SkipToFill:
                        base.Storyboard.SkipToFill();
                        break;

                    default:
                        return;
                }
            }
        }
    }
}
