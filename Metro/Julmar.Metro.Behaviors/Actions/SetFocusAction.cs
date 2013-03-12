using System.Windows.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JulMar.Windows.Interactivity.Actions
{
    /// <summary>
    /// Shifts focus to some control
    /// </summary>
    public sealed class SetFocusAction : TargetedTriggerAction<Control>
    {
        /// <summary>
        /// Invoke method - must be overridden
        /// </summary>
        /// <param name="parameter">Unused</param>
        protected override void Invoke(object parameter)
        {
            if (Target != null)
            {
                Target.Focus(FocusState.Programmatic);
            }
        }
    }
}
