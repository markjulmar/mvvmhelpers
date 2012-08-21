using System;
using Windows.UI.Xaml;

namespace JulMar.Windows.Interactivity
{
    public abstract class TargetedTriggerAction : TriggerAction
    {
        public static readonly DependencyProperty TargetObjectProperty = DependencyProperty.Register("TargetObject", typeof(object), typeof(TargetedTriggerAction), new PropertyMetadata(null));

        public object TargetObject
        {
            get { return base.GetValue(TargetObjectProperty); }
            set { base.SetValue(TargetObjectProperty, value); }
        }

        protected TargetedTriggerAction(Type associatedObjectTypeConstraint) : base(associatedObjectTypeConstraint)
        {
        }

        protected object Target
        {
            get { return TargetObject ?? AssociatedObject; }
        }
    }

    public abstract class TargetedTriggerAction<T> : TargetedTriggerAction where T : class
    {
        protected TargetedTriggerAction() : base(typeof(T))
        {
        }

        protected new T Target
        {
            get { return (T)base.Target; }
        }
    }


}
