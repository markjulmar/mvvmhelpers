using System;

using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Animation;

using Microsoft.Xaml.Interactivity;

namespace JulMar.Interactivity.Media
{
    /// <summary>
    /// Trigger to run an action when a storyboard finishes.
    /// </summary>
    [ContentProperty(Name="Actions")]
    public class StoryboardCompletedTriggerBehavior : DependencyObject, IBehavior
    {
        /// <summary>
        /// Backing storage for the Storyboard
        /// </summary>
        public static readonly DependencyProperty StoryboardProperty = DependencyProperty.Register("Storyboard",
            typeof(Storyboard), typeof(StoryboardCompletedTriggerBehavior), new PropertyMetadata(null, OnStoryboardChanged));

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
        /// Backing storage for Actions collection
        /// </summary>
        public static readonly DependencyProperty ActionsProperty =
            DependencyProperty.Register("Actions", typeof(ActionCollection), typeof(TimerTriggerBehavior), new PropertyMetadata(null));

        /// <summary>
        /// Actions collection
        /// </summary>
        public ActionCollection Actions
        {
            get
            {
                ActionCollection actions = (ActionCollection) base.GetValue(ActionsProperty);
                if (actions == null)
                {
                    actions = new ActionCollection();
                    base.SetValue(ActionsProperty, actions);
                }
                return actions;
            }
        }

        /// <summary>
        /// Method called when the storyboard is changed
        /// </summary>
        /// <param name="args"></param>
        private void OnStoryboardChanged(DependencyPropertyChangedEventArgs args)
        {
            Storyboard storyboard = args.OldValue as Storyboard;
            Storyboard storyboard2 = args.NewValue as Storyboard;

            if (storyboard != storyboard2)
            {
                if (storyboard != null)
                {
                    storyboard.Completed -= this.StoryboardCompleted;
                }
                if (storyboard2 != null)
                {
                    storyboard2.Completed += this.StoryboardCompleted;
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
            Interaction.ExecuteActions(this.AssociatedObject, this.Actions, e);
        }

        /// <summary>
        /// Change notification handler for the storyboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnStoryboardChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var trigger = sender as StoryboardCompletedTriggerBehavior;
            if (trigger != null)
            {
                trigger.OnStoryboardChanged(args);
            }
        }

        /// <summary>
        /// Attaches to the specified object.
        /// </summary>
        /// <param name="associatedObject">The <see cref="T:Windows.UI.Xaml.DependencyObject"/> to which the <seealso cref="T:Microsoft.Xaml.Interactivity.IBehavior"/> will be attached.</param>
        public void Attach(DependencyObject associatedObject)
        {
            if ((associatedObject != this.AssociatedObject) && !DesignMode.DesignModeEnabled)
            {
                if (this.AssociatedObject != null)
                    throw new InvalidOperationException("Cannot attach behavior multiple times.");

                this.AssociatedObject = associatedObject;
                if (this.Storyboard != null)
                {
                    this.Storyboard.Completed += this.StoryboardCompleted;
                }
            }
        }

        /// <summary>
        /// Detaches this instance from its associated object.
        /// </summary>
        public void Detach()
        {
            if (this.Storyboard != null)
                this.Storyboard.Completed -= this.StoryboardCompleted;
            
            this.AssociatedObject = null;
        }

        /// <summary>
        /// Gets the <see cref="T:Windows.UI.Xaml.DependencyObject"/> to which the <seealso cref="T:Microsoft.Xaml.Interactivity.IBehavior"/> is attached.
        /// </summary>
        public DependencyObject AssociatedObject { get; private set; }
    }

}
