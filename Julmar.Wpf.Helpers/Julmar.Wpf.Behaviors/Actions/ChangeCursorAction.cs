using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;


namespace JulMar.Windows.Actions
{
    /// <summary>
    /// This action is used to trigger a cursor change based on some event.
    /// </summary>
    public class ChangeCursorAction : TriggerAction<FrameworkElement>
    {
        /// <summary>
        /// Dependency Property to back the OverrideGlobal property
        /// </summary>
        public static readonly DependencyProperty OverrideGlobalProperty =
            DependencyProperty.Register("OverrideGlobal", typeof(bool), typeof(ChangeCursorAction), new PropertyMetadata(default(bool)));

        /// <summary>
        /// True to override the global mouse cursor (all elements)
        /// False to simply set the cursor on the associated control
        /// </summary>
        public bool OverrideGlobal
        {
            get { return (bool)GetValue(OverrideGlobalProperty); }
            set { SetValue(OverrideGlobalProperty, value); }
        }

        /// <summary>
        /// Dependency Property for Cursor to change to
        /// </summary>
        public static readonly DependencyProperty CursorProperty =
            DependencyProperty.Register("Cursor", typeof(Cursor), typeof(ChangeCursorAction), new PropertyMetadata(null));

        /// <summary>
        /// Cursor to change to
        /// </summary>
        public Cursor Cursor
        {
            get { return (Cursor)GetValue(CursorProperty); }
            set { SetValue(CursorProperty, value); }
        }

        /// <summary>
        /// Invokes the action.
        /// </summary>
        /// <param name="parameter">The parameter to the action. If the action does not require a parameter, the parameter may be set to a null reference.</param>
        protected override void Invoke(object parameter)
        {
            if (OverrideGlobal)
            {
                Mouse.OverrideCursor = this.Cursor;
            }
            else
            {
                AssociatedObject.Cursor = this.Cursor;
            }
        }
    }
}
