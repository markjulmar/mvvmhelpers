using Windows.UI.Xaml;

namespace System.Windows.Interactivity
{
    /// <summary>
    /// Manager for behaviors - attaches the collection of behaviors to an element.
    /// Modeled after System.Windows.Interactivity: remove once that is available
    /// </summary>
    public static class Interaction
    {
        public static readonly DependencyProperty BehaviorsProperty = DependencyProperty.RegisterAttached("Behaviors",
                typeof(BehaviorCollection), typeof(Interaction), new PropertyMetadata(null, OnBehaviorListChanged));

        /// <summary>
        /// Get the behavior list
        /// </summary>
        /// <param name="obj">Object to retrieve behavior collection from</param>
        /// <returns>Behavior collection (empty if none)</returns>
        public static BehaviorCollection GetBehaviors(DependencyObject obj)
        {
            var coll = (BehaviorCollection)obj.GetValue(BehaviorsProperty);
            if (coll == null)
            {
                coll = new BehaviorCollection();
                SetBehaviors(obj, coll);
            }
            return coll;
        }

        /// <summary>
        /// Sets the behavior list.
        /// </summary>
        /// <param name="obj">Object to set behavior collection on</param>
        /// <param name="value">Collection to assign</param>
        public static void SetBehaviors(DependencyObject obj, BehaviorCollection value)
        {
            obj.SetValue(BehaviorsProperty, value);
        }

        /// <summary>
        /// This is called when the behavior collection is changed on an object.
        /// </summary>
        /// <param name="dpo">FrameworkElement owner</param>
        /// <param name="e"></param>
        private static void OnBehaviorListChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            BehaviorCollection list = e.OldValue as BehaviorCollection;
            if (list != null)
            {
                list.Detach();
            }

            list = e.NewValue as BehaviorCollection;
            FrameworkElement fe = dpo as FrameworkElement;
            if (list != null && fe != null)
            {
                list.Attach(fe);
            }
        }

        public static readonly DependencyProperty TriggersProperty = DependencyProperty.RegisterAttached("Triggers",
            typeof(TriggerCollection), typeof(Interaction), new PropertyMetadata(null, OnTriggerListChanged));

        /// <summary>
        /// Get the trigger list
        /// </summary>
        /// <param name="obj">Object to retrieve trigger collection from</param>
        /// <returns>Behavior collection (empty if none)</returns>
        public static TriggerCollection GetTriggers(DependencyObject obj)
        {
            var coll = (TriggerCollection)obj.GetValue(TriggersProperty);
            if (coll == null)
            {
                coll = new TriggerCollection();
                SetTriggers(obj, coll);
            }
            return coll;
        }

        /// <summary>
        /// Sets the behavior list.
        /// </summary>
        /// <param name="obj">Object to set behavior collection on</param>
        /// <param name="value">Collection to assign</param>
        public static void SetTriggers(DependencyObject obj, TriggerCollection value)
        {
            obj.SetValue(TriggersProperty, value);
        }

        /// <summary>
        /// This is called when the behavior collection is changed on an object.
        /// </summary>
        /// <param name="dpo">FrameworkElement owner</param>
        /// <param name="e"></param>
        private static void OnTriggerListChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            TriggerCollection list = e.OldValue as TriggerCollection;
            if (list != null)
            {
                list.Detach();
            }

            list = e.NewValue as TriggerCollection;
            FrameworkElement fe = dpo as FrameworkElement;
            if (list != null && fe != null)
            {
                list.Attach(fe);
            }
        }

    }
}
