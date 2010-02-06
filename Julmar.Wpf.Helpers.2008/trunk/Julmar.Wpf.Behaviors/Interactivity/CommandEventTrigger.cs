using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using JulMar.Windows.Behaviors;

namespace JulMar.Windows.Interactivity
{
    /// <summary>
    /// This is an attached behavior for Blend 3 that allows a Event -> Command trigger
    /// </summary>
    public class CommandEventTrigger : TriggerBase<FrameworkElement>
    {
        private CommandEvent _commandEvent;

        /// <summary>
        /// Command Property Dependency Property
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(CommandEventTrigger), new UIPropertyMetadata(null));

        /// <summary>
        /// Parameter for the ICommand
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(CommandEventTrigger), new UIPropertyMetadata(null, OnParameterChanged));

        /// <summary>
        /// Event Dependency Property
        /// </summary>
        public static readonly DependencyProperty EventProperty = DependencyProperty.Register("Event", typeof(string), typeof(CommandEventTrigger), new UIPropertyMetadata(string.Empty));

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
        /// This is called when the command parameter changes -- this can
        /// happen if it is databound.
        /// </summary>
        /// <param name="dpo">DependencyObject</param>
        /// <param name="e">Change</param>
        private static void OnParameterChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            CommandEventTrigger ce = (CommandEventTrigger)dpo;
            if (ce._commandEvent != null)
                ce._commandEvent.CommandParameter = e.NewValue;
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();

            if (!AssociatedObject.IsLoaded)
            {
                RoutedEventHandler loadedEvent = null;
                loadedEvent = (s, e) => {
                                      AssociatedObject.Loaded -= loadedEvent;
                                      HookEvents();
                                  };
                AssociatedObject.Loaded += loadedEvent;
            }
            else HookEvents();
        }

        /// <summary>
        /// Hooks the event maps
        /// </summary>
        private void HookEvents()
        {
            var mappings = EventCommander.GetMappings(AssociatedObject);
            if (mappings == null)
            {
                mappings = new CommandEventCollection();
                EventCommander.SetMappings(AssociatedObject, mappings);
            }

            _commandEvent = new CommandEvent { Command = this.Command, CommandParameter = this.CommandParameter, Event = this.Event };
            mappings.Add(_commandEvent);
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>
        /// Override this to unhook functionality from the AssociatedObject.
        /// </remarks>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            var mappings = EventCommander.GetMappings(AssociatedObject);
            if (mappings != null && _commandEvent != null)
            {
                mappings.Remove(_commandEvent);
            }
        }
    }
}
