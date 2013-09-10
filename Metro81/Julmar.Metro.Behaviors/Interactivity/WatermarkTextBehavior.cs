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
            DependencyProperty.RegisterAttached("Text", typeof(string), typeof(WatermarkTextBehavior),
                                                new PropertyMetadata(null, OnTextChanged));

        /// <summary>
        /// Get or set the watermark text on the behavior.
        /// </summary>
        public string WatermarkText
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Backing storage for IsWatermarkVisible property.
        /// </summary>
        public static readonly DependencyProperty IsWatermarkVisibleProperty =
            DependencyProperty.Register("IsWatermarkVisible", typeof (bool), typeof (WatermarkTextBehavior), new PropertyMetadata(default(bool), OnChangeWatermarkVisibility));

        /// <summary>
        /// This is called when the watermark visibility changes.
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="e"></param>
        private static void OnChangeWatermarkVisibility(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            WatermarkTextBehavior wtb = (WatermarkTextBehavior) dpo;
            if ((bool) e.NewValue == true)
            {
                if (!wtb.OnSetWatermarkText())
                {
                    wtb.IsWatermarkVisible = false;
                }
            }
            else
            {
                wtb.OnRemoveWatermarkText();
            }
        }

        /// <summary>
        /// Returns whether the watermark is currently visible.  This can be databound to test within a ViewModel.
        /// This should be used to determine if the Text property of the TextBox is valid or not.
        /// </summary>
        public bool IsWatermarkVisible
        {
            get { return (bool)GetValue(IsWatermarkVisibleProperty); }
            set { SetValue(IsWatermarkVisibleProperty, value); }
        }

        /// <summary>
        /// Helper method to determine if the TextBox has any text in it.
        /// </summary>
        /// <param name="tb"></param>
        /// <returns></returns>
        public static bool GetWatermarkVisible(TextBox tb)
        {
            var behaviors = Interaction.GetBehaviors(tb);
            var thisBehavior = behaviors.FirstOrDefault(b => b is WatermarkTextBehavior) as WatermarkTextBehavior;
            if (thisBehavior != null && thisBehavior.AssociatedObject == tb)
            {
                return thisBehavior.IsWatermarkVisible;
            }

            return false;
        }

        /// <summary>
        /// Attached property version to attach the behavior to a TextBox
        /// </summary>
        /// <param name="tb"></param>
        /// <returns></returns>
        public static string GetText(TextBox tb)
        {
            return (string) tb.GetValue(TextProperty);
        }

        /// <summary>
        /// Attached property version to attach the behavior to a TextBox
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static void SetText(TextBox tb, string text)
        {
            tb.SetValue(TextProperty, text);
        }

        /// <summary>
        /// Text color property - set in XAML
        /// </summary>
        public static readonly DependencyProperty WatermarkColorProperty =
            DependencyProperty.Register("WatermarkColor", typeof(Brush), typeof(WatermarkTextBehavior),
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
        public Brush WatermarkColor
        {
            get { return (Brush)GetValue(WatermarkColorProperty); }
            set { SetValue(WatermarkColorProperty, value); }
        }

        /// <summary>
        /// This is called when the watermark text is changed on either a WatermarkTextboxBehavior
        /// OR on a TextBox directly to attach/detach the behavior.
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="e"></param>
        private static void OnTextChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            WatermarkTextBehavior wtb = dpo as WatermarkTextBehavior;
            if (wtb != null)
            {
                if (wtb.AssociatedObject != null)
                    wtb.IsWatermarkVisible = true;
            }
            // Attach/Detach request
            else
            {
                TextBox tb = dpo as TextBox;
                if (tb != null)
                {
                    string text = (e.NewValue ?? "").ToString();
                    var behaviors = Interaction.GetBehaviors(tb);
                    var thisBehavior =
                        behaviors.FirstOrDefault(b => b is WatermarkTextBehavior) as WatermarkTextBehavior;

                    if (string.IsNullOrWhiteSpace(text))
                    {
                        if (thisBehavior != null)
                        {
                            behaviors.Remove(thisBehavior);
                        }
                    }
                    else
                    {
                        if (thisBehavior == null)
                        {
                            thisBehavior = new WatermarkTextBehavior() {WatermarkText = text};
                            behaviors.Add(thisBehavior);
                        }
                        else
                            thisBehavior.WatermarkText = text;
                    }

                }
            }
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
                IsWatermarkVisible = true;
        }

        /// <summary>
        /// Called when the TextBox is loaded - we associate the watermark
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        private void TbOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            IsWatermarkVisible = true;
        }

        /// <summary>
        /// Override called when behavior is detached
        /// </summary>
        protected override void OnDetaching()
        {
            AssociatedObject.GotFocus -= TbOnGotFocus;
            AssociatedObject.LostFocus -= TbOnLostFocus;
            AssociatedObject.TextChanged -= TbOnTextChanged;
            
            IsWatermarkVisible = false;
        }

        /// <summary>
        /// Method to set the watermark text
        /// </summary>
        private bool OnSetWatermarkText()
        {
            if (string.IsNullOrEmpty(AssociatedObject.Text))
            {
                Brush fgColor = AssociatedObject.Foreground;
                _fgColor = fgColor != Control.ForegroundProperty.GetMetadata(typeof (TextBox)).DefaultValue ? fgColor : null;
                AssociatedObject.Foreground = this.WatermarkColor;
                AssociatedObject.Text = this.WatermarkText + WmtMarker;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Used to change the foreground color at runtime.
        /// </summary>
        private void OnChangeWatermarkColor()
        {
            if (!string.IsNullOrEmpty(AssociatedObject.Text))
            {
                if (IsWatermarkVisible)
                {
                    AssociatedObject.Foreground = this.WatermarkColor;
                }
            }
        }

        /// <summary>
        /// This is used to remove the watermark text
        /// </summary>
        private bool OnRemoveWatermarkText()
        {
            if (!string.IsNullOrEmpty(AssociatedObject.Text))
            {
                if (AssociatedObject.Text == WatermarkText + WmtMarker)
                {
                    if (_fgColor != null)
                        AssociatedObject.Foreground = _fgColor;
                    else
                        AssociatedObject.ClearValue(Control.ForegroundProperty);
                    AssociatedObject.Text = "";
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Called when focus is lost - possibly add in watermark.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        private void TbOnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            IsWatermarkVisible = true;
        }

        /// <summary>
        /// Called when focus is gained - remove watermark
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        private void TbOnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            IsWatermarkVisible = false;
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
            if (!IsFocused() && !IsWatermarkVisible)
            {
                // Cleared via property.
                if (string.IsNullOrEmpty(AssociatedObject.Text))
                {
                    IsWatermarkVisible = true;
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
