using System.Windows.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace JulMar.Windows.Interactivity.Media
{
    public abstract class StoryboardTrigger : TriggerBase<FrameworkElement>
    {
        /// <summary>
        /// Backing storage for the Storyboard
        /// </summary>
        public static readonly DependencyProperty StoryboardProperty = DependencyProperty.Register("Storyboard", 
            typeof(Storyboard), typeof(StoryboardTrigger), new PropertyMetadata(null, OnStoryboardChanged));

        /// <summary>
        /// Storyboard to work with
        /// </summary>
        public Storyboard Storyboard
        {
            get
            {
                return (Storyboard)base.GetValue(StoryboardProperty);
            }
            set
            {
                base.SetValue(StoryboardProperty, value);
            }
        }

        /// <summary>
        /// Method called when the storyboard is changed
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnStoryboardChanged(DependencyPropertyChangedEventArgs args)
        {
        }

        /// <summary>
        /// Change notification handler for the storyboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnStoryboardChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            StoryboardTrigger trigger = sender as StoryboardTrigger;
            if (trigger != null)
            {
                trigger.OnStoryboardChanged(args);
            }
        }

    }

}
