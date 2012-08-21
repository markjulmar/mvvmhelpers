using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace System.Windows.Interactivity
{
    /// <summary>
    /// This is the base class for all trigger types.
    /// </summary>
    [ContentProperty(Name = "Actions")]
    public abstract class TriggerBase : AttachedObject
    {
        /// <summary>
        /// Backing storage for Actions collection
        /// </summary>
        public static readonly DependencyProperty ActionsProperty = DependencyProperty.Register("Actions", 
            typeof(TriggerActionCollection), typeof(TriggerBase), null);

        /// <summary>
        /// Event raised prior to Invoke
        /// </summary>
        public event EventHandler<PreviewInvokeEventArgs> PreviewInvoke;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="associatedObjectTypeConstraint"></param>
        internal TriggerBase(Type associatedObjectTypeConstraint) : base(associatedObjectTypeConstraint)
        {
            base.SetValue(ActionsProperty, new TriggerActionCollection());
        }

        /// <summary>
        /// Attach trigger to a specific object
        /// </summary>
        /// <param name="element"></param>
        public void Attach(FrameworkElement element)
        {
            if (element != AssociatedObjectInternal)
            {
                if (AssociatedObjectInternal != null)
                    throw new InvalidOperationException("Trigger cannot be applied more than once.");

                AssociatedObjectInternal = element;
                Actions.Attach(element);
            }
        }

        /// <summary>
        /// Detach trigger from an element
        /// </summary>
        public void Detach()
        {
            AssociatedObjectInternal = null;
            this.Actions.Detach();
        }

        /// <summary>
        /// Method used to invoke the associated actions
        /// </summary>
        /// <param name="parameter"></param>
        protected void InvokeActions(object parameter)
        {
            if (DesignMode.DesignModeEnabled)
                return;

            if (PreviewInvoke != null)
            {
                var e = new PreviewInvokeEventArgs();
                this.PreviewInvoke(this, e);
                if (e.Cancelling)
                    return;
            }

            foreach (TriggerAction action in this.Actions)
            {
                action.CallInvoke(parameter);
            }
        }

        /// <summary>
        /// Collection of actions to invoke
        /// </summary>
        public TriggerActionCollection Actions
        {
            get
            {
                return (TriggerActionCollection) base.GetValue(ActionsProperty);
            }
        }
    }

    /// <summary>
    /// Typesafe trigger
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TriggerBase<T> : TriggerBase where T : FrameworkElement
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected TriggerBase() : base(typeof(T))
        {
        }

        /// <summary>
        /// Typed associated object
        /// </summary>
        protected T AssociatedObject
        {
            get
            {
                return (T)base.AssociatedObjectInternal;
            }
        }
    }
}
