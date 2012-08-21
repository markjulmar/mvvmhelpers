using System;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace JulMar.Windows.Interactivity
{
    [ContentProperty(Name = "Actions")]
    public abstract class TriggerBase : AttachedObject
    {
        public static readonly DependencyProperty ActionsProperty = DependencyProperty.Register("Actions", 
            typeof(TriggerActionCollection), typeof(TriggerBase), null);

        public event EventHandler<PreviewInvokeEventArgs> PreviewInvoke;

        internal TriggerBase(Type associatedObjectTypeConstraint) : base(associatedObjectTypeConstraint)
        {
            base.SetValue(ActionsProperty, new TriggerActionCollection());
        }

        public void Attach(FrameworkElement dependencyObject)
        {
            if (dependencyObject != AssociatedObjectInternal)
            {
                if (AssociatedObjectInternal != null)
                    throw new InvalidOperationException("Trigger cannot be applied more than once.");

                AssociatedObjectInternal = dependencyObject;
                Actions.Attach(dependencyObject);
            }
        }

        public void Detach()
        {
            AssociatedObjectInternal = null;
            this.Actions.Detach();
        }

        protected void InvokeActions(object parameter)
        {
            if (DesignMode.DesignModeEnabled)
                return;

            if (PreviewInvoke != null)
            {
                PreviewInvokeEventArgs e = new PreviewInvokeEventArgs();
                this.PreviewInvoke(this, e);
                if (e.Cancelling)
                {
                    return;
                }
            }

            foreach (TriggerAction action in this.Actions)
            {
                action.CallInvoke(parameter);
            }
        }

        public TriggerActionCollection Actions
        {
            get
            {
                return (TriggerActionCollection) base.GetValue(ActionsProperty);
            }
        }
    }

    public abstract class TriggerBase<T> : TriggerBase where T : FrameworkElement
    {
        protected TriggerBase() : base(typeof(T))
        {
        }

        protected T AssociatedObject
        {
            get
            {
                return (T)base.AssociatedObjectInternal;
            }
        }
    }


}
