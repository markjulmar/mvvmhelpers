using System.Windows;

using Microsoft.Expression.Interactivity;
using Microsoft.Expression.Interactivity.Core;

namespace JulMar.Interactivity.Actions
{
    /// <summary>
    /// A GotoStateAction that can switch based on the supplied parameter
    /// </summary>
    public class GotoVisualStateAction : GoToStateAction
    {
        FrameworkElement _stateTarget;

        /// <summary>
        /// Called when the target changes. If the TargetName property isn't set, this action has custom behavior.
        /// </summary>
        /// <param name="oldTarget"/><param name="newTarget"/><exception cref="T:System.InvalidOperationException">Could not locate an appropriate FrameworkElement with states.</exception>
        protected override void OnTargetChanged(FrameworkElement oldTarget, FrameworkElement newTarget)
        {
            base.OnTargetChanged(oldTarget, newTarget);
            if (string.IsNullOrEmpty(this.TargetName)
                && this.ReadLocalValue(TargetObjectProperty) == DependencyProperty.UnsetValue)
            {
                FrameworkElement resolvedControl;
                if (VisualStateUtilities.TryFindNearestStatefulControl(
                    this.AssociatedObject as FrameworkElement,
                    out resolvedControl) && resolvedControl != null) 
                    this._stateTarget = resolvedControl;
                else
                    this._stateTarget = this.Target;
            }
        }

        /// <summary>
        /// This method is called when some criteria is met and the action is invoked.
        /// </summary>
        /// <param name="parameter"/><exception cref="T:System.InvalidOperationException">Could not change the target to the specified StateName.</exception>
        protected override void Invoke(object parameter)
        {
            if (this.AssociatedObject != null && this._stateTarget != null)
            {
                var state = !string.IsNullOrWhiteSpace(this.StateName)
                                ? this.StateName
                                : parameter != null ? parameter.ToString() : "";
                if (!string.IsNullOrEmpty(state))
                    VisualStateUtilities.GoToState(this._stateTarget, state, this.UseTransitions);
            }            
        }
    }
}
