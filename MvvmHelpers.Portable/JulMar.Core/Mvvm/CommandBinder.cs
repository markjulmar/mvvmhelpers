using System;
using System.Linq;
using System.Windows.Input;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

namespace JulMar.Mvvm
{
	/// <summary>
	/// This class provides a wrapper for a single event to command binding.
	/// </summary>
	internal class EventCommandBinding
	{
        private Delegate method;
        private ICommand command;

		/// <summary>
		/// The event name we are bound to.
		/// </summary>
		public string Event { get; set; }

	    /// <summary>
	    /// The command to execute.
	    /// </summary>
	    public ICommand Command
	    {
	        get
	        {
	            return this.command;
	        }
	        set
	        {
                if (this.command != value)
                {
                    if (this.command != null)
                        this.command.CanExecuteChanged -= this.OnCommandStateChanged;
                    this.command = value;
                    if (this.command != null)
                        this.command.CanExecuteChanged += this.OnCommandStateChanged;
                }
	        }
	    }

	    /// <summary>
	    /// The Command Parameter to pass to the event.
	    /// </summary>
	    public Func<object> CommandParameter { get; set; }

        /// <summary>
        /// This is called when the command state is changed.
        /// </summary>
        public Action<bool> CommandStateChanged { get; set; }

		/// <summary>
		/// Wires up an event to the target
		/// </summary>
		/// <param name="target"></param>
		internal void Subscribe(object target)
		{
			if (target != null)
			{
			    EventInfo ei = target.GetType().GetRuntimeEvent(Event);
				if (ei != null)
				{
					ei.RemoveEventHandler(target, GetEventMethod(ei));
					ei.AddEventHandler(target, GetEventMethod(ei));
					return;
				}
			}

			Debug.WriteLine("Unable to locate event {0} on {1}", Event, target);

            // Invoke the CanExecuteChanged handler so it's setup initially.
            if (Command != null)
                this.OnCommandStateChanged(Command, EventArgs.Empty);
		}

        /// <summary>
        /// This is called when the command state is changed - it raises the CommandStateChanged
        /// action which will enable/disable elements.
        /// </summary>
        /// <param name="sender">Command</param>
        /// <param name="eventArgs">Empty event args</param>
	    private void OnCommandStateChanged(object sender, EventArgs eventArgs)
	    {
            var csc = CommandStateChanged;
            if (csc != null)
            {
                var cmd = sender as ICommand;
                if (cmd != null)
                {
                    object parameter = (CommandParameter == null) ? null : CommandParameter();
                    csc.Invoke(cmd.CanExecute(parameter));
                }
            }
	    }

	    /// <summary>
		/// Unwires target event
		/// </summary>
		/// <param name="target"></param>
		internal void Unsubscribe(object target)
		{
			if (target != null)
			{
                EventInfo ei = target.GetType().GetRuntimeEvent(Event);
				if (ei != null)
					ei.RemoveEventHandler(target, GetEventMethod(ei));
			}

            Command = null;
		}

	    private Delegate GetEventMethod(EventInfo ei)
		{
			if (ei == null)
				throw new ArgumentNullException("ei");
			if (ei.EventHandlerType == null)
				throw new ArgumentException("EventHandlerType is null");

		    if (this.method == null)
		    {
		        MethodInfo eventHandler = GetType().GetRuntimeMethods().First(mi => mi.Name == "_OnEventRaised");
		        this.method = eventHandler.CreateDelegate(ei.EventHandlerType, this);
		    }

		    return this.method;
		}

		/// <summary>
		/// This is invoked by the event - it invokes the command.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void _OnEventRaised(object sender, EventArgs e)
		{
			if (Command != null)
			{
				object parameter = (CommandParameter == null) ? null : CommandParameter();
				if (Command.CanExecute(parameter))
					Command.Execute(parameter);
			}
			else
			{
				Debug.WriteLine("Missing Command on event handler, {0}: Sender={1}, EventArgs={2}", Event, sender, e);
			}
		}
    }

    /// <summary>
    /// This class is used to bind a command to an event on a type.
    /// </summary>
	public class CommandBinder : IDisposable
	{
		private readonly List<Tuple<WeakReference, EventCommandBinding>> _handlers = new List<Tuple<WeakReference, EventCommandBinding>>();

        /// <summary>
        /// Constructor
        /// </summary>
	    private CommandBinder()
	    {
	        
	    }

        /// <summary>
        /// Creates a new CommandBinder.
        /// </summary>
        /// <returns>Command Binder instance</returns>
		public static CommandBinder Create()
		{
			return new CommandBinder();
		}

        /// <summary>
        /// Adds a new command binding to the list.
        /// </summary>
        /// <param name="target">Object exposing event</param>
        /// <param name="eventName">Name of the event</param>
        /// <param name="command">Command to bind</param>
        /// <param name="commandParameter">Command Parameter function</param>
        /// <param name="stateChanged">Called when the command state changes.</param>
        /// <returns>Command Binder instance</returns>
        public CommandBinder Add(object target, string eventName, ICommand command, Func<object> commandParameter = null, Action<bool> stateChanged = null)
		{
			EventCommandBinding ce = new EventCommandBinding() {
				Event = eventName,
				Command = command,
				CommandParameter = commandParameter,
                CommandStateChanged = stateChanged,
			};
			ce.Subscribe(target);
			_handlers.Add(Tuple.Create(new WeakReference(target), ce));

			return this;
		}

        /// <summary>
        /// Disposes of the list of handlers.
        /// </summary>
		public void Dispose()
		{
			foreach (var entry in _handlers)
			{
				if (entry.Item1.IsAlive) {
					object target = entry.Item1.Target;
					entry.Item2.Unsubscribe(target);
				}
			}
			_handlers.Clear();
		}
	}
}

