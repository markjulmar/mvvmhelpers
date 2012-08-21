using Julmar.Windows.Interactivity;
using Windows.UI.Xaml;

namespace JulMar.Windows.Interactivity
{
    public class DataTrigger : PropertyChangedTrigger
    {
        public static readonly DependencyProperty ComparisonProperty = DependencyProperty.Register("Comparison",
            typeof(ComparisonConditionType), typeof(DataTrigger), new PropertyMetadata(ComparisonConditionType.Equal, OnComparisonChanged));
        
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

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",
            typeof(object), typeof(DataTrigger), new PropertyMetadata(null, OnValueChanged));

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

        private bool Compare()
        {
            return ((base.AssociatedObject != null) && 
                ComparisonLogic.EvaluateImpl(base.Binding, this.Comparison, this.Value));
        }

        protected override void EvaluateBindingChange(DependencyPropertyChangedEventArgs args)
        {
            if (this.Compare())
            {
                base.InvokeActions(args);
            }
        }

        private static void OnComparisonChanged(object dpo, DependencyPropertyChangedEventArgs e)
        {
            ((DataTrigger)dpo).EvaluateBindingChange(e);
        }

        private static void OnValueChanged(object dpo, DependencyPropertyChangedEventArgs e)
        {
            ((DataTrigger)dpo).EvaluateBindingChange(e);
        }
    }
}
