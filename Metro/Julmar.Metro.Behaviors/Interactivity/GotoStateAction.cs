using JulMar.Windows.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Julmar.Metro.Behaviors.Interactivity
{
    public class GoToStateAction : TargetedTriggerAction<Control>
    {
        public static readonly DependencyProperty StateNameProperty = DependencyProperty.Register("StateName", typeof(string), typeof(GoToStateAction), new PropertyMetadata(string.Empty));

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

        public static readonly DependencyProperty UseTransitionsProperty = DependencyProperty.Register("UseTransitions", typeof(bool), typeof(GoToStateAction), new PropertyMetadata(true));

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

        protected override void Invoke(object parameter)
        {
            if (base.Target != null)
            {
                VisualStateManager.GoToState(Target, this.StateName, UseTransitions);
            }
        }
    }

 

}
