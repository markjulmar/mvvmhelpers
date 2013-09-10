using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JulMar.Windows.Interactivity
{
    /// <summary>
    /// Shifts focus to some control
    /// </summary>
    public sealed class SetFocusAction : DependencyObject, IAction
    {
        /// <summary>
        /// Backing storage for Target property
        /// </summary>
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof (Control), typeof (SetFocusAction),
                new PropertyMetadata(null));

        /// <summary>
        /// The focus target.
        /// </summary>
        public Control Target
        {
            get { return (Control)base.GetValue(TargetProperty); }
            set { base.SetValue(TargetProperty, value);}
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="sender">The <see cref="T:System.Object"/> that is passed to the action by the behavior. Generally this is <seealso cref="P:Microsoft.Xaml.Interactivity.IBehavior.AssociatedObject"/> or a target object.</param><param name="parameter">The value of this parameter is determined by the caller.</param>
        /// <remarks>
        /// An example of parameter usage is EventTriggerBehavior, which passes the EventArgs as a parameter to its actions.
        /// </remarks>
        /// <returns>
        /// Returns the result of the action.
        /// </returns>
        public object Execute(object sender, object parameter)
        {
            if (Target != null)
            {
                return Target.Focus(FocusState.Programmatic);
            }
            return false;
        }
    }
}
