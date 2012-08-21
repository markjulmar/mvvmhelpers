using Windows.UI.Xaml;

namespace JulMar.Windows.Interactivity
{
    public class PropertyChangedTrigger : TriggerBase<FrameworkElement>
    {
        public static readonly DependencyProperty BindingProperty = DependencyProperty.Register("Binding", 
                        typeof(object), typeof(PropertyChangedTrigger), new PropertyMetadata(null, OnBindingChanged));

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

        protected virtual void EvaluateBindingChange(DependencyPropertyChangedEventArgs e)
        {
            base.InvokeActions(e);
        }

        /// <summary>
        /// Override called when behavior is attached
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            base.PreviewInvoke += this.OnPreviewInvoke;
        }

        private static void OnBindingChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            ((PropertyChangedTrigger)dpo).EvaluateBindingChange(e);
        }

        /// <summary>
        /// Override called when behavior is detached
        /// </summary>
        protected override void OnDetaching()
        {
            base.PreviewInvoke -= this.OnPreviewInvoke;
            this.OnDetaching();
        }

        private void OnPreviewInvoke(object sender, PreviewInvokeEventArgs e)
        {
            DataBindingHelper.EnsureDataBindingOnActionsUpToDate(this);
        }
    }
}
