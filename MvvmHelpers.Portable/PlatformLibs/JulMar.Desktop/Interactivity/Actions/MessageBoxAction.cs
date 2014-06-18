using System.Windows;
using System.Windows.Interactivity;

namespace JulMar.Interactivity.Actions
{
    /// <summary>
    /// Displays a MessageBox on the screen.
    /// </summary>
    public class MessageBoxAction : TriggerAction<FrameworkElement>
    {
        /// <summary>
        /// Backing storage for the title
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof (string), typeof (MessageBoxAction), new PropertyMetadata(default(string)));

        /// <summary>
        /// The title to use
        /// </summary>
        public string Title
        {
            get { return (string) this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// The backing storage for the message box image
        /// </summary>
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof (MessageBoxImage), typeof (MessageBoxAction), new PropertyMetadata(MessageBoxImage.None));

        /// <summary>
        /// The MessageBoxImage to use
        /// </summary>
        public MessageBoxImage Image
        {
            get { return (MessageBoxImage) this.GetValue(ImageProperty); }
            set { this.SetValue(ImageProperty, value); }
        }

        /// <summary>
        /// Backing storage for the text
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof (string), typeof (MessageBoxAction), new PropertyMetadata(default(string)));

        /// <summary>
        /// Text to display
        /// </summary>
        public string Text
        {
            get { return (string) this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Invokes the action.
        /// </summary>
        /// <param name="parameter">The parameter to the action. If the action does not require a parameter, the parameter may be set to a null reference.</param>
        protected override void Invoke(object parameter)
        {
            if (!string.IsNullOrEmpty(this.Text))
                MessageBox.Show(this.Text, this.Title, MessageBoxButton.OK, this.Image, MessageBoxResult.None);
        }
    }
}