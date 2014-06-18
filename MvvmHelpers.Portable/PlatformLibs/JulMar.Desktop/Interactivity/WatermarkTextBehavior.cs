using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace JulMar.Interactivity
{
    /// <summary>
    /// This behavior associates a watermark onto a TextBox indicating what the user should
    /// provide as input.
    /// </summary>
    public class WatermarkTextBoxBehavior : Behavior<TextBox>
    {
        /// <summary>
        /// The watermark text
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof (string), typeof (WatermarkTextBoxBehavior),
                                        new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// Readonly property used to style the TextBox when the watermark is present
        /// </summary>
        static readonly DependencyPropertyKey IsWatermarkedPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("IsWatermarked", typeof(bool), typeof(WatermarkTextBoxBehavior), 
                                        new FrameworkPropertyMetadata(false));

        /// <summary>
        /// This readonly property is applied to the TextBox and indicates whether the watermark
        /// is currently being displayed.  It allows a style to change the visual appearanve of the
        /// TextBox.
        /// </summary>
        public static readonly DependencyProperty IsWatermarkedProperty = IsWatermarkedPropertyKey.DependencyProperty;

        /// <summary>
        /// Retrieves the current watermarked state of the TextBox.
        /// </summary>
        /// <param name="tb"></param>
        /// <returns></returns>
        public static bool GetIsWatermarked(TextBox tb)
        {
            return (bool) tb.GetValue(IsWatermarkedProperty);
        }

        /// <summary>
        /// Retrieves the current watermarked state of the TextBox.
        /// </summary>
        private bool IsWatermarked
        {
            get { return (bool) this.AssociatedObject.GetValue(IsWatermarkedProperty); }
            set { this.AssociatedObject.SetValue(IsWatermarkedPropertyKey, value);}
        }

        /// <summary>
        /// The watermark text
        /// </summary>
        [Category("Appearance")]
        public string Text
        {
            get { return (string) base.GetValue(TextProperty); }
            set { base.SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.GotFocus += this.OnGotFocus;
            this.AssociatedObject.LostFocus += this.OnLostFocus;
            this.AssociatedObject.TextChanged += this.OnTextChanged;

            this.OnLostFocus(null, null);
        }

        /// <summary>
        /// This method is called when the text for the TextBox is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this._isChangingText && !this.AssociatedObject.IsFocused)
            {
                this.OnLostFocus(null, null);
            }
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>
        /// Override this to unhook functionality from the AssociatedObject.
        /// </remarks>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.GotFocus -= this.OnGotFocus;
            this.AssociatedObject.LostFocus -= this.OnLostFocus;
        }

        /// <summary>
        /// This method is called when the textbox gains focus.  It removes the watermark.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (this.IsWatermarked)
                this.ChangeText(string.Empty);

            this.IsWatermarked = false;
        }

        /// <summary>
        /// This method is called when focus is lost from the TextBox.  It puts the watermark
        /// into place if no text is in the textbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.AssociatedObject.Text))
            {
                this.ChangeText(this.Text);
                this.IsWatermarked = true;
            }
            else
                this.IsWatermarked = false;
        }

        private bool _isChangingText;

        /// <summary>
        /// This method is used to change the text.
        /// </summary>
        /// <param name="newText">New string to assign to TextBox</param>
        private void ChangeText(string newText)
        {
            this._isChangingText = true;
            this.AssociatedObject.Text = newText;
            this._isChangingText = false;
        }
    }
}
