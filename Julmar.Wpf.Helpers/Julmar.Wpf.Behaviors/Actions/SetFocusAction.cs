using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace JulMar.Windows.Actions
{
    /// <summary>
    /// This action sets focus to the associated element.
    /// </summary>
    public class SetFocusAction : TriggerAction<FrameworkElement>
    {
        /// <summary>
        /// Dependency Property backing the Target property.
        /// </summary>
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(FrameworkElement), typeof(SetFocusAction), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// This property allows you to set the focus target independent of where this
        /// action is applied - so you can apply the trigger/action to the Window and then
        /// push focus to a child element as an example.
        /// </summary>
        public FrameworkElement Target
        {
            get { return (FrameworkElement) base.GetValue(TargetProperty); }    
            set { base.SetValue(TargetProperty, value);}
        }

        /// <summary>
        /// Invokes the action.
        /// </summary>
        /// <param name="parameter">The parameter to the action. If the Action does not require a parameter, the parameter may be set to a null reference.</param>
        protected override void Invoke(object parameter)
        {
            var element = Target ?? AssociatedObject;
            if (element != null)
            {
                // If unable to directly set focus, then attempt to set *logical* focus
                // to our element so that when/if focus returns to this focus scope we will have focus.
                if (!element.Focus())
                {
                    var fs = FocusManager.GetFocusScope(element);
                    FocusManager.SetFocusedElement(fs, element);
                }
            }
        }
    }
}