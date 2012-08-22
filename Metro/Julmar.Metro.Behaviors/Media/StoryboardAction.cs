using System.Windows.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace JulMar.Windows.Interactivity.Media
{
    /// <summary>
    /// Action to work with a storyboard animation
    /// </summary>
    public abstract class StoryboardAction : TriggerAction<FrameworkElement>
    {
        /// <summary>
        /// Backing storage for Storyboard
        /// </summary>
        public static readonly DependencyProperty StoryboardProperty = DependencyProperty.Register("Storyboard", typeof(Storyboard), typeof(StoryboardAction),
            new PropertyMetadata(null, OnStoryboardChanged));

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
        /// Method called when storyboard changes.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnStoryboardChanged(DependencyPropertyChangedEventArgs args)
        {
        }

        /// <summary>
        /// Change notification handler for Storyboard property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnStoryboardChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            StoryboardAction action = sender as StoryboardAction;
            if (action != null)
            {
                action.OnStoryboardChanged(args);
            }
        }

    }

}
