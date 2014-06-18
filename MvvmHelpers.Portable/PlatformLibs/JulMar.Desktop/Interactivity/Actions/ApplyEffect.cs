using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media.Effects;

namespace JulMar.Interactivity.Actions
{
    /// <summary>
    /// This action applies a specific media effect to the element.  It is used as a Blend
    /// Action and will apply/remove an effect based on the Effect property (leave it null
    /// to clear any effect on the target element).
    /// </summary>
    public class ApplyEffect : TriggerAction<FrameworkElement>
    {
        /// <summary>
        /// Effect property
        /// </summary>
        public static readonly DependencyProperty EffectProperty = DependencyProperty.Register("Effect", typeof(Effect), typeof(ApplyEffect));

        /// <summary>
        /// The effect to apply
        /// </summary>
        public Effect Effect
        {
            get { return (Effect) GetValue(EffectProperty); }
            set { SetValue(EffectProperty, value);}
        }

        /// <summary>
        /// Called to apply the effect when the trigger is active.
        /// </summary>
        /// <param name="parameter">Can pass in specific effect if desired, overrides Property</param>
        protected override void Invoke(object parameter)
        {
            var effect = parameter as Effect ?? Effect;

            if (AssociatedObject != null)
            {
                if (effect != null && AssociatedObject.Effect == null)
                    AssociatedObject.Effect = effect;
                else if (effect == null && AssociatedObject.Effect != null)
                    AssociatedObject.Effect = null;
            }
        }
    }
}
