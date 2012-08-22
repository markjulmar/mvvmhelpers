using System.Windows.Interactivity;
using JulMar.Windows.Interactivity.Internal;
using Windows.UI.Xaml;

namespace JulMar.Windows.Interactivity
{
    /// <summary>
    /// Trigger which monitors a specific binding change
    /// </summary>
    public class PropertyChangedTrigger : TriggerBase<FrameworkElement>
    {
        /// <summary>
        /// Backing storage for the Binding
        /// </summary>
        public static readonly DependencyProperty BindingProperty = DependencyProperty.Register("Binding", 
                        typeof(object), typeof(PropertyChangedTrigger), new PropertyMetadata(null, OnBindingChanged));

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
        /// Called when the binding changes
        /// </summary>
        /// <param name="e"></param>
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

        /// <summary>
        /// Handler method for the binding change
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// This is raised just before the invoke and ensures the bindings are all valid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPreviewInvoke(object sender, PreviewInvokeEventArgs e)
        {
            DataBindingHelper.EnsureDataBindingOnActionsUpToDate(this);
        }
    }
}
