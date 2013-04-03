using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace JulMar.Windows.Interactivity.Input
{
    /// <summary>
    /// A trigger based on a keystroke
    /// </summary>
    public class KeyTrigger : System.Windows.Interactivity.EventTrigger
    {
        private ICoreWindow _targetElement;

        /// <summary>
        /// Backing storage for FiredOn property
        /// </summary>
        public static readonly DependencyProperty FiredOnProperty = DependencyProperty.Register("FiredOn", 
            typeof(KeyTriggerFiredOn), typeof(KeyTrigger), new PropertyMetadata(KeyTriggerFiredOn.KeyDown));

        /// <summary>
        /// Determine when the trigger reacts
        /// </summary>
        public KeyTriggerFiredOn FiredOn
        {
            get
            {
                return (KeyTriggerFiredOn)base.GetValue(FiredOnProperty);
            }
            set
            {
                base.SetValue(FiredOnProperty, value);
            }
        }

        /// <summary>
        /// Backing storage for the key to watch for
        /// </summary>
        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key", typeof(VirtualKey), typeof(KeyTrigger), null);

        /// <summary>
        /// Key to watch for
        /// </summary>
        public VirtualKey Key
        {
            get
            {
                return (VirtualKey)base.GetValue(KeyProperty);
            }
            set
            {
                base.SetValue(KeyProperty, value);
            }
        }

        /// <summary>
        /// Backing storage for the modifiers
        /// </summary>
        public static readonly DependencyProperty ModifiersProperty = DependencyProperty.Register("Modifiers", typeof(ModifierKeys), typeof(KeyTrigger), null);

        /// <summary>
        /// Additional keys (CTRL,Alt, Win,etc.)
        /// </summary>
        public ModifierKeys Modifiers
        {
            get
            {
                return (ModifierKeys)base.GetValue(ModifiersProperty);
            }
            set
            {
                base.SetValue(ModifiersProperty, value);
            }
        }


        /// <summary>
        /// Constructor
        /// </summary>
        public KeyTrigger()
        {
            EventName = "Loaded";
        }

        /// <summary>
        /// Locate the root element
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        private static UIElement GetRoot(DependencyObject current)
        {
            UIElement element = null;
            while (current != null)
            {
                element = current as UIElement;
                current = VisualTreeHelper.GetParent(current);
            }
            return element;
        }

        /// <summary>
        /// Override called when behavior is detached
        /// </summary>
        protected override void OnDetaching()
        {
            if (this._targetElement != null)
            {
                if (this.FiredOn == KeyTriggerFiredOn.KeyDown)
                {
                    this._targetElement.KeyDown -= this.OnKeyPress;
                }
                else
                {
                    this._targetElement.KeyUp -= this.OnKeyPress;
                }
            }
            base.OnDetaching();
        }

        /// <summary>
        /// This invokes the actions when the event is raised.
        /// </summary>
        /// <param name="eventArgs"></param>
        protected override void OnEvent(object eventArgs)
        {
            _targetElement = Window.Current.CoreWindow;
            if (_targetElement == null)
                return;

            if (FiredOn == KeyTriggerFiredOn.KeyDown)
            {
                _targetElement.KeyDown += OnKeyPress;
            }
            else
            {
                _targetElement.KeyUp += OnKeyPress;
            }
        }

        /// <summary>
        /// Method called when a key is pressed/released
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyPress(CoreWindow sender, KeyEventArgs e)
        {
            if (e.VirtualKey == this.Key && IsModifierKeyDown())
            {
                base.InvokeActions(e);
            }
        }

        /// <summary>
        /// Test for modifier keys
        /// </summary>
        /// <returns></returns>
        private bool IsModifierKeyDown()
        {
            if (this.Modifiers == ModifierKeys.None)
                return true;

            bool good = true;
            if ((this.Modifiers & ModifierKeys.Windows) == ModifierKeys.Windows)
            {
                if (_targetElement.GetKeyState(VirtualKey.LeftWindows) != CoreVirtualKeyStates.Down
                    && _targetElement.GetKeyState(VirtualKey.RightWindows) != CoreVirtualKeyStates.Down)
                    good = false;
            }

            if ((this.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                if (_targetElement.GetKeyState(VirtualKey.LeftShift) != CoreVirtualKeyStates.Down
                    && _targetElement.GetKeyState(VirtualKey.RightShift) != CoreVirtualKeyStates.Down)
                    good = false;
            }

            if ((this.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (_targetElement.GetKeyState(VirtualKey.Control) != CoreVirtualKeyStates.Down)
                    good = false;
            }

            if ((this.Modifiers & ModifierKeys.Menu) == ModifierKeys.Menu)
            {
                if (_targetElement.GetKeyState(VirtualKey.Menu) != CoreVirtualKeyStates.Down)
                    good = false;
            }

            return good;
        }
    }

}
