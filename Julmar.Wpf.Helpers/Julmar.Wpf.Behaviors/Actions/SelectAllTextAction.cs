using System.Windows.Controls.Primitives;
using Microsoft.Xaml.Behaviors;

namespace JulMar.Windows.Actions
{
    /// <summary>
    /// Selects all the text in the applied TextBox when the trigger is processed.
    /// </summary>
    public class SelectAllTextAction : TriggerAction<TextBoxBase>
    {
        /// <summary>
        /// Invokes the action.
        /// </summary>
        /// <param name="parameter">The parameter to the action. If the action does not require a parameter, the parameter may be set to a null reference.</param>
        protected override void Invoke(object parameter)
        {
            AssociatedObject.SelectAll();
        }
    }
}
