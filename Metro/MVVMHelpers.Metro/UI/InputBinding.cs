using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using JulMar.Core.Extensions;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.System;
using Windows.UI.Xaml.Data;
using JulMar.Windows.Extensions;
using Windows.UI.Xaml.Input;

namespace JulMar.Windows.UI
{
    /// <summary>
    /// InputBinder binds keyboard events to ICommand implementations
    /// </summary>
    public static class InputBinder
    {
        private static Page _activePage;
        private static readonly List<VirtualKey> ActiveKeys = new List<VirtualKey>();  

        /// <summary>
        /// Mappings backing store
        /// </summary>
        public static readonly DependencyProperty MappingsProperty = DependencyProperty.RegisterAttached("Mappings",
                            typeof(InputBinderCollection), typeof(InputBinder),
                            new PropertyMetadata(null, OnMappingsChanged));

        /// <summary>
        /// Retrieves the mapping collection
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static InputBinderCollection GetMappings(DependencyObject obj)
        {
            var map = obj.GetValue(MappingsProperty) as InputBinderCollection;
            if (map == null)
            {
                map = new InputBinderCollection();
                SetMappings(obj, map);
            }
            return map;
        }

        /// <summary>
        /// This sets the mapping collection.
        /// </summary>
        /// <param name="obj">Dependency Object</param>
        /// <param name="value">Mapping collection</param>
        public static void SetMappings(DependencyObject obj, InputBinderCollection value)
        {
            obj.SetValue(MappingsProperty, value);
        }

        /// <summary>
        /// This changes the event mapping
        /// </summary>
        /// <param name="target"></param>
        /// <param name="e"></param>
        private static void OnMappingsChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (Designer.InDesignMode)
                return;

            var sender = target as Page;
            if (sender != null)
            {
                _activePage = sender;
                _activePage.Unloaded += SenderOnUnloaded;

                if (e.NewValue != null)
                {
                    Window.Current.Dispatcher.AcceleratorKeyActivated += DispatcherOnAcceleratorKeyActivated;
                    Window.Current.CoreWindow.Activated += CoreWindowOnActivated;

                    InputBinderCollection bindingCollection = (InputBinderCollection) e.NewValue;

                    // Forward the data context so bindings work properly.
                    foreach (var entry in bindingCollection)
                    {
                        entry.SetBinding(FrameworkElement.DataContextProperty,
                            new Binding { Source = _activePage, Path=new PropertyPath("DataContext")});
                    }

                    bindingCollection.CollectionChanged += OnForwardDataContext;

                }
                else
                {
                    Window.Current.Dispatcher.AcceleratorKeyActivated -= DispatcherOnAcceleratorKeyActivated;
                    Window.Current.CoreWindow.Activated -= CoreWindowOnActivated;
                    ActiveKeys.Clear();
                }

                if (e.OldValue != null)
                {
                    InputBinderCollection bindingCollection = (InputBinderCollection)e.OldValue;
                    bindingCollection.CollectionChanged -= OnForwardDataContext;
                }
            }
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
        /// This is called when items are added to the collection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnForwardDataContext(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var entry in e.NewItems.Cast<InputBinding>())
                {
                    entry.SetBinding(FrameworkElement.DataContextProperty,
                        new Binding { Source = _activePage, Path = new PropertyPath("DataContext") });
                }
            }
        }

        /// <summary>
        /// Called when an accelerator key is pressed (i.e. a key with a Menu)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void DispatcherOnAcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs e)
        {
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
        /// The associated Page has been unloaded - want to detach
        /// from the global key events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        private static void SenderOnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender == _activePage)
            {
                Window.Current.Dispatcher.AcceleratorKeyActivated -= DispatcherOnAcceleratorKeyActivated;
                ActiveKeys.Clear();
            }
        }

        /// <summary>
        /// Checks for any input binding match in the current keyboard state.
        /// If we find one, run the associated command.
        /// </summary>
        private static void CheckForActiveKey()
        {
            if (_activePage == null || ActiveKeys.Count == 0)
                return;

            InputBinderCollection bindingCollection = GetMappings(_activePage);
            if (bindingCollection == null || bindingCollection.Count == 0)
                return;

            foreach (var binding in bindingCollection)
            {
                if (binding.CheckKey(ActiveKeys))
                {
                    // If the currently active element is some form of TextBox, then
                    // shift focus so that any data bound information is transferred properly
                    Control currentFocusedControl = FocusManager.GetFocusedElement() as Control;
                    if (currentFocusedControl is TextBox
                        || currentFocusedControl is RichEditBox
                        || currentFocusedControl is PasswordBox)
                    {
                        Frame frameOwner = Window.Current.Content as Frame;
                        if (frameOwner != null)
                        {
                            if (frameOwner.Focus(FocusState.Programmatic))
                            {
                                currentFocusedControl.Focus(FocusState.Programmatic);
                            }
                        }

                    }

                    ICommand command = binding.Command;
                    if (command != null && command.CanExecute(binding.CommandParameter))
                        command.Execute(binding.CommandParameter);
                }
            }
        }
    }

    /// <summary>
    /// A single input binding
    /// </summary>
    public sealed class InputBinding : FrameworkElement
    {
        /// <summary>
        /// Command Property Dependency Property
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(InputBinding), new PropertyMetadata(null));

        /// <summary>
        /// Parameter for the ICommand
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(InputBinding), new PropertyMetadata(null));

        /// <summary>
        /// Parameter for the Key
        /// </summary>
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(string), typeof(InputBinding), new PropertyMetadata(null, OnKeyChanged));

        /// <summary>
        /// Gets or sets the Command property. 
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the CommandParameter property.
        /// </summary>
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Key which invokes command
        /// </summary>
        public string Key
        {
            get { return (string) GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        private List<VirtualKey> _keys = new List<VirtualKey>(); 

        /// <summary>
        /// Called when the key associated with this command changes.
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="e"></param>
        private static void OnKeyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            ((InputBinding)dpo).OnKeyChanged((string)e.NewValue);
        }

        /// <summary>
        /// Called when the key associated with this command changes.
        /// </summary>
        /// <param name="keys">Keys</param>
        private void OnKeyChanged(string keys)
        {
            _keys.Clear();

            var keyList = AccessKey.ParseKeys(keys);
            if (keyList != null)
                _keys.AddRange(keyList);
        }

        /// <summary>
        /// Check a set of keys against a string
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <returns></returns>
        public bool CheckKey(IList<VirtualKey> keys)
        {
            return _keys.Compare(keys);
        }
    }

    /// <summary>
    /// Collection of input bindings
    /// </summary>
    public sealed class InputBinderCollection : ObservableCollection<InputBinding>
    {
    }
}
