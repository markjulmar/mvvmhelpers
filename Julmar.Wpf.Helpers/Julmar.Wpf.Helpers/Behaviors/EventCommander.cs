using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows;
using System.Reflection;
using System.Diagnostics;

namespace JulMar.Windows.Behaviors
{
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
        public EventArgs EventArgs { get; set; }

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
        internal EventParameters(ICommand command, object sender, EventArgs e, object parameter)
        {
            Command = command;
            Sender = sender;
            EventArgs = e;
            CommandParameter = parameter;
        }
    }

    /// <summary>
    /// This represents a single event to command mapping.  It derives from Freezable in order to inherit context and support 
    /// element name bindings per Mike Hillberg blog post.
    /// </summary>
    public class CommandEvent : Freezable
    {
        /// <summary>
        /// Command Property Dependency Property
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof (ICommand), typeof (CommandEvent), new UIPropertyMetadata(null));

        /// <summary>
        /// Parameter for the ICommand
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(CommandEvent), new UIPropertyMetadata(null));

        /// <summary>
        /// Event Dependency Property
        /// </summary>
        public static readonly DependencyProperty EventProperty = DependencyProperty.Register("Event", typeof(string), typeof(CommandEvent), new UIPropertyMetadata(string.Empty));

        /// <summary>
        /// DataContext for any bindings applied to this CommandEvent
        /// </summary>
        public static readonly DependencyProperty DataContextProperty = FrameworkElement.DataContextProperty.AddOwner(typeof(CommandEvent), new FrameworkPropertyMetadata(null));

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
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the CommandParameter property.
        /// </summary>
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty);  }
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
                BindingOperations.SetBinding(this, FrameworkElement.DataContextProperty,
                                             new Binding("DataContext") {Source = target});

                EventInfo ei = target.GetType().GetEvent(eventName, BindingFlags.Public | BindingFlags.Instance);
                if (ei != null)
                {
                    ei.RemoveEventHandler(target, GetEventMethod(ei));
                    ei.AddEventHandler(target, GetEventMethod(ei));
                    return;
                }

                // If the name has a colon namespace declaration, then drop that off.
                int dotPos = eventName.IndexOf(':');
                if (dotPos > 0)
                    eventName = eventName.Substring(dotPos + 1);

                // Look for an attached event on a Dependency Property
                dotPos = eventName.IndexOf('.');
                if (dotPos > 0 && eventName.Length > dotPos)
                {
                    // Scan the event manager for the specified event.
                    var attachedEvent = EventManager.GetRoutedEvents().Where(evt => evt.ToString() == eventName).SingleOrDefault();
                    if (attachedEvent != null)
                    {
                        FrameworkElement fe = target as FrameworkElement;
                        if (fe == null)
                        {
                            Debug.WriteLine(string.Format("Failed to cast Target {0} to FrameworkElement to subscribe to attached event {1}", target, eventName));
                        }
                        else
                        {
                            fe.RemoveHandler(attachedEvent, GetRoutedEventMethod());
                            fe.AddHandler(attachedEvent, GetRoutedEventMethod());
                        }
                    }
                    return;
                }
            }

            Debug.WriteLine(string.Format("Unable to locate event {0} on {1}", eventName, target));
        }

        /// <summary>
        /// Unwires target event
        /// </summary>
        /// <param name="target"></param>
        internal void Unsubscribe(object target)
        {
            if (target != null)
            {
                EventInfo ei = target.GetType().GetEvent(Event, BindingFlags.Public | BindingFlags.Instance);
                if (ei != null)
                    ei.RemoveEventHandler(target, GetEventMethod(ei));
                else
                {
                    string eventName = Event;

                    // If the name has a colon namespace declaration, then drop that off.
                    int dotPos = eventName.IndexOf(':');
                    if (dotPos > 0)
                        eventName = eventName.Substring(dotPos + 1);

                    // Look for an attached event on a Dependency Property
                    dotPos = eventName.IndexOf('.');
                    if (dotPos > 0 && eventName.Length > dotPos)
                    {
                        // Scan the event manager for the specified event.
                        var attachedEvent = EventManager.GetRoutedEvents().Where(evt => evt.Name == eventName).SingleOrDefault();
                        if (attachedEvent != null)
                        {
                            FrameworkElement fe = target as FrameworkElement;
                            if (fe != null)
                                fe.RemoveHandler(attachedEvent, GetRoutedEventMethod());
                        }
                    }
                }
            }
        }

        private Delegate _method;
        private Delegate GetEventMethod(EventInfo ei)
        {
            if (ei == null)
                throw new ArgumentNullException("ei");
            if (ei.EventHandlerType == null)
                throw new ArgumentException("EventHandlerType is null");
            return _method ?? (_method = Delegate.CreateDelegate(ei.EventHandlerType, this, GetType().GetMethod("OnEventRaised", BindingFlags.NonPublic | BindingFlags.Instance)));
        }

        private Delegate GetRoutedEventMethod()
        {
            return _method ?? (_method = Delegate.CreateDelegate(typeof(RoutedEventHandler), this, GetType().GetMethod("OnEventRaised", BindingFlags.NonPublic | BindingFlags.Instance)));
        }

        /// <summary>
        /// This is invoked by the event - it invokes the command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        // ReSharper disable UnusedMember.Local
        private void OnEventRaised(object sender, EventArgs e)
        // ReSharper restore UnusedMember.Local
        {
            if (Command != null)
            {
                var ep = new EventParameters(Command, sender, e, CommandParameter);
                if (Command.CanExecute(ep))
                    Command.Execute(ep);
            }
            else
            {
                Debug.WriteLine(string.Format("Missing Command on event handler, {0}: Sender={1}, EventArgs={2}", Event, sender, e));
            }
        }

        /// <summary>
        /// When implemented in a derived class, creates a new instance of the <see cref="T:System.Windows.Freezable"/> derived class. 
        /// </summary>
        /// <returns>
        /// The new instance.
        /// </returns>
        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Collection of command to event mappings
    /// </summary>
    public class CommandEventCollection : FreezableCollection<CommandEvent>
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
            foreach(var item in this)
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
                    _currentList.ForEach(i => i.Unsubscribe(_target));
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

    /// <summary>
    /// This class manages a collection of command to event mappings.  It is used to wire up View events to a
    /// ViewModel ICommand implementation.  Note that if it is lifetime events (Loaded, Activated, Closing, Closed, etc.)
    /// then you should use the LifetimeEvents behavior instead.  This is for other input events to be tied to the 
    /// ViewModel without codebehind.
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// 
    /// <Behaviors:EventCommander.Mappings>
    ///    <Behaviors:CommandEvent Command="{Binding MouseEnterCommand}" Event="MouseEnter" />
    ///    <Behaviors:CommandEvent Command="{Binding MouseLeaveCommand}" Event="MouseLeave" />
    /// </Behaviors:EventCommander.Mappings>
    /// 
    /// ]]>
    /// </example>
    public static class EventCommander
    {
        // Make it internal so WPF ignores the property and always uses the public getter/setter.  This is per
        // John Gossman blog post - 07/2008.
        internal static readonly DependencyProperty MappingsProperty = DependencyProperty.RegisterAttached("InternalMappings", 
                            typeof(CommandEventCollection), typeof(EventCommander),
                            new UIPropertyMetadata(null, OnMappingsChanged));

        /// <summary>
        /// Retrieves the mapping collection
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static CommandEventCollection InternalGetMappingCollection(DependencyObject obj)
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
        /// This retrieves the mapping collection
        /// </summary>
        /// <param name="obj">Dependency Object</param>
        /// <returns>Mapping collection</returns>
        public static CommandEventCollection GetMappings(DependencyObject obj)
        {
            return InternalGetMappingCollection(obj);
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
