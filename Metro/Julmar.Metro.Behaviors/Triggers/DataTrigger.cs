using Windows.UI.Xaml;

namespace JulMar.Windows.Interactivity
{
    /// <summary>
    /// This provides a Blend trigger based on a Data Binding expression + comparison
    /// </summary>
    public class DataTrigger : PropertyChangedTrigger
    {
        /// <summary>
        /// Backing storage for the comparison type
        /// </summary>
        public static readonly DependencyProperty ComparisonProperty = DependencyProperty.Register("Comparison",
            typeof(ComparisonConditionType), typeof(DataTrigger), new PropertyMetadata(ComparisonConditionType.Equal, OnComparisonChanged));
        
        /// <summary>
        /// Comparison condition to perform
        /// </summary>
        public ComparisonConditionType Comparison
        {
            get
            {
                return (ComparisonConditionType)base.GetValue(ComparisonProperty);
            }
            set
            {
                base.SetValue(ComparisonProperty, value);
            }
        }

        /// <summary>
        /// Backing storage for the value to compare against
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",
            typeof(object), typeof(DataTrigger), new PropertyMetadata(null, OnValueChanged));

        /// <summary>
        /// Value to compare to
        /// </summary>
        public object Value
        {
            get
            {
                return base.GetValue(ValueProperty);
            }
            set
            {
                base.SetValue(ValueProperty, value);
            }
        }

        /// <summary>
        /// Method which performs the comparison
        /// </summary>
        /// <returns>True/False for equality</returns>
        private bool Compare()
        {
            return ((base.AssociatedObject != null) && 
                ComparisonLogic.EvaluateImpl(base.Binding, this.Comparison, this.Value));
        }

        /// <summary>
        /// This method is called each time our source binding changes
        /// </summary>
        /// <param name="args"></param>
        protected override void EvaluateBindingChange(DependencyPropertyChangedEventArgs args)
        {
            if (this.Compare())
            {
                base.InvokeActions(args);
            }
        }

        /// <summary>
        /// Change handler for the Comparison condition
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="e"></param>
        private static void OnComparisonChanged(object dpo, DependencyPropertyChangedEventArgs e)
        {
            ((DataTrigger)dpo).EvaluateBindingChange(e);
        }

        /// <summary>
        /// Change handler for the comparison value
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="e"></param>
        private static void OnValueChanged(object dpo, DependencyPropertyChangedEventArgs e)
        {
            ((DataTrigger)dpo).EvaluateBindingChange(e);
        }
    }
}
