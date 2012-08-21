using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using System.Reflection;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using System.Runtime.InteropServices.WindowsRuntime;

namespace JulMar.Windows.Behaviors
{
    /// <summary>
    /// This class manages a collection of command to event mappings.  It is used to wire up View events to a
    /// ViewModel ICommand implementation. 
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// 
    /// <Behaviors:EventCommander.Mappings>
    ///    <Behaviors:CommandEvent Command="{Binding PointerEnterCommand}" Event="PointerEnter" />
    ///    <Behaviors:CommandEvent Command="{Binding PointerLeaveCommand}" Event="PointerLeave" />
    /// </Behaviors:EventCommander.Mappings>
    /// 
    /// ]]>
    /// </example>
    public static class EventCommander
    {
        public static readonly DependencyProperty MappingsProperty = DependencyProperty.RegisterAttached("Mappings",
                            typeof(CommandEventCollection), typeof(EventCommander), 
                            new PropertyMetadata(null, OnMappingsChanged));

        /// <summary>
        /// Retrieves the mapping collection
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static CommandEventCollection GetMappings(DependencyObject obj)
        {
            var map = obj.GetValue(MappingsProperty) as CommandEventCollection;
            if (map == null)
            {
                map = new CommandEventCollection();
                SetMappings(obj, map);
            }
            return map;
        }

