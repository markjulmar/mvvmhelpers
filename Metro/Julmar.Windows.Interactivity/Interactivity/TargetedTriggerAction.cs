using Windows.UI.Xaml;

namespace System.Windows.Interactivity
{
    /// <summary>
    /// This represents a TriggerAction which has a separate Target to work on.
    /// </summary>
    public abstract class TargetedTriggerAction : TriggerAction
    {
        /// <summary>
        // Target dependency property
        /// </summary>
        public static readonly DependencyProperty TargetObjectProperty = DependencyProperty.Register("TargetObject", typeof(object), typeof(TargetedTriggerAction), new PropertyMetadata(null));

        /// <summary>
        /// Target object to work on
        /// </summary>
        public object TargetObject
        {
            get { return base.GetValue(TargetObjectProperty); }
            set { base.SetValue(TargetObjectProperty, value); }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="associatedObjectTypeConstraint"></param>
        protected TargetedTriggerAction(Type associatedObjectTypeConstraint) : base(associatedObjectTypeConstraint)
        {
        }

        /// <summary>
        /// Target object (either TargetObject or AssociatedObject)
        /// </summary>
        protected object Target
        {
            get { return TargetObject ?? AssociatedObject; }
        }
    }

    /// <summary>
    /// Typesafe targeted trigger action
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TargetedTriggerAction<T> : TargetedTriggerAction where T : class
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected TargetedTriggerAction() : base(typeof(T))
        {
        }

        /// <summary>
        /// Target object
        /// </summary>
        protected new T Target
        {
            get { return (T)base.Target; }
        }
    }
}
