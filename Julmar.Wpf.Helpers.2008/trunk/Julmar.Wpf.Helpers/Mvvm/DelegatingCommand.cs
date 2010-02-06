using System;
using System.Diagnostics;
using System.Windows.Input;

namespace JulMar.Windows.Mvvm
{
    /// <summary>
    /// A simple command to delegate forwarding class
    /// </summary>
    public class DelegatingCommand : ICommand
    {
        private readonly Action<object> _command;
        private readonly Func<object, bool> _commandValid;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command">Function mapped to ICommand.Execute</param>
        public DelegatingCommand(Action<object> command) : this(command,null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command">Function mapped to ICommand.Execute</param>
        public DelegatingCommand(Action command) : this(command,null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command">Function mapped to ICommand.Execute</param>
        /// <param name="test">Function mapped to ICommand.CanExecute</param>
        public DelegatingCommand(Action command, Func<bool> test)
        {
            Debug.Assert(command != null);
            _command = delegate { command(); };
            if (test != null)
                _commandValid = delegate { return test(); };
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command">Function mapped to ICommand.Execute</param>
        /// <param name="test">Function mapped to ICommand.CanExecute</param>
        public DelegatingCommand(Action<object> command, Func<object, bool> test)
        {
            Debug.Assert(command != null);
            _command = command;
            _commandValid = test;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                // Register with the WPF CommandManager if we have a delegate that can change
                // the CanExecute value.  This idea came from Josh Smith (http://joshsmithonwpf.wordpress.com/)
                if (_commandValid != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public bool CanExecute(object parameter)
        {
            return (_commandValid == null) ? true : _commandValid(parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            _command(parameter);
        }
    }

    /// <summary>
    /// A simple command to delegate forwarding class which auto-casts the parameter
    /// passed to a given type.
    /// </summary>
    /// <typeparam name="T">Parameter type</typeparam>
    public class DelegatingCommand<T> : ICommand
    {
        private readonly Action<T> _command;
        private readonly Func<T, bool> _commandValid;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command">Function mapped to ICommand.Execute</param>
        public DelegatingCommand(Action<T> command)
            : this(command, null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command">Function mapped to ICommand.Execute</param>
        /// <param name="test">Function mapped to ICommand.CanExecute</param>
        public DelegatingCommand(Action<T> command, Func<T, bool> test)
        {
            Debug.Assert(command != null);
            _command = command;
            _commandValid = test;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                // Register with the WPF CommandManager if we have a delegate that can change
                // the CanExecute value.  This idea came from Josh Smith (http://joshsmithonwpf.wordpress.com/)
                if (_commandValid != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public bool CanExecute(object parameter)
        {
            return (_commandValid == null) ? true : _commandValid((T)parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            _command((T)parameter);
        }
    }
}