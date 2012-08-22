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
        public static readonly DependencyProperty TargetObjectProperty = DependencyProperty.Register("TargetObject", typeof(object), typeof(TargetedTriggerAction), new PropertyMetadata(null, OnTargetObjectChanged));

        /// <summary>
        /// Target object to work on
        /// </summary>
        public object TargetObject
        {
            get { return base.GetValue(TargetObjectProperty); }
            set { base.SetValue(TargetObjectProperty, value); }
        }

        /// <summary>
        // TargetName dependency property
        /// </summary>
        public static readonly DependencyProperty TargetNameProperty = DependencyProperty.Register("TargetName", typeof(string), typeof(TargetedTriggerAction), new PropertyMetadata(null, OnTargetObjectChanged));

        /// <summary>
        /// TargetObject name
        /// </summary>
        public string TargetName
        {
            get { return (string)base.GetValue(TargetNameProperty); }
            set { base.SetValue(TargetNameProperty, value); }
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
            get
            {
                if (TargetObject != null)
                    return TargetObject;

                if (AssociatedObject != null && !String.IsNullOrEmpty(TargetName))
                {
                    var value = AssociatedObject.FindName(TargetName);
                    if (value != null)
                        return value;
                }

                return AssociatedObject;
            }
        }

        /// <summary>
        /// Method called when the target object is changed
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTargetObjectChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Change notification handler for the TargetObject.
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="e"></param>
        static void OnTargetObjectChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            ((TargetedTriggerAction) dpo).OnTargetObjectChanged(e);
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
