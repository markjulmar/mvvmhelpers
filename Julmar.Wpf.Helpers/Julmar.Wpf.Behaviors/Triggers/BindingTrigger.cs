using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Microsoft.Xaml.Behaviors;

namespace JulMar.Windows.Interactivity
{
    /// <summary>
    /// This triggers off a binding expression.  It allows a designer to run a set of actions using a MVVM property as the trigger.
    /// As a side effect it also allows the Value to be data bound so you can compare two ViewModel properties for equality and then run 
    /// a set of actions when the condition is matched.
    /// </summary>
    public class BindingTrigger : TriggerBase<UIElement>
    {
        // These are used to bind to a property for comparison
        private BindingBase _binding;

        private static readonly DependencyProperty InternalBindingValueProperty =
            DependencyProperty.Register(
                "_InternalBindingValue",
                typeof(object),
                typeof(BindingTrigger),
                new PropertyMetadata(OnSourceBindingValueChanged));

        /// <summary>
        /// Binding declaration of the conditional 
        /// </summary>
        public BindingBase Binding
        {
            get { return _binding; }
            set
            {
                _binding = value;
                BindingOperations.SetBinding(this, InternalBindingValueProperty, Binding);
            }
        }

        /// <summary>
        /// Backing storage for the AlwaysRunOnChanges property.
        /// </summary>
        public static readonly DependencyProperty AlwaysRunOnChangesProperty =
            DependencyProperty.Register(
                "AlwaysRunOnChangesProperty",
                typeof(bool),
                typeof(BindingTrigger),
                new PropertyMetadata(false));

        /// <summary>
        /// This is used to always raise our invoke when
        /// the source binding changes.
        /// </summary>
        public bool AlwaysRunOnChanges
        {
            get
            {
                return (bool)this.GetValue(AlwaysRunOnChangesProperty);
            }

            set
            {
                SetValue(AlwaysRunOnChangesProperty, value);
            }
        }

        /// <summary>
        /// Value to test against
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(object),
            typeof(BindingTrigger),
            new UIPropertyMetadata(null, OnValueChanged));

        /// <summary>
        /// Gets or sets the Value property.
        /// </summary>
        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// This is called when the DP backed by the binding changes
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="e"></param>
        private static void OnValueChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            ((BindingTrigger) dpo).CheckValue(false);
        }

        /// <summary>
        /// This is called when source binding value changes.
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="e"></param>
        private static void OnSourceBindingValueChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            ((BindingTrigger)dpo).CheckValue(true);
        }

        /// <summary>
        /// This compares a changed value
        /// </summary>
        private void CheckValue(bool isSourceChange)
        {
            // Get the new value (SOURCE)
            object newValue = GetValue(InternalBindingValueProperty);
            // Get our comparison value
            object referenceValue = this.Value;

            // If this is a source change, and we always notify on
            // every property change, then send the notification.
            if (isSourceChange && AlwaysRunOnChanges)
            {
                this.InvokeActions(newValue);
                return;
            }

            // Simple object test
            if (newValue == null && referenceValue == null ||
                ReferenceEquals(newValue, referenceValue) ||
                Equals(newValue, referenceValue))
            {
                this.InvokeActions(newValue);
                return;
            }

            // Skip if one is not assigned and the other is.
            if (newValue == null || referenceValue == null)
                return;

            // If our value is a string, see if it matches.
            if (referenceValue.GetType() == typeof(string))
            {
                if (String.Compare((string)referenceValue, newValue.ToString()) == 0)
                {
                    this.InvokeActions(newValue);
                    return;
                }
            }

            // Try a type converter as a last resort.
            TypeConverter tc = TypeDescriptor.GetConverter(referenceValue.GetType());
            if (tc != null)
            {
                if (tc.CanConvertFrom(newValue.GetType()))
                {
                    if (tc.ConvertFrom(newValue) == referenceValue)
                    {
                        this.InvokeActions(newValue);
                        return;
                    }
                }
            }
        }
    }
}
