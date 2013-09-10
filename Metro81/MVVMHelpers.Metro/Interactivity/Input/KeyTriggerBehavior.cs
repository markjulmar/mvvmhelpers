using System;
using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Microsoft.Xaml.Interactivity;

namespace JulMar.Windows.Interactivity.Input
{
    /// <summary>
    /// A trigger based on a keystroke
    /// </summary>
    [ContentProperty(Name = "Actions")]
    public class KeyTriggerBehavior : DependencyObject, IBehavior
    {
        private ICoreWindow _targetElement;

        /// <summary>
        /// Backing storage for Actions collection
        /// </summary>
        public static readonly DependencyProperty ActionsProperty = 
            DependencyProperty.Register("Actions", typeof(ActionCollection), typeof(TimerTriggerBehavior), new PropertyMetadata(null));

        /// <summary>
        /// Actions collection
        /// </summary>
        public ActionCollection Actions
        {
            get
            {
                ActionCollection actions = (ActionCollection)base.GetValue(ActionsProperty);
                if (actions == null)
                {
                    actions = new ActionCollection();
                    base.SetValue(ActionsProperty, actions);
                }
                return actions;
            }
        }
        /// <summary>
        /// Backing storage for FiredOn property
        /// </summary>
        public static readonly DependencyProperty FiredOnProperty = DependencyProperty.Register("FiredOn", 
            typeof(KeyTriggerFiredOn), typeof(KeyTriggerBehavior), new PropertyMetadata(KeyTriggerFiredOn.KeyDown));

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
        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key", typeof(VirtualKey), typeof(KeyTriggerBehavior), null);

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
        public static readonly DependencyProperty ModifiersProperty = DependencyProperty.Register("Modifiers", typeof(ModifierKeys), typeof(KeyTriggerBehavior), null);

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
        /// This method hooks into the keyboard events for the window.
        /// </summary>
        private bool HookEvents()
        {
            _targetElement = Window.Current.CoreWindow;
            if (_targetElement == null)
                return false;

            if (FiredOn == KeyTriggerFiredOn.KeyDown)
            {
                _targetElement.KeyDown += OnKeyPress;
            }
            else
            {
                _targetElement.KeyUp += OnKeyPress;
            }

            return true;
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
                Interaction.ExecuteActions(AssociatedObject, Actions, e);
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

        /// <summary>
        /// Gets the <see cref="T:Windows.UI.Xaml.DependencyObject"/> to which the <seealso cref="T:Microsoft.Xaml.Interactivity.IBehavior"/> is attached.
        /// </summary>
        public DependencyObject AssociatedObject
        {
            get; private set;
        }

        /// <summary>
        /// Attaches to the specified object.
        /// </summary>
        /// <param name="associatedObject">The <see cref="T:Windows.UI.Xaml.DependencyObject"/> to which the <seealso cref="T:Microsoft.Xaml.Interactivity.IBehavior"/> will be attached.</param>
        public void Attach(DependencyObject associatedObject)
        {
            if ((associatedObject != AssociatedObject) && !DesignMode.DesignModeEnabled)
            {
                if (AssociatedObject != null)
                    throw new InvalidOperationException("Cannot attach behavior multiple times.");

                AssociatedObject = associatedObject;
                if (!HookEvents())
                {
                    FrameworkElement fe = associatedObject as FrameworkElement;
                    if (fe != null)
                    {
                        RoutedEventHandler handler = null;
                        handler = delegate
                        {
                            HookEvents();
                            fe.Loaded -= handler;
                        };
                        fe.Loaded += handler;
                    }
                }
            }
        }

        /// <summary>
        /// Detaches this instance from its associated object.
        /// </summary>
        public void Detach()
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

            AssociatedObject = null;
        }
    }
}
