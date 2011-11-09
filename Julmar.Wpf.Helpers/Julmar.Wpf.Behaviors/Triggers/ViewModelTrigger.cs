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
            get { return GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
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
            get { return (string)GetValue(EventNameProperty); }
            set { SetValue(EventNameProperty, value); }
        }

        /// <summary>
        /// This is called when the trigger is attached
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            Register(Target, EventName);
        }

        /// <summary>
        /// Called when the trigger is detached
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            Unregister(Target, EventName);
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
                var handler = Delegate.CreateDelegate(ei.EventHandlerType, this, "RaiseTrigger");
                ei.RemoveEventHandler(target, handler);
                ei.AddEventHandler(target, handler);
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
                var handler = Delegate.CreateDelegate(ei.EventHandlerType, this, "RaiseTrigger");
                ei.RemoveEventHandler(target, handler);
            }
        }

        /// <summary>
        /// This is called when the trigger occurs.
        /// </summary>
        /// <param name="parameter"></param>
        private void RaiseTrigger(object parameter = null)
        {
            InvokeActions(parameter);
        }
    }
}
