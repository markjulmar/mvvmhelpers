using System;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Markup;
using JulMar.Windows.Interactivity.Internal;
using Microsoft.Xaml.Interactions.Core;
using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;

namespace JulMar.Windows.Interactivity
{
    /// <summary>
    /// This provides a Blend trigger based on a Data Binding expression with a comparison value.
    /// This differs from the built-in DataTriggerBehavior in two ways:
    /// 1. If no comparison value is provided, the action is always executed when the binding changes
    /// 2. The binding value is passed as the parameter to the action.
    /// </summary>
    [ContentProperty(Name = "Actions")]
    public sealed class BindingTriggerBehavior : DependencyObject, IBehavior
    {
        /// <summary>
        /// Backing storage for the Binding
        /// </summary>
        public static readonly DependencyProperty BindingProperty = DependencyProperty.Register("Binding",
                        typeof(object), typeof(BindingTriggerBehavior), new PropertyMetadata(null, OnBindingChanged));

        /// <summary>
        /// Binding to monitor
        /// </summary>
        public object Binding
        {
            get
            {
                return base.GetValue(BindingProperty);
            }
            set
            {
                base.SetValue(BindingProperty, value);
            }
        }

        /// <summary>
        /// Backing storage for the comparison condition
        /// </summary>
        public static readonly DependencyProperty ComparisonConditionProperty =
            DependencyProperty.Register("ComparisonCondition", typeof(ComparisonConditionType), typeof(BindingTriggerBehavior),
                new PropertyMetadata(ComparisonConditionType.Equal, OnBindingChanged));

        /// <summary>
        /// Comparison condition (==, >, >=, etc.)
        /// </summary>
        public ComparisonConditionType ComparisonCondition 
        {
            get { return (ComparisonConditionType) base.GetValue(ComparisonConditionProperty); }
            set { base.SetValue(ComparisonConditionProperty, value); }
        }
        
        /// <summary>
        /// Backing storage for the compare value
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(BindingTriggerBehavior),
                new PropertyMetadata(null, OnBindingChanged));

        /// <summary>
        /// Comparison value
        /// </summary>
        public object Value
        {
            get { return base.GetValue(ValueProperty); }
            set { base.SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Backing storage for Actions collection
        /// </summary>
        public static readonly DependencyProperty ActionsProperty =
            DependencyProperty.Register("Actions", typeof(ActionCollection), typeof(TimerTriggerBehavior), new PropertyMetadata(null));

        /// <summary>
        /// Actions collection
        /// </summary>
        public ActionCollection Actions
        {
            get
            {
                ActionCollection actions = (ActionCollection)base.GetValue(ActionsProperty);
                if (actions == null)
                {
                    actions = new ActionCollection();
                    base.SetValue(ActionsProperty, actions);
                }
                return actions;
            }
        }

        /// <summary>
        /// Handler method for the binding change
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="e"></param>
        private static void OnBindingChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            ((BindingTriggerBehavior)dpo).EvaluateBindingChange(e);
        }

        /// <summary>
        /// This method is called each time our source binding changes
        /// </summary>
        /// <param name="e"></param>
        private void EvaluateBindingChange(DependencyPropertyChangedEventArgs e)
        {
            DataBindingHelper.EnsureDataBindingOnActionsUpToDate(this.Actions);

            bool hasValue = (base.ReadLocalValue(ValueProperty) != DependencyProperty.UnsetValue);
            if (!hasValue || ComparisonLogic.Evaluate(Binding, ComparisonCondition, Value))
            {
                // Pass the result of the binding
                Interaction.ExecuteActions(AssociatedObject, Actions, Binding);
            }
        }

        /// <summary>
        /// Attaches to the specified object.
        /// </summary>
        /// <param name="associatedObject">The <see cref="T:Windows.UI.Xaml.DependencyObject"/> to which the <seealso cref="T:Microsoft.Xaml.Interactivity.IBehavior"/> will be attached.</param>
        public void Attach(DependencyObject associatedObject)
        {
            if ((associatedObject != AssociatedObject) && !DesignMode.DesignModeEnabled)
            {
                if (AssociatedObject != null)
                    throw new InvalidOperationException("Cannot attach behavior multiple times.");

                AssociatedObject = associatedObject;
                
            }
        }

        /// <summary>
        /// Detaches this instance from its associated object.
        /// </summary>
        public void Detach()
        {
            AssociatedObject = null;
        }

        /// <summary>
        /// Gets the <see cref="T:Windows.UI.Xaml.DependencyObject"/> to which the <seealso cref="T:Microsoft.Xaml.Interactivity.IBehavior"/> is attached.
        /// </summary>
        public DependencyObject AssociatedObject { get; private set; }
    }
}
