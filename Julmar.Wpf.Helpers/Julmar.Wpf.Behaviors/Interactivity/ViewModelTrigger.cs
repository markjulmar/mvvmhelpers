using System.Windows;
using System.Windows.Interactivity;
using JulMar.Windows.Interfaces;

namespace JulMar.Windows.Interactivity
{
    /// <summary>
    /// This is a Blend trigger that binds to a IViewModelTrigger and invokes actions when it 
    /// is raised by the ViewModel.  This allows the VM to trigger behaviors in the View easily.
    /// </summary>
    public class ViewModelTrigger : TriggerBase<UIElement>
    {
        /// <summary>
        /// The DependencyProperty used to hold the IViewModelTrigger.
        /// </summary>
        public static readonly DependencyProperty TriggerProperty = DependencyProperty.Register("Trigger", typeof(IViewModelTrigger), typeof(ViewModelTrigger), new FrameworkPropertyMetadata(null, OnTriggerChanged));

        /// <summary> 
        /// IViewModelTrigger to hook into.
        /// </summary>
        public IViewModelTrigger Trigger
        {
            get { return (IViewModelTrigger)GetValue(TriggerProperty); }
            set { SetValue(TriggerProperty, value); }
        }

        /// <summary>
        /// Change handler for IViewModelTrigger.
        /// </summary>
        /// <param name="dpo">VMTrigger object</param>
        /// <param name="e">EventArgs</param>
        private static void OnTriggerChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            ViewModelTrigger vmt = (ViewModelTrigger) dpo;

            IViewModelTrigger ivm = e.OldValue as IViewModelTrigger;
            if (ivm != null)
                ivm.Execute -= vmt.TriggerRaised;
            ivm = e.NewValue as IViewModelTrigger;
            if (ivm != null)
                ivm.Execute += vmt.TriggerRaised;
        }

        /// <summary>
        /// This is called when the trigger occurs.
        /// </summary>
        /// <param name="parameter"></param>
        private void TriggerRaised(object parameter)
        {
            InvokeActions(parameter);
        }
    }
}
