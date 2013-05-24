using System.Linq;
using System.Windows.Interactivity;
using JulMar.Windows.Extensions;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace JulMar.Windows.Interactivity.Interactivity
{
    /// <summary>
    /// A behavior to add watermark text to a TextBox.
    /// </summary>
    public sealed class WatermarkTextBehavior : Behavior<TextBox>
    {
        private const string WmtMarker = "\x00A0";
        private Brush _fgColor;

        /// <summary>
        /// Text property - set in XAML
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(WatermarkTextBehavior),
                                                new PropertyMetadata(null, OnTextChanged));

        /// <summary>
        /// Get or set the watermark text.
        /// </summary>
        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Text color property - set in XAML
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(WatermarkTextBehavior),
                            new PropertyMetadata(new SolidColorBrush(Colors.Gray), OnColorChanged));

        /// <summary>
        /// This is called when the watermark color is changed
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="e"></param>
        private static void OnColorChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            WatermarkTextBehavior wtb = (WatermarkTextBehavior) dpo;
            if (wtb.AssociatedObject != null)
            {
                wtb.OnChangeWatermarkColor();
            }
        }

        /// <summary>
        /// Get or set the watermark text color.
        /// </summary>
        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        /// <summary>
        /// This is called when the watermark text is changed.
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="e"></param>
        private static void OnTextChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            WatermarkTextBehavior wtb = (WatermarkTextBehavior) dpo;
            if (wtb.AssociatedObject != null)
                wtb.OnSetWatermarkText();
        }

        /// <summary>
        /// Override called when behavior is attached
        /// </summary>
        protected override void OnAttached()
        {
            // Now hook up the text box.
            AssociatedObject.GotFocus += TbOnGotFocus;
            AssociatedObject.LostFocus += TbOnLostFocus;
            AssociatedObject.TextChanged += TbOnTextChanged;
            AssociatedObject.Loaded += TbOnLoaded;
            
            if (AssociatedObject.Parent != null)
                OnSetWatermarkText();
        }

        /// <summary>
        /// Called when the TextBox is loaded - we associate the watermark
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        private void TbOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            OnSetWatermarkText();
        }

        /// <summary>
        /// Override called when behavior is detached
        /// </summary>
        protected override void OnDetaching()
        {
            AssociatedObject.GotFocus -= TbOnGotFocus;
            AssociatedObject.LostFocus -= TbOnLostFocus;
            AssociatedObject.TextChanged -= TbOnTextChanged;
            OnRemoveWatermarkText();
        }

        /// <summary>
        /// Method to set the watermark text
        /// </summary>
        private void OnSetWatermarkText()
        {
            if (string.IsNullOrEmpty(AssociatedObject.Text))
            {
                Brush fgColor = AssociatedObject.Foreground;
                _fgColor = fgColor != Control.ForegroundProperty.GetMetadata(typeof (TextBox)).DefaultValue ? fgColor : null;
                AssociatedObject.Foreground = this.Foreground;
                AssociatedObject.Text = this.Text + WmtMarker;
            }
        }

        /// <summary>
        /// Used to change the foreground color at runtime.
        /// </summary>
        private void OnChangeWatermarkColor()
        {
            if (!string.IsNullOrEmpty(AssociatedObject.Text))
            {
                if (AssociatedObject.Text == this.Text + WmtMarker)
                {
                    AssociatedObject.Foreground = this.Foreground;
                }
            }
        }

        /// <summary>
        /// This is used to remove the watermark text
        /// </summary>
        private void OnRemoveWatermarkText()
        {
            if (!string.IsNullOrEmpty(AssociatedObject.Text))
            {
                if (AssociatedObject.Text == this.Text + WmtMarker)
                {
                    if (_fgColor != null)
                        AssociatedObject.Foreground = _fgColor;
                    else
                        AssociatedObject.ClearValue(Control.ForegroundProperty);
                    AssociatedObject.Text = "";
                }
            }
        }

        /// <summary>
        /// Called when focus is lost - possibly add in watermark.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        private void TbOnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            OnSetWatermarkText();
        }

        /// <summary>
        /// Called when focus is gained - remove watermark
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        private void TbOnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            OnRemoveWatermarkText();
        }

        /// <summary>
        /// Helper method to determine our associated element, or any CHILD of our parent has focus.
        /// </summary>
        /// <returns></returns>
        private bool IsFocused()
        {
            // If the associated TextBox says it has focus then it probably does.
            if (AssociatedObject.FocusState != FocusState.Unfocused)
                return true;

            // If we are transitioning, then see if a child has focus.
            var focusedElement = FocusManager.GetFocusedElement() as DependencyObject;
            var isFocused =
                (this == focusedElement) ||
                (focusedElement != null && focusedElement.ReverseEnumerateVisualTree<TextBox>(v => v == AssociatedObject).Any());
            
            return isFocused;
        }

        /// <summary>
        /// Called if the code changes the text in the associated TextBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TbOnTextChanged(object sender, TextChangedEventArgs e)
        {
            // Only process when the Textbox does not have focus.
            if (!IsFocused() && AssociatedObject.Text != this.Text+WmtMarker)
            {
                // Cleared via property.
                if (string.IsNullOrEmpty(AssociatedObject.Text))
                {
                    OnSetWatermarkText();
                }
                // Set via property - remove color
                else
                {
                    if (_fgColor != null)
                        AssociatedObject.Foreground = _fgColor;
                    else
                        AssociatedObject.ClearValue(Control.ForegroundProperty);
                }
            }
        }
    }
}
