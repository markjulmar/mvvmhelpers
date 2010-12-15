using System;
using System.ComponentModel;
using System.Windows.Input;

namespace JulMar.Windows.Mvvm
{
    /// <summary>
    /// An asynchronous version of the DelegatingCommand
    /// </summary>
    public class AsyncDelegatingCommand : ICommand 
    {
        readonly BackgroundWorker _worker = new BackgroundWorker();
        readonly Func<object, bool> _canExecute;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action">The action to be executed</param>
        public AsyncDelegatingCommand(Action action)
            : this(action, null, null, null)
        {
        }

        /// <summary>
        /// Constructor for no-parameter ICommand
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        public AsyncDelegatingCommand(Action action, Func<bool> canExecute) : this(action, canExecute, null, null)
        {
        }

        /// <summary>
        /// Constructor for no-parameter ICommand
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        /// <param name="completedCallback">Will be invoked when the action is completed</param>
        public AsyncDelegatingCommand(Action action, Func<bool> canExecute, Action<object> completedCallback) : this(action, canExecute, completedCallback, null)
        {
        }

        /// <summary>
        /// Constructor for no-parameter ICommand
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        /// <param name="completedCallback">Will be invoked when the action is completed</param>
        /// <param name="errorCallback">Will be invoked if the action throws an error</param>
        public AsyncDelegatingCommand(Action action, Func<bool> canExecute, Action<object> completedCallback, Action<Exception> errorCallback)
        {
            _worker.DoWork += (s, e) =>
            {
                CommandManager.InvalidateRequerySuggested();
                action();
            };

            _worker.RunWorkerCompleted += (s, e) =>
            {
                if (completedCallback != null && e.Error == null)
                    completedCallback(e.Result);
                if (errorCallback != null && e.Error != null)
                    errorCallback(e.Error);
                CommandManager.InvalidateRequerySuggested();
            };

            if (canExecute != null)
                _canExecute = delegate { return canExecute(); };
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action">The action to be executed</param>
        public AsyncDelegatingCommand(Action<object> action)
            : this(action, null, null, null)
        {
        }

        /// <summary>
        /// Constructor for no-parameter ICommand
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        public AsyncDelegatingCommand(Action<object> action, Func<object,bool> canExecute)
            : this(action, canExecute, null, null)
        {
        }

        /// <summary>
        /// Constructor for no-parameter ICommand
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        /// <param name="completedCallback">Will be invoked when the action is completed</param>
        public AsyncDelegatingCommand(Action<object> action, Func<object, bool> canExecute, Action<object> completedCallback)
            : this(action, canExecute, completedCallback, null)
        {
        }

        /// <summary>
        /// Constructor for object-based parameter ICommand
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        /// <param name="completed">Will be invoked when the action is completed</param>
        /// <param name="error">Will be invoked if the action throws an error</param>
        public AsyncDelegatingCommand(Action<object> action, Func<object, bool> canExecute, Action<object> completed, Action<Exception> error)
        {
            _worker.DoWork += (s, e) =>
                {
                    CommandManager.InvalidateRequerySuggested();
                    action(e.Argument);
                };

            _worker.RunWorkerCompleted += (s, e) =>
                {
                    if (completed != null && e.Error == null)
                        completed(e.Result);
                    if (error != null && e.Error != null)
                        error(e.Error);
                    CommandManager.InvalidateRequerySuggested();
                };

            _canExecute = canExecute;
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
            return (_canExecute == null) ? !_worker.IsBusy : !_worker.IsBusy && _canExecute(parameter);    
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// We always register (unlike DelegatingCommand) since the BackgroundWorker state
        /// affects our execution state.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; } 
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Invoke the command
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            _worker.RunWorkerAsync(parameter); 
        }
    }

    /// <summary>
    /// An asynchronous version of the DelegatingCommand
    /// </summary>
    public class AsyncDelegatingCommand<T> : ICommand
    {
        readonly BackgroundWorker _worker = new BackgroundWorker();
        readonly Func<T, bool> _canExecute;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action">The action to be executed</param>
        public AsyncDelegatingCommand(Action<T> action)
            : this(action, null, null, null)
        {
        }

        /// <summary>
        /// Constructor for no-parameter ICommand
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        public AsyncDelegatingCommand(Action<T> action, Func<T, bool> canExecute)
            : this(action, canExecute, null, null)
        {
        }

        /// <summary>
        /// Constructor for no-parameter ICommand
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        /// <param name="completedCallback">Will be invoked when the action is completed</param>
        public AsyncDelegatingCommand(Action<T> action, Func<T, bool> canExecute, Action<object> completedCallback)
            : this(action, canExecute, completedCallback, null)
        {
        }

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        /// <param name="completed">Will be invoked when the action is completed</param>
        /// <param name="error">Will be invoked if the action throws an error</param>
        public AsyncDelegatingCommand(Action<T> action, Func<T, bool> canExecute = null, Action<object> completed = null, Action<Exception> error = null)
        {
            _worker.DoWork += (s, e) =>
            {
                CommandManager.InvalidateRequerySuggested();
                action((T)e.Argument);
            };

            _worker.RunWorkerCompleted += (s, e) =>
            {
                if (completed != null && e.Error == null)
                    completed(e.Result);
                if (error != null && e.Error != null)
                    error(e.Error);
                CommandManager.InvalidateRequerySuggested();
            };
            _canExecute = canExecute;
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
            return (_canExecute == null) ?
                    !(_worker.IsBusy) : !(_worker.IsBusy)
                        && _canExecute((T)parameter);
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// We always register (unlike DelegatingCommand) since the BackgroundWorker state
        /// affects our execution state.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Invoke the command
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            _worker.RunWorkerAsync(parameter);
        }
    }
}