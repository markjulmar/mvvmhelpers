using System.Windows.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JulMar.Windows.Interactivity.Interactivity
{
    /// <summary>
    /// Attached behavior to simulate the UpdateSourceTrigger feature of other XAML platforms.
    /// DataBind to "BindableText" instead of Text and set the "Enabled" property to "true".
    /// This works by hooking the TextChanged property of the associated TextBox and then pushing the value
    /// to a new data-bindable property each time the value is changed.
    /// </summary>
    public class UpdateSourceTriggerBehavior : Behavior<TextBox>
    {
        /// <summary>
        /// BindableText property key
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), 
                typeof(UpdateSourceTriggerBehavior), new PropertyMetadata(default(string), OnBindableTextChanged));

        /// <summary>
        /// Property wrapper for Text
        /// </summary>
        public string Text
        {
            get { return (string) base.GetValue(TextProperty); }
            set { base.SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Override called when behavior is attached
        /// </summary>
        protected override void OnAttached()
        {
            AssociatedObject.TextChanged += OnTextChanged;
        }

        /// <summary>
        /// Override called when behavior is detached
        /// </summary>
        protected override void OnDetaching()
        {
            AssociatedObject.TextChanged -= OnTextChanged;
        }

        /// <summary>
        /// This is called to change the TextBox.Text value from a ViewModel's bound version
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnBindableTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateSourceTriggerBehavior ustb = (UpdateSourceTriggerBehavior) sender;
            if (ustb != null && ustb.AssociatedObject != null)
            {
               string newValue = e.NewValue as string;
               ustb.AssociatedObject.Text = newValue ?? string.Empty;
            }
        }

        /// <summary>
        /// Called when the TextBox.TextChanged event is raised. This copies
        /// the current text from the TextBox into the BindableText property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Text = AssociatedObject.Text;
        }
    }
}
