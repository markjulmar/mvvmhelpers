using System.Windows;
using System.Windows.Controls;

namespace JulMar.Windows.Behaviors
{
    /// <summary>
    /// This class provides a bindable property for the PasswordBox class.
    /// <PasswordBox PasswordBoxHelper.Text="{Binding .. }" />
    /// </summary>
    public static class PasswordBoxHelper
    {
        // Internal property
        static readonly DependencyProperty _UpdatingProperty = DependencyProperty.RegisterAttached("_UpdatingPwd", typeof(bool), typeof(PasswordBoxHelper), new PropertyMetadata(false));

        #region Text Property
        /// <summary>
        /// Text Property to pul from PasswordBox
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached("Text", typeof(string), typeof(PasswordBoxHelper), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnAttachDetach));

        /// <summary>
        /// Get the Password Text as a DP.
        /// </summary>
        /// <param name="dpo"></param>
        /// <returns></returns>
        public static string GetText(DependencyObject dpo)
        {
            return (string)dpo.GetValue(TextProperty);
        }

        /// <summary>
        /// Set the Password Text as a DP.
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="value"></param>
        public static void SetText(DependencyObject dpo, string value)
        {
            dpo.SetValue(TextProperty, value);
        }
        #endregion

        /// <summary>
        /// Called to attach/detach the handler
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="e"></param>
        private static void OnAttachDetach(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox passwordBox = dpo as PasswordBox;
            if (passwordBox == null)
                return;

            // Always disconnect when reapplying value.
            passwordBox.PasswordChanged -= HandlePasswordChanged;

            // Set the value.
            if ((bool)dpo.GetValue(_UpdatingProperty) == false)
                passwordBox.Password = (e.NewValue != null) ? e.NewValue.ToString() : string.Empty;

            // Hook the value
            passwordBox.PasswordChanged += HandlePasswordChanged;
        }

        /// <summary>
        /// This is called when the PasswordBox value changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = (PasswordBox) sender;

            string currentValue = GetText(passwordBox);
            string pbValue = passwordBox.Password;

            if (string.Compare(currentValue, pbValue) != 0)
            {
                passwordBox.SetValue(_UpdatingProperty, true);
                SetText(passwordBox, pbValue);
                passwordBox.SetValue(_UpdatingProperty, false);
            }
        }
    }
}