        /// <summary>
        /// This sets the mapping collection.
        /// </summary>
        /// <param name="obj">Dependency Object</param>
        /// <param name="value">Mapping collection</param>
        public static void SetMappings(DependencyObject obj, CommandEventCollection value)
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
            var sender = target as FrameworkElement;
            if (sender != null)
            {
                if (e.OldValue != null)
                {
                    CommandEventCollection cec = e.OldValue as CommandEventCollection;
                    if (cec != null)
                        cec.Unsubscribe(target);
                }
                if (e.NewValue != null)
                {
                    CommandEventCollection cec = e.NewValue as CommandEventCollection;
                    if (cec != null)
                        cec.Subscribe(target);
                }
            }
        }
    }

    /// <summary>
    /// This is passed to the ICommand handler for the event
    /// </summary>
    public class EventParameters
    {
        /// <summary>
        /// The sender of the handled event
        /// </summary>
        public object Sender { get; set; }

        /// <summary>
        /// The passed EventArgs for the event.
        /// </summary>
        public object EventArgs { get; set; }

        /// <summary>
        /// The ICommand which has just been executed
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        /// The associated CommandParameter (if any).
        /// </summary>
        public object CommandParameter { get; set; }

        /// <summary>
        /// Constructor for the EventParameters
        /// </summary>
        /// <param name="command">ICommand</param>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event args</param>
        /// <param name="parameter">CommandParameter</param>
        internal EventParameters(ICommand command, object sender, object e, object parameter)
        {
            Command = command;
            Sender = sender;
            EventArgs = e;
            CommandParameter = parameter;
        }
    }

    /// <summary>
    /// This represents a single event to command mapping. 
    /// </summary>
    public class CommandEvent : FrameworkElement
    {
        /// <summary>
        /// Command Property Dependency Property
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(CommandEvent), new PropertyMetadata(null));

        /// <summary>
        /// Parameter for the ICommand
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(CommandEvent), new PropertyMetadata(null));

        /// <summary>
        /// Event Dependency Property
        /// </summary>
        public static readonly DependencyProperty EventProperty =
            DependencyProperty.Register("Event", typeof(string), typeof(CommandEvent), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the Event property.
        /// </summary>
        public string Event
        {
            get { return (string)GetValue(EventProperty); }
            set { SetValue(EventProperty, value); }
        }

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
        /// Wires up an event to the target
        /// </summary>
        /// <param name="target"></param>
        internal void Subscribe(object target)
        {
            string eventName = Event;

            if (target != null)
            {
                SetBinding(DataContextProperty, new Binding { Source = target, Path = new PropertyPath("DataContext"), Mode = BindingMode.OneWay });

                EventInfo ei = LookForEventDeclaration(target, eventName);
                if (ei != null)
                {
                    var handler = GetEventMethod(ei);

                    // WinRT events cannot be subscribed to through normal events because it uses
                    // event tokens to map events to handlers. The compiler emits calls through WRM to do the work.
                    WindowsRuntimeMarshal.AddEventHandler(
                        d => (EventRegistrationToken)ei.AddMethod.Invoke(target, new object[] { d }),
                        et => ei.RemoveMethod.Invoke(target, new object[] { et }), handler);
                    //ei.AddEventHandler(target, handler);
                    return;
                }
            }

            Debug.WriteLine(string.Format("Unable to locate event {0} on {1}", eventName, target));
        }

        /// <summary>
        /// This searches the type for a given event - including ancestor classes.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="eventName"></param>
        /// <returns></returns>
        private EventInfo LookForEventDeclaration(object target, string eventName)
        {
            Type currentType = target.GetType();
            while (currentType != typeof(object))
            {
                TypeInfo typeInfo = currentType.GetTypeInfo();
                EventInfo eventInfo = typeInfo.GetDeclaredEvent(eventName);
                if (eventInfo != null)
                    return eventInfo;

                currentType = typeInfo.BaseType;
            }

            return null;
        }

        /// <summary>
        /// Unwires target event
        /// </summary>
        /// <param name="target"></param>
        internal void Unsubscribe(object target)
        {
            if (target != null)
            {
                EventInfo ei = LookForEventDeclaration(target, Event);
                if (ei != null)
                {
                    var handler = GetEventMethod(ei);
                    WindowsRuntimeMarshal.RemoveEventHandler(
                            et => ei.RemoveMethod.Invoke(target, new object[] { et }), handler);
                    //ei.RemoveEventHandler(target, GetEventMethod(ei));
                }
            }
        }

        private Delegate _method;

        /// <summary>
        /// Retrieves a delegate to our OnEventRaised method which may be used with a specific
        /// EventHandler subtype based on an EventInfo.
        /// </summary>
        /// <param name="ei">Event to generate</param>
        /// <returns></returns>
        private Delegate GetEventMethod(EventInfo ei)
        {
            if (ei == null)
                throw new ArgumentNullException("ei");
            if (ei.EventHandlerType == null)
                throw new ArgumentException("EventHandlerType is null");
            if (_method == null)
            {
                MethodInfo methodInfo = GetType().GetTypeInfo().DeclaredMethods.FirstOrDefault(
                        mi => mi.Name == "OnEventRaised" && mi.IsPrivate && !mi.IsStatic);

                _method = methodInfo.CreateDelegate(ei.EventHandlerType, this);
            }

            return _method;
        }

        /// <summary>
        /// This is invoked by the event - it invokes the command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEventRaised(object sender, object e)
        {
            // If we didn't get the DC from the binding (not sure why WinRT won't
            // carry this across), then just reach in and get it ourselves.
            if (DataContext == null)
            {
                var fe = sender as FrameworkElement;
                if (fe != null)
                    DataContext = fe.DataContext;
            }

            var thisCommand = Command;
            if (thisCommand != null)
            {
                var ep = new EventParameters(thisCommand, sender, e, CommandParameter);
                if (thisCommand.CanExecute(ep))
                    thisCommand.Execute(ep);
            }
            else
            {
                Debug.WriteLine(string.Format("Missing Command on event handler, {0}: Sender={1}, EventArgs={2}, DataContext={3}", Event, sender, e, DataContext));
            }
        }
    }

    /// <summary>
    /// Collection of command to event mappings
    /// </summary>
    public class CommandEventCollection : ObservableCollection<CommandEvent>
    {
        private object _target;
        private readonly List<CommandEvent> _currentList = new List<CommandEvent>();

        /// <summary>
        /// Constructor
        /// </summary>
        public CommandEventCollection()
        {
            ((INotifyCollectionChanged)this).CollectionChanged += OnCollectionChanged;
        }

        /// <summary>
        /// Wire up events to the target
        /// </summary>
        /// <param name="target"></param>
        internal void Subscribe(object target)
        {
            _target = target;
            foreach (var item in this)
                item.Subscribe(target);
        }

        /// <summary>
        /// Unwire all target events
        /// </summary>
        /// <param name="target"></param>
        internal void Unsubscribe(object target)
        {
            foreach (var item in this)
                item.Unsubscribe(target);
            _target = null;
        }

        /// <summary>
        /// This handles the collection change event - it then subscribes and unsubscribes events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                        OnItemAdded((CommandEvent)item);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                        OnItemRemoved((CommandEvent)item);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (var item in e.OldItems)
                        OnItemRemoved((CommandEvent)item);
                    foreach (var item in e.NewItems)
                        OnItemAdded((CommandEvent)item);
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Reset:
                    foreach (var item in _currentList)
                        item.Unsubscribe(_target);
                    _currentList.Clear();
                    foreach (var item in this)
                        OnItemAdded(item);
                    break;

                default:
                    return;
            }
        }

        /// <summary>
        /// A new item has been added to the event list
        /// </summary>
        /// <param name="item"></param>
        private void OnItemAdded(CommandEvent item)
        {
            if (item != null && _target != null)
            {
                _currentList.Add(item);
                item.Subscribe(_target);
            }
        }

        /// <summary>
        /// An item has been removed from the event list.
        /// </summary>
        /// <param name="item"></param>
        private void OnItemRemoved(CommandEvent item)
        {
            if (item != null && _target != null)
            {
                _currentList.Remove(item);
                item.Unsubscribe(_target);
            }
        }
    }
}
