using Windows.ApplicationModel;
using Windows.UI.Xaml.Markup;
using Microsoft.Xaml.Interactivity;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;

namespace JulMar.Windows.Interactivity
{
    /// <summary>
    /// This is a Blend trigger that binds to an event within a ViewModel and invokes actions when it 
    /// is raised by the ViewModel.  This allows the VM to trigger behaviors in the View easily.
    /// </summary>
    [ContentProperty(Name = "Actions")]
    public class ViewModelTriggerBehavior : DependencyObject, IBehavior
    {
        /// <summary>
        /// The DependencyProperty used to hold the target object (VM).
        /// </summary>
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", 
            typeof(object), typeof(ViewModelTriggerBehavior), new PropertyMetadata(null, OnTargetChanged));

        /// <summary> 
        /// Object holding event
        /// </summary>
        public object Target
        {
            get { return GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty used to hold the IViewModelTrigger.
        /// </summary>
        public static readonly DependencyProperty EventNameProperty = DependencyProperty.Register("EventName", 
            typeof(string), typeof(ViewModelTriggerBehavior), new PropertyMetadata(null, OnEventNameChanged));

        /// <summary> 
        /// Name of the event to hook into.
        /// </summary>
        public string EventName
        {
            get { return (string)GetValue(EventNameProperty); }
            set { SetValue(EventNameProperty, value); }
        }

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
        /// Change handler for the event name
        /// </summary>
        /// <param name="dpo">VMTrigger object</param>
        /// <param name="e">EventArgs</param>
        private static void OnEventNameChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            ViewModelTriggerBehavior vmt = (ViewModelTriggerBehavior) dpo;
            if (vmt.Target == null)
                return;

            string oldEventName = e.OldValue as string;
            if (!string.IsNullOrEmpty(oldEventName))
                vmt.UnregisterEvent(vmt.Target, oldEventName);
            vmt.RegisterEvent(vmt.Target, vmt.EventName);
        }

        /// <summary>
        /// Change handler for the event name
        /// </summary>
        /// <param name="dpo">VMTrigger object</param>
        /// <param name="e">EventArgs</param>
        private static void OnTargetChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            ViewModelTriggerBehavior vmt = (ViewModelTriggerBehavior)dpo;
            if (string.IsNullOrEmpty(vmt.EventName))
                return;

            object oldTarget = e.OldValue;
            if (oldTarget != null)
                vmt.UnregisterEvent(oldTarget, vmt.EventName);
            vmt.RegisterEvent(vmt.Target, vmt.EventName);
        }

        /// <summary>
        /// Register an event handler
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eventName"></param>
        private void RegisterEvent(object obj, string eventName)
        {
            EventInfo eventInfo = LookForEventDeclaration(obj, eventName);
            if (eventInfo == null)
                throw new ArgumentException("Cannot find event " + eventName);
            var handler = GetEventMethod(eventInfo);

            // WinRT events cannot be subscribed to through normal events because it uses
            // event tokens to map events to handlers. The compiler emits calls through WRM to do the work.
            if (eventInfo.DeclaringType.GetTypeInfo().Attributes.HasFlag(TypeAttributes.WindowsRuntime))
            {
                WindowsRuntimeMarshal.AddEventHandler(
                    d => (EventRegistrationToken) eventInfo.AddMethod.Invoke(obj, new object[] {d}),
                    et => eventInfo.RemoveMethod.Invoke(obj, new object[] {et}), handler);
            }
            else
            {
                eventInfo.AddEventHandler(obj, handler);
            }
        }

        /// <summary>
        /// Unregister an event handler
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eventName"></param>
        private void UnregisterEvent(object obj, string eventName)
        {
            EventInfo eventInfo = LookForEventDeclaration(obj, eventName);
            if (eventInfo != null)
            {
                var handler = GetEventMethod(eventInfo);
                if (eventInfo.DeclaringType.GetTypeInfo().Attributes.HasFlag(TypeAttributes.WindowsRuntime))
                {
                    WindowsRuntimeMarshal.RemoveEventHandler(
                        et => eventInfo.RemoveMethod.Invoke(obj, new object[] {et}), handler);
                }
                else
                {
                    eventInfo.RemoveEventHandler(obj, handler);
                }
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

            int paramCount = ei.EventHandlerType.GetTypeInfo().DeclaredMethods.First(mi => mi.Name == "Invoke").GetParameters().Length;
            if (paramCount == 0)
            {
                MethodInfo methodInfo = typeof(ViewModelTriggerBehavior).GetTypeInfo().DeclaredMethods.FirstOrDefault(
                        mi => mi.Name == "OnEventRaisedWithNoParameter" && mi.IsPrivate && !mi.IsStatic);
                return methodInfo.CreateDelegate(ei.EventHandlerType, this);
            }
            else if (paramCount == 1)
            {
                MethodInfo methodInfo = typeof(ViewModelTriggerBehavior).GetTypeInfo().DeclaredMethods.FirstOrDefault(
                        mi => mi.Name == "OnEventRaisedWithParameter" && mi.IsPrivate && !mi.IsStatic);
                return methodInfo.CreateDelegate(ei.EventHandlerType, this);
            }
            else
                throw new NotSupportedException("Cannot bind to events with more than one parameter");
        }

        /// <summary>
        /// This is invoked by the event - it runs all the actions
        /// </summary>
        private void OnEventRaisedWithParameter(object parameter)
        {
            this.InvokeActions(parameter);
        }

        /// <summary>
        /// This is invoked by the event - it runs all the actions
        /// </summary>
        private void OnEventRaisedWithNoParameter()
        {
            // Invoke our actions
            this.InvokeActions(null);
        }

        /// <summary>
        /// Method to invoke our actions.
        /// </summary>
        /// <param name="parameter"></param>
        private void InvokeActions(object parameter)
        {
            Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
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
                if (Target != null && !string.IsNullOrEmpty(EventName))
                    RegisterEvent(Target, EventName);
            }
        }

        /// <summary>
        /// Detaches this instance from its associated object.
        /// </summary>
        public void Detach()
        {
            if (Target != null && !string.IsNullOrEmpty(EventName))
                UnregisterEvent(Target, EventName);
            AssociatedObject = null;
        }

        /// <summary>
        /// Gets the <see cref="T:Windows.UI.Xaml.DependencyObject"/> to which the <seealso cref="T:Microsoft.Xaml.Interactivity.IBehavior"/> is attached.
        /// </summary>
        public DependencyObject AssociatedObject { get; private set; }
    }
}
