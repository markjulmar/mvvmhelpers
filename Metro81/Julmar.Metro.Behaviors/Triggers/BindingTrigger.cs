using Windows.UI.Xaml;

namespace JulMar.Windows.Interactivity
{
    /// <summary>
    /// This provides a Blend trigger based on a Data Binding expression with no comparison
    /// </summary>
    public class BindingTrigger : PropertyChangedTrigger
    {
        /// <summary>
        /// This method is called each time our source binding changes
        /// </summary>
        /// <param name="args"></param>
        protected override void EvaluateBindingChange(DependencyPropertyChangedEventArgs args)
        {
            base.InvokeActions(args.NewValue);
        }
    }
}
