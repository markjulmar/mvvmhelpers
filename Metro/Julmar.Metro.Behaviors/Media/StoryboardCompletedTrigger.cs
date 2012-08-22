using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace JulMar.Windows.Interactivity.Media
{
    /// <summary>
    /// Trigger to run an action when a storyboard finishes.
    /// </summary>
    public class StoryboardCompletedTrigger : StoryboardTrigger
    {
        /// <summary>
        /// Override called when behavior is detached
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (base.Storyboard != null)
            {
                base.Storyboard.Completed -= StoryboardCompleted;
            }
        }

        /// <summary>
        /// Method called when the storyboard is changed
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStoryboardChanged(DependencyPropertyChangedEventArgs args)
        {
            Storyboard storyboard = args.OldValue as Storyboard;
            Storyboard storyboard2 = args.NewValue as Storyboard;

            if (storyboard != storyboard2)
            {
                if (storyboard != null)
                {
                    storyboard.Completed -= StoryboardCompleted;
                }
                if (storyboard2 != null)
                {
                    storyboard2.Completed += StoryboardCompleted;
                }
            }
        }

        /// <summary>
        /// Method called when the storyboard completes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StoryboardCompleted(object sender, object e)
        {
            base.InvokeActions(e);
        }
    }

}
