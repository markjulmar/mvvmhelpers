using System;
using System.ComponentModel;
using Windows.UI.Xaml;

namespace JulMar.Windows.Interactivity
{
    public abstract class TriggerAction : AttachedObject
    {
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled", typeof(bool), typeof(TriggerAction), new PropertyMetadata(true));

        internal TriggerAction(Type associatedObjectTypeConstraint) : base(associatedObjectTypeConstraint)
        {
        }

        public void Attach(FrameworkElement dependencyObject)
        {
            if (dependencyObject != AssociatedObject)
            {
                if (AssociatedObject != null)
                    throw new InvalidOperationException("Trigger cannot be applied more than once.");

                AssociatedObjectInternal = dependencyObject;
            }
        }

        internal void CallInvoke(object parameter)
        {
            if (IsEnabled)
            {
                Invoke(parameter);
            }
        }

        public void Detach()
        {
            AssociatedObjectInternal = null;
        }

        protected abstract void Invoke(object parameter);
        
        protected FrameworkElement AssociatedObject
        {
            get { return AssociatedObjectInternal; }
        }

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
    }

    public abstract class TriggerAction<T> : TriggerAction where T : FrameworkElement
    {
        protected TriggerAction() : base(typeof(T))
        {
        }

        protected new T AssociatedObject
        {
            get { return (T) base.AssociatedObject; }
        }
    }


}
