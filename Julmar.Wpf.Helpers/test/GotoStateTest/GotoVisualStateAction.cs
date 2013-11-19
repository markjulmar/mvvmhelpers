using System.Windows;
using Microsoft.Expression.Interactivity;
using Microsoft.Expression.Interactivity.Core;

namespace VMTriggers
{
    /// <summary>
    /// A GotoStateAction that can switch based on the supplied parameter
    /// </summary>
    public class GotoVisualStateAction : GoToStateAction
    {
        FrameworkElement _stateTarget;

        protected override void OnTargetChanged(FrameworkElement oldTarget, FrameworkElement newTarget)
        {
            base.OnTargetChanged(oldTarget, newTarget);
            if (string.IsNullOrEmpty(this.TargetName)
                && ReadLocalValue(TargetObjectProperty) == DependencyProperty.UnsetValue)
            {
                FrameworkElement resolvedControl;
                if (VisualStateUtilities.TryFindNearestStatefulControl(
                    this.AssociatedObject as FrameworkElement,
                    out resolvedControl) && resolvedControl != null) 
                    _stateTarget = resolvedControl;
                else
                    _stateTarget = this.Target;
            }
        }

        protected override void Invoke(object parameter)
        {
            if (this.AssociatedObject != null && _stateTarget != null)
            {
                var state = !string.IsNullOrWhiteSpace(this.StateName)
                                ? this.StateName
                                : parameter != null ? parameter.ToString() : "";
                if (!string.IsNullOrEmpty(state))
                    VisualStateUtilities.GoToState(_stateTarget, state, this.UseTransitions);
            }            
        }
    }
}
