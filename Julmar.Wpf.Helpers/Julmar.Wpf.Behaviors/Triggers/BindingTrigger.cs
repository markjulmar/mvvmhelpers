using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace JulMar.Windows.Interactivity
{
    /// <summary>
    /// This triggers off a binding expression.  It allows a designer to run a set of actions using a MVVM property as the trigger.
    /// As a side effect it also allows the Value to be data bound so you can compare two ViewModel properties for equality and then run 
    /// a set of actions when the condition is matched.
    /// </summary>
    public class BindingTrigger : TriggerBase<UIElement>
    {
        private BindingBase _binding;
        private static readonly DependencyProperty InternalBindingValueProperty = DependencyProperty.Register("_InternalBindingValue", 
            typeof (object), typeof (BindingTrigger), new PropertyMetadata(OnValueChanged));

        /// <summary>
        /// Binding declaration of the conditional 
        /// </summary>
        public BindingBase Binding
        {
            get { return _binding; }
            set { _binding = value;BindingOperations.SetBinding(this, InternalBindingValueProperty, Binding); }
        }

        /// <summary>
        /// Value to test against
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(BindingTrigger), new UIPropertyMetadata(null, OnValueChanged));

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
            ((BindingTrigger) dpo).CheckValue();
        }

        /// <summary>
        /// This compares a changed value
        /// </summary>
        private void CheckValue()
        {
            object newValue = GetValue(InternalBindingValueProperty);
            object referenceValue = this.Value;

            // Simple object test
            if (newValue == null && referenceValue == null ||
                ReferenceEquals(newValue, referenceValue) ||
                Equals(newValue, referenceValue))
            {
                this.InvokeActions(null);
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
                    this.InvokeActions(null);
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
                        this.InvokeActions(null);
                        return;
                    }
                }
            }
        }
    }
}
