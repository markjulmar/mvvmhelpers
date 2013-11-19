using System;
using System.Reflection;
using System.Windows;
using System.Windows.Interactivity;

namespace JulMar.Windows.Interactivity
{
    /// <summary>
    /// This is a Blend trigger that binds to a IViewModelTrigger and invokes actions when it 
    /// is raised by the ViewModel.  This allows the VM to trigger behaviors in the View easily.
    /// </summary>
    public class ViewModelTrigger : TriggerBase<UIElement>
    {
        /// <summary>
        /// The DependencyProperty used to hold the target object (VM).
        /// </summary>
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(object), typeof(ViewModelTrigger), new FrameworkPropertyMetadata(null, OnTargetChanged));

        /// <summary> 
        /// Object holding event
        /// </summary>
        public object Target
        {
            get { return this.GetValue(TargetProperty); }
            set { this.SetValue(TargetProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty used to hold the IViewModelTrigger.
        /// </summary>
        public static readonly DependencyProperty EventNameProperty = DependencyProperty.Register("EventName", typeof(string), typeof(ViewModelTrigger), new FrameworkPropertyMetadata(null, OnEventNameChanged));

        /// <summary> 
        /// Name of the event to hook into.
        /// </summary>
        public string EventName
        {
            get { return (string)this.GetValue(EventNameProperty); }
            set { this.SetValue(EventNameProperty, value); }
        }

        /// <summary>
        /// This is called when the trigger is attached
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.Register(this.Target, this.EventName);
        }

        /// <summary>
        /// Called when the trigger is detached
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.Unregister(this.Target, this.EventName);
        }

        /// <summary>
        /// Change handler for the event name
        /// </summary>
        /// <param name="dpo">VMTrigger object</param>
        /// <param name="e">EventArgs</param>
        private static void OnEventNameChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            ViewModelTrigger vmt = (ViewModelTrigger) dpo;

            string oldEventName = e.OldValue as string;
            if (!string.IsNullOrEmpty(oldEventName))
                vmt.Unregister(vmt.Target, oldEventName);
            vmt.Register(vmt.Target, vmt.EventName);
        }

        /// <summary>
        /// Change handler for the event name
        /// </summary>
        /// <param name="dpo">VMTrigger object</param>
        /// <param name="e">EventArgs</param>
        private static void OnTargetChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            ViewModelTrigger vmt = (ViewModelTrigger)dpo;

            object oldTarget = e.OldValue;
            if (oldTarget != null)
                vmt.Unregister(oldTarget, vmt.EventName);
            vmt.Register(vmt.Target, vmt.EventName);
        }

        /// <summary>
        /// Method to hook into event chain by name.
        /// </summary>
        /// <param name="target">Target object</param>
        /// <param name="eventName">Event name</param>
        private void Register(object target, string eventName)
        {
            if (target == null || string.IsNullOrEmpty(eventName))
                return;

            Type targetType = target.GetType();
            EventInfo ei = targetType.GetEvent(eventName);

            if (ei != null)
            {
                int paramCount = ((MethodInfo) ei.EventHandlerType.GetMember("Invoke")[0]).GetParameters().Length;
                if (paramCount == 1)
                {
                    var handler = Delegate.CreateDelegate(ei.EventHandlerType, this, "RaiseTriggerWithParameter");
                    ei.RemoveEventHandler(target, handler);
                    ei.AddEventHandler(target, handler);
                }
                else if (paramCount == 0)
                {
                    var handler = Delegate.CreateDelegate(ei.EventHandlerType, this, "RaiseTriggerNoParams");
                    ei.RemoveEventHandler(target, handler);
                    ei.AddEventHandler(target, handler);
                }
                else
                    throw new NotSupportedException("Cannot bind to events with more than one parameter");
            }
        }

        /// <summary>
        /// Method to unhook event
        /// </summary>
        /// <param name="target">Target</param>
        /// <param name="eventName">Event Name</param>
        private void Unregister(object target, string eventName)
        {
            if (target == null || string.IsNullOrEmpty(eventName))
                return;

            Type targetType = target.GetType();
            EventInfo ei = targetType.GetEvent(eventName);
            if (ei != null)
            {
                int paramCount = ((MethodInfo)ei.EventHandlerType.GetMember("Invoke")[0]).GetParameters().Length;
                if (paramCount == 1)
                {
                    var handler = Delegate.CreateDelegate(ei.EventHandlerType, this, "RaiseTriggerWithParameter");
                    ei.RemoveEventHandler(target, handler);
                }
                else if (paramCount == 0)
                {
                    var handler = Delegate.CreateDelegate(ei.EventHandlerType, this, "RaiseTriggerNoParams");
                    ei.RemoveEventHandler(target, handler);
                }
                else
                    throw new NotSupportedException("Cannot bind to events with more than one parameter");
            }
        }

        /// <summary>
        /// This is called when the trigger occurs.
        /// </summary>
        private void RaiseTriggerNoParams()
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.Invoke((Action)this.RaiseTriggerNoParams);
                return;
            }
            this.InvokeActions(null);
        }

        /// <summary>
        /// This is called when the trigger occurs.
        /// </summary>
        /// <param name="parameter"></param>
        private void RaiseTriggerWithParameter(object parameter)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.Invoke((Action)(() => this.RaiseTriggerWithParameter(parameter)));
                return;
            }
            this.InvokeActions(parameter);
        }
    }
}
