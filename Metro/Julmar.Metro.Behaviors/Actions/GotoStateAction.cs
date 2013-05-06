using System.Windows.Interactivity;
using JulMar.Windows.Interactivity.Internal;
using Windows.UI.Xaml;

namespace JulMar.Windows.Interactivity
{
    /// <summary>
    /// This method provides a VSM change based on a trigger action
    /// </summary>
    public class GoToStateAction : TargetedTriggerAction<FrameworkElement>
    {
        /// <summary>
        /// Backing storage for the state name to transition to
        /// </summary>
        public static readonly DependencyProperty StateNameProperty = DependencyProperty.Register("StateName", typeof(string), typeof(GoToStateAction), new PropertyMetadata(string.Empty));

        /// <summary>
        /// New state name to transition to
        /// </summary>
        public string StateName
        {
            get
            {
                return (string)base.GetValue(StateNameProperty);
            }
            set
            {
                base.SetValue(StateNameProperty, value);
            }
        }

        /// <summary>
        /// Backing storage for the UseTransition property
        /// </summary>
        public static readonly DependencyProperty UseTransitionsProperty = DependencyProperty.Register("UseTransitions", typeof(bool), typeof(GoToStateAction), new PropertyMetadata(true));

        /// <summary>
        /// True to use transitions when changing states
        /// </summary>
        public bool UseTransitions
        {
            get
            {
                return (bool)base.GetValue(UseTransitionsProperty);
            }
            set
            {
                base.SetValue(UseTransitionsProperty, value);
            }
        }

        /// <summary>
        /// This method invokes our VSM state
        /// </summary>
        /// <param name="parameter"></param>
        protected override void Invoke(object parameter)
        {
            if (base.Target != null)
            {
                string stateName = this.StateName;
                if (string.IsNullOrEmpty(stateName) && parameter is string)
                    stateName = parameter.ToString();

                // Locate the nearest state group
                var stateControl = VisualStateUtilities.FindNearestStatefulControl(base.Target);
                if (stateControl != null)
                    VisualStateUtilities.GoToState(stateControl, stateName, UseTransitions);
            }
        }
    }

 

}
