using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;

using JulMar.Extensions;

namespace JulMar.UI
{
    /// <summary>
    /// Class to add an accelerator key to a control
    /// </summary>
    public static class AccessKey
    {
        private static readonly List<Tuple<List<VirtualKey>, Control>> ActiveAccelerators = new List<Tuple<List<VirtualKey>, Control>>();
        private static readonly List<VirtualKey> ActiveKeys = new List<VirtualKey>(); 

        /// <summary>
        /// Keyboard accelerator
        /// </summary>
        public static readonly DependencyProperty AcceleratorProperty =
            DependencyProperty.RegisterAttached("Accelerator", typeof (string), typeof (AccessKey), new PropertyMetadata(default(string), OnAcceleratorChanged));

        /// <summary>
        /// Retrieve the keyboard accelerator for an element.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static string GetAccelerator(FrameworkElement owner)
        {
            return (string)owner.GetValue(AcceleratorProperty);
        }

        /// <summary>
        /// Sets the keyboard accelerator for an element.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="keyText"></param>
        public static void SetAccelerator(FrameworkElement owner, string keyText)
        {
            owner.SetValue(AcceleratorProperty, keyText);
        }

        private static void OnAcceleratorChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            if (Designer.InDesignMode)
                return;

            Control owner = dpo as Control;
            if (owner == null)
                return;

            // Remove any existing accelerator lists for this control, or 
            // dead entries which are not valid anymore.
            foreach (var entry in ActiveAccelerators
                .Where(ac => ac.Item2 == owner).ToList())
            {
                ActiveAccelerators.Remove(entry);
            }

            if (e.NewValue != null)
            {
                string keyText = e.NewValue.ToString();
                var watchFor = ParseKeys(keyText);
                if (watchFor != null && watchFor.Count > 0)
                {
                    owner.Unloaded += OwnerOnUnloaded;
                    ActiveAccelerators.Add(Tuple.Create(watchFor, owner));

                    // Set the accelerator/access key for automation
                    if (watchFor.Contains(VirtualKey.Menu)
                        && !watchFor.Contains(VirtualKey.Control))
                        AutomationProperties.SetAcceleratorKey(owner, keyText);
                    else if (watchFor.Contains(VirtualKey.Control)
                        && !watchFor.Contains(VirtualKey.Menu))
                        AutomationProperties.SetAccessKey(owner, keyText);
                }

                if (ActiveAccelerators.Count == 1)
                {
                    Window.Current.Dispatcher.AcceleratorKeyActivated += DispatcherOnAcceleratorKeyActivated;
                    Window.Current.CoreWindow.Activated += CoreWindowOnActivated;
                }
            }
            else
            {
                owner.Unloaded -= OwnerOnUnloaded;
                if (ActiveAccelerators.Count == 0)
                {
                    Window.Current.Dispatcher.AcceleratorKeyActivated -= DispatcherOnAcceleratorKeyActivated;
                    Window.Current.CoreWindow.Activated -= CoreWindowOnActivated;
                }
            }
        }

        /// <summary>
        /// Returns a list of virtual keys from a string.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        internal static List<VirtualKey> ParseKeys(string keys)
        {
            if (string.IsNullOrEmpty(keys))
                return null;

            List<VirtualKey> keyList = new List<VirtualKey>();

            string[] values;
            if (keys.Contains(" "))
            {
                values = keys.Split(' ');
            }
            else if (keys.Contains("+"))
            {
                values = keys.Split('+');
            }
            else
                values = new string[1] { keys };

            foreach (string item in values)
            {
                VirtualKey vkey;
                if (Enum.TryParse(item, true, out vkey))
                    keyList.Add(vkey);
                else if (string.Compare("ALT", item, StringComparison.OrdinalIgnoreCase) == 0)
                    keyList.Add(VirtualKey.Menu);
                else
                    throw new ArgumentException("Could not match " + item + " to a VirtualKey enum value");
            }

            return keyList;
        }

        /// <summary>
        /// Called when the window gets or loses input focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CoreWindowOnActivated(CoreWindow sender, WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == CoreWindowActivationState.Deactivated)
                ActiveKeys.Clear();
        }

        /// <summary>
        /// Detect an accelerator key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void DispatcherOnAcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs e)
        {
            Debug.WriteLine("Key:{0}, EventType:{1}, IsMenuKeyDown:{2}", 
                e.VirtualKey, e.EventType, e.KeyStatus.IsMenuKeyDown);

            if (e.EventType == CoreAcceleratorKeyEventType.KeyDown
                || e.EventType == CoreAcceleratorKeyEventType.SystemKeyDown)
            {
                if (!ActiveKeys.Contains(e.VirtualKey))
                    ActiveKeys.Add(e.VirtualKey);
            }
            else if (e.EventType == CoreAcceleratorKeyEventType.KeyUp
                  || e.EventType == CoreAcceleratorKeyEventType.SystemKeyUp)
            {
                if (ActiveKeys.Contains(e.VirtualKey))
                {
                    CheckForActiveKey();
                    ActiveKeys.Remove(e.VirtualKey);
                }
            }
        }

        /// <summary>
        /// Look for an active key and shift focus when found.
        /// </summary>
        private static void CheckForActiveKey()
        {
            foreach (var entry in ActiveAccelerators.ToList())
            {
                if (entry.Item1.Compare(ActiveKeys))
                {
                    // Shift focus to the element.
                    entry.Item2.Focus(FocusState.Programmatic);
                }
            }
        }

        /// <summary>
        /// Called when a specific control we are monitoring is unloaded.
        /// We remove it from the list we are holding onto.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        private static void OwnerOnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            // Remove any existing accelerator lists for this control, or 
            // dead entries which are not valid anymore.
            foreach (var entry in ActiveAccelerators
                .Where(ac => ac.Item2 == sender).ToList())
            {
                ActiveAccelerators.Remove(entry);
            }
        }
    }
}
