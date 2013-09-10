using System.ComponentModel;
using Windows.UI.Xaml;

namespace System.Windows.Interactivity
{
    /// <summary>
    /// Action performed on behalf of a Blend trigger
    /// </summary>
    public abstract class TriggerAction : AttachedObject
    {
        /// <summary>
        /// IsEnabled dependency property backing storage
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled", typeof(bool), typeof(TriggerAction), new PropertyMetadata(true));

        /// <summary>
        /// True/False for enabled state
        /// </summary>
        [DefaultValue(true)]
        public bool IsEnabled
        {
            get
            {
                return (bool)base.GetValue(IsEnabledProperty);
            }

            set
            {
                base.SetValue(IsEnabledProperty, value);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="associatedObjectTypeConstraint"></param>
        internal TriggerAction(Type associatedObjectTypeConstraint) : base(associatedObjectTypeConstraint)
        {
        }

        /// <summary>
        /// This is used to attach this trigger to an object.
        /// </summary>
        /// <param name="element"></param>
        public void Attach(FrameworkElement element)
        {
            if (element != AssociatedObject)
            {
                if (AssociatedObject != null)
                    throw new InvalidOperationException("Trigger cannot be applied more than once.");

                AssociatedObjectInternal = element;
            }
        }

        /// <summary>
        /// This invokes the action
        /// </summary>
        /// <param name="parameter"></param>
        internal void CallInvoke(object parameter)
        {
            if (IsEnabled)
            {
                Invoke(parameter);
            }
        }

        /// <summary>
        /// This is used to detach the action from an object
        /// </summary>
        public void Detach()
        {
            AssociatedObjectInternal = null;
        }

        /// <summary>
        /// Invoke method - must be overridden
        /// </summary>
        /// <param name="parameter"></param>
        protected abstract void Invoke(object parameter);
        
        /// <summary>
        /// Associated object
        /// </summary>
        protected FrameworkElement AssociatedObject
        {
            get { return AssociatedObjectInternal; }
        }
    }

    /// <summary>
    /// Typesafe trigger action
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TriggerAction<T> : TriggerAction where T : FrameworkElement
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected TriggerAction() : base(typeof(T))
        {
        }

        /// <summary>
        /// Associated object
        /// </summary>
        protected new T AssociatedObject
        {
            get { return (T) base.AssociatedObject; }
        }
    }


}
