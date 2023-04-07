using System.Collections.Generic;
using System.Windows;
using Microsoft.Xaml.Behaviors;

using TriggerAction = Microsoft.Xaml.Behaviors.TriggerAction;
using TriggerBase = Microsoft.Xaml.Behaviors.TriggerBase;
using TriggerCollection = Microsoft.Xaml.Behaviors.TriggerCollection;

namespace JulMar.Windows
{
    /// <summary>
    /// Behaviors to apply
    /// </summary>
    public class StyleBehaviorCollection : FreezableCollection<Behavior> { /* */ }

    /// <summary>
    /// Triggers to apply
    /// </summary>
    public class StyleTriggerCollection : FreezableCollection<TriggerBase> { /* */ }

    /// <summary>
    /// This class applies Blend Behaviors and Triggers through Style Setter tags.
    /// </summary>
    public static class StyleInteraction
    {
        /// <summary>
        /// The Behaviors to apply
        /// </summary>
        public static readonly DependencyProperty BehaviorsProperty =
            DependencyProperty.RegisterAttached("Behaviors", typeof(StyleBehaviorCollection),
                            typeof(StyleInteraction), new FrameworkPropertyMetadata(null, OnBehaviorsPropertyChanged));

        /// <summary>
        /// Returns the applied behaviors collection
        /// </summary>
        /// <param name="uie"></param>
        /// <returns></returns>
        public static StyleBehaviorCollection GetBehaviors(UIElement uie)
        {
            return (StyleBehaviorCollection) uie.GetValue(BehaviorsProperty);
        }

        /// <summary>
        /// Sets the applied behaviors collection
        /// </summary>
        /// <param name="uie"></param>
        /// <param name="value"></param>
        public static void SetBehaviors(UIElement uie, StyleBehaviorCollection value)
        {
            uie.SetValue(BehaviorsProperty, value);
        }

        /// <summary>
        /// The Triggers to apply
        /// </summary>
        public static readonly DependencyProperty TriggersProperty =
            DependencyProperty.RegisterAttached("Triggers", typeof(StyleTriggerCollection),
                            typeof(StyleInteraction), new FrameworkPropertyMetadata(null, OnTriggersPropertyChanged));

        /// <summary>
        /// Returns the applied triggers collection
        /// </summary>
        /// <param name="uie"></param>
        /// <returns></returns>
        public static StyleTriggerCollection GetTriggers(UIElement uie)
        {
            return (StyleTriggerCollection) uie.GetValue(TriggersProperty);
        }

        /// <summary>
        /// Sets the applied triggers collection
        /// </summary>
        /// <param name="uie"></param>
        /// <param name="value"></param>
        public static void SetTriggers(UIElement uie, StyleTriggerCollection value)
        {
            uie.SetValue(TriggersProperty, value);
        }

        /// <summary>
        /// This is called when the behaviors collection is altered via a Style.
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="e"></param>
        private static void OnBehaviorsPropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            var uie = dpo as UIElement;
            if (uie == null)
                return;

            // Get the old and new behaviors.
            var oldBehaviors = e.OldValue as IList<Behavior>;
            var newBehaviors = e.NewValue as IList<Behavior>;

            // Get the actual System.Interaction collection.
            BehaviorCollection itemBehaviors = Interaction.GetBehaviors(uie);

            // Remove any missing behaviors.
            if (oldBehaviors != null)
            {
                foreach (var behavior in oldBehaviors)
                {
                    if (newBehaviors == null || !newBehaviors.Contains(behavior))
                        itemBehaviors.Remove(behavior);
                }
            }

            // Add any new behaviors.
            if (newBehaviors != null)
            {
                foreach (var behavior in newBehaviors)
                {
                    if (!itemBehaviors.Contains(behavior))
                    {
                        var thisBehavior = behavior;
                        if (thisBehavior.IsFrozen)
                            thisBehavior = (Behavior) behavior.Clone();
                        itemBehaviors.Add(thisBehavior);
                    }
                }
            }
        }

        /// <summary>
        /// This is called when the triggers collection is altered via a Style.
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="e"></param>
        private static void OnTriggersPropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            var uie = dpo as UIElement;
            if (uie == null)
                return;

            // Get the old and new trigger set.
            var oldTriggers = e.OldValue as IList<TriggerBase>;
            var newTriggers = e.NewValue as IList<TriggerBase>;

            // Get the actual System.Interaction collection
            TriggerCollection itemTriggers = Interaction.GetTriggers(uie);

            // Remove any old triggers
            if (oldTriggers != null)
            {
                foreach (var trigger in oldTriggers)
                {
                    if (newTriggers == null || !newTriggers.Contains(trigger))
                        itemTriggers.Remove(trigger);
                }
            }

            // Add new triggers
            if (newTriggers != null)
            {
                foreach (TriggerBase trigger in newTriggers)
                {
                    if (!itemTriggers.Contains(trigger))
                    {
                        var thisTrigger = trigger;
                        if (thisTrigger.IsFrozen)
                        {
                            thisTrigger = (TriggerBase) trigger.Clone();
                            foreach (var a in trigger.Actions)
                            {
                                var thisAction = (TriggerAction) a.Clone();
                                thisTrigger.Actions.Add(thisAction);
                            }
                        }
                        itemTriggers.Add(thisTrigger);
                    }
                }
            }
        }
    }
}


