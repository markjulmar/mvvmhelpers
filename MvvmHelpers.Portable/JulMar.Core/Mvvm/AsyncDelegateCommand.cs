using System;
using System.Threading;
using System.Threading.Tasks;
using JulMar.Interfaces;

namespace JulMar.Mvvm
{
    /// <summary>
    /// An asynchronous version of the DelegatingCommand
    /// </summary>
    public sealed class AsyncDelegateCommand : IDelegateCommand
    {
        private readonly Action<object> _execute;
        private readonly Action _completedCallback;
        private readonly Action<Exception> _errorCallback; 
        private readonly Func<object, bool> _canExecute;
        private bool _isBusy;

        /// <summary>
        /// Event that is raised when the current state for our command has changed.
        /// </summary>
        public event EventHandler CanExecuteChanged = delegate { };

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action">The action to be executed</param>
        public AsyncDelegateCommand(Action action) : this(action, null, null, null)
        {
        }

        /// <summary>
        /// Constructor for no-parameter ICommand
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        public AsyncDelegateCommand(Action action, Func<bool> canExecute) : this(action, canExecute, null, null)
        {
        }

        /// <summary>
        /// Constructor for no-parameter ICommand
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        /// <param name="completedCallback">Will be invoked when the action is completed</param>
        public AsyncDelegateCommand(Action action, Func<bool> canExecute, Action completedCallback) : this(action, canExecute, completedCallback, null)
        {
        }

        /// <summary>
        /// Constructor for no-parameter ICommand
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        /// <param name="completedCallback">Will be invoked when the action is completed</param>
        /// <param name="errorCallback">Will be invoked if the action throws an error</param>
        public AsyncDelegateCommand(Action action, Func<bool> canExecute, Action completedCallback, Action<Exception> errorCallback)
        {
            if (action == null)
                throw new ArgumentNullException("action", "Command action must be supplied.");

            this._execute = delegate { action(); };
            this._completedCallback = completedCallback;
            this._errorCallback = errorCallback;

            if (canExecute != null)
            {
                this._canExecute = delegate { return canExecute(); };
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action">The action to be executed</param>
        public AsyncDelegateCommand(Action<object> action) : this(action, null, null, null)
        {
        }

        /// <summary>
        /// Constructor for no-parameter ICommand
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        public AsyncDelegateCommand(Action<object> action, Func<object,bool> canExecute)
            : this(action, canExecute, null, null)
        {
        }

        /// <summary>
        /// Constructor for no-parameter ICommand
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        /// <param name="completedCallback">Will be invoked when the action is completed</param>
        public AsyncDelegateCommand(Action<object> action, Func<object, bool> canExecute, Action completedCallback)
            : this(action, canExecute, completedCallback, null)
        {
        }

        /// <summary>
        /// Constructor for object-based parameter ICommand
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        /// <param name="completedCallback">Will be invoked when the action is completed</param>
        /// <param name="errorCallback">Will be invoked if the action throws an error</param>
        public AsyncDelegateCommand(Action<object> action, Func<object, bool> canExecute, Action completedCallback, Action<Exception> errorCallback)
        {
            if (action == null)
                throw new ArgumentNullException("action", "Command action must be supplied.");

            this._execute = action;
            this._completedCallback = completedCallback;
            this._errorCallback = errorCallback;
            this._canExecute = canExecute;
        }

        /// <summary>
        /// This method can be used to raise the CanExecuteChanged handler.
        /// This will force WinRT to re-query the status of this command directly.
        /// This is not necessary if you use the AutoCanExecuteRequery feature.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            this.CanExecuteChanged(this, EventArgs.Empty);
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
            return (this._canExecute == null) ? !this._isBusy : !this._isBusy && this._canExecute(parameter);
        }

        /// <summary>
        /// Invoke the command
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            this._isBusy = true;
            this.RaiseCanExecuteChanged();

            // Spin up the task.
            Task parentTask = Task.Factory.StartNew(this._execute, parameter, CancellationToken.None,
                TaskCreationOptions.LongRunning, TaskScheduler.Default);

            // Do the cleanup on the UI thread.
            parentTask.ContinueWith(t =>
            {
                if (this._completedCallback != null && t.Exception == null)
                    this._completedCallback();
                if (this._errorCallback != null && t.Exception != null)
                    this._errorCallback(t.Exception);

                this._isBusy = false;
                this.RaiseCanExecuteChanged();

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }

    /// <summary>
    /// An asynchronous version of the DelegatingCommand
    /// </summary>
    public sealed class AsyncDelegateCommand<T> : IDelegateCommand
    {
        private readonly Action<T> _execute;
        private readonly Action _completedCallback;
        private readonly Action<Exception> _errorCallback;
        readonly Func<T, bool> _canExecute;
        private bool _isBusy;

        /// <summary>
        /// Event that is raised when the current state for our command has changed.
        /// Note that this is an instance event - unlike the CommandManager.RequerySuggested event
        /// and as such we don't need to manage weak references here.
        /// </summary>
        public event EventHandler CanExecuteChanged = delegate { };

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action">The action to be executed</param>
        public AsyncDelegateCommand(Action<T> action) : this(action, null, null, null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        public AsyncDelegateCommand(Action<T> action, Func<T,bool> canExecute)
            : this(action, canExecute, null, null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        /// <param name="completedCallback">Will be invoked when the action is completed</param>
        public AsyncDelegateCommand(Action<T> action, Func<T,bool> canExecute, Action completedCallback)
            : this(action, canExecute, completedCallback, null)
        {
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        /// <param name="completedCallback">Will be invoked when the action is completed</param>
        /// <param name="errorCallback">Will be invoked if the action throws an error</param>
        public AsyncDelegateCommand(Action<T> action, Func<T,bool> canExecute, Action completedCallback, Action<Exception> errorCallback)
        {
            this._execute = action;
            this._completedCallback = completedCallback;
            this._errorCallback = errorCallback;
            this._canExecute = canExecute;
        }

        /// <summary>
        /// This method can be used to raise the CanExecuteChanged handler.
        /// This will force WinRT to re-query the status of this command directly.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            this.CanExecuteChanged(this, EventArgs.Empty);
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
            return (this._canExecute == null) ? !this._isBusy : !this._isBusy && this._canExecute((T)parameter);
        }

        /// <summary>
        /// Invoke the command
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            this._isBusy = true;
            this.RaiseCanExecuteChanged();

            Task parentTask = Task.Factory.StartNew(o => this._execute((T)o), parameter, CancellationToken.None,
                TaskCreationOptions.LongRunning, TaskScheduler.Default);
            parentTask.ContinueWith(t =>
            {
                if (this._completedCallback != null && t.Exception == null)
                    this._completedCallback();
                if (this._errorCallback != null && t.Exception != null)
                    this._errorCallback(t.Exception);
                
                this._isBusy = false;
                this.RaiseCanExecuteChanged();

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}