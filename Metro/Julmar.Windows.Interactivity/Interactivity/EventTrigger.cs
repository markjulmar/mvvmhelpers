using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.ApplicationModel;

namespace System.Windows.Interactivity
{
    /// <summary>
    /// A Blend Behavior trigger driven off an event.
    /// This was taken and modified from the System.Windows.Interactivity support and may be removed
    /// once true Blend behaviors are shipped.
    /// </summary>
    public class EventTrigger : TriggerBase<FrameworkElement>
    {
        private Delegate _method;

        /// <summary>
        /// EventName dependency property
        /// </summary>
        public static readonly DependencyProperty EventNameProperty = DependencyProperty.Register("EventName", 
            typeof(string), typeof(EventTrigger), new PropertyMetadata("Loaded", OnEventNameChanged));

        /// <summary>
        /// Name of the event to monitor which fires actions
        /// </summary>
        public string EventName
        {
            get
            {
                return (string)base.GetValue(EventNameProperty);
            }
            set
            {
                base.SetValue(EventNameProperty, value);
            }
        }

        /// <summary>
        /// Property change handler for EventName property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnEventNameChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            ((EventTrigger)sender).OnEventNameChanged((string)args.OldValue, (string)args.NewValue);
        }

        /// <summary>
        /// Override called when behavior is attached
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            RegisterEvent(AssociatedObject, EventName);
        }

        /// <summary>
        /// Override called when behavior is detached
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            UnregisterEvent(AssociatedObject, EventName);
        }

        /// <summary>
        /// This invokes the actions when the event is raised.
        /// </summary>
        /// <param name="eventArgs"></param>
        protected virtual void OnEvent(EventArgs eventArgs)
        {
            base.InvokeActions(eventArgs);
        }

        /// <summary>
        /// This method registers the event handler for the specific named event
        /// </summary>
        /// <param name="oldEventName"></param>
        /// <param name="newEventName"></param>
        internal void OnEventNameChanged(string oldEventName, string newEventName)
        {
            if (DesignMode.DesignModeEnabled)
                return;

            if (base.AssociatedObject != null)
            {
                if (oldEventName != newEventName)
                {
                    if (!string.IsNullOrEmpty(oldEventName))
                        UnregisterEvent(AssociatedObject, oldEventName);
                    if (!string.IsNullOrEmpty(newEventName))
                        RegisterEvent(AssociatedObjectInternal, newEventName);
                }
            }
        }

        /// <summary>
        /// Register an event handler for a given object and event name.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eventName"></param>
        private void RegisterEvent(object obj, string eventName)
        {
            EventInfo eventInfo = LookForEventDeclaration(obj, eventName);
            if (eventInfo == null)
                throw new ArgumentException("Cannot find event " + eventName);

            // WinRT events cannot be subscribed to through normal events because it uses
            // event tokens to map events to handlers. The compiler emits calls through WRM to do the work.
            var handler = GetEventMethod(eventInfo);
            WindowsRuntimeMarshal.AddEventHandler(
                d => (EventRegistrationToken)eventInfo.AddMethod.Invoke(obj, new object[] { d }),
                et => eventInfo.RemoveMethod.Invoke(obj, new object[] { et }), handler);
        }

        /// <summary>
        /// Unregister an event handler for a given object and event name.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eventName"></param>
        private void UnregisterEvent(object obj, string eventName)
        {
            EventInfo eventInfo = LookForEventDeclaration(obj, eventName);
            if (eventInfo != null)
            {
                var handler = GetEventMethod(eventInfo);
                WindowsRuntimeMarshal.RemoveEventHandler(
                    et => eventInfo.RemoveMethod.Invoke(obj, new object[] {et}), handler);
            }
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
        /// This is invoked by the event - it runs all the actions
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

            this.OnEvent(e as EventArgs);
        }
    }
}
