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
        private bool _autoCanExecuteRequery;
        private EventHandler _internalCanExecuteChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action">The action to be executed</param>
        public AsyncDelegatingCommand(Action action) : this(action, null, null, null, false)
        {
        }

        /// <summary>
        /// Constructor for no-parameter ICommand
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        public AsyncDelegatingCommand(Action action, Func<bool> canExecute) : this(action, canExecute, null, null, true)
        {
        }

        /// <summary>
        /// Constructor for no-parameter ICommand
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        /// <param name="completedCallback">Will be invoked when the action is completed</param>
        public AsyncDelegatingCommand(Action action, Func<bool> canExecute, Action<object> completedCallback) : this(action, canExecute, completedCallback, null, true)
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
            : this(action, canExecute, completedCallback, errorCallback, true)
        {
        }

        /// <summary>
        /// Constructor for no-parameter ICommand
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        /// <param name="completedCallback">Will be invoked when the action is completed</param>
        /// <param name="errorCallback">Will be invoked if the action throws an error</param>
        /// <param name="autoCanExecuteRequery">True to use WPF CommandManager for CanExecute re-query operations</param>
        public AsyncDelegatingCommand(Action action, Func<bool> canExecute, Action<object> completedCallback, Action<Exception> errorCallback, bool autoCanExecuteRequery)
        {
            _worker.DoWork += (s, e) =>
            {
                if (autoCanExecuteRequery)
                    CommandManager.InvalidateRequerySuggested();
                else
                    RaiseCanExecuteChanged();
                action();
            };

            _worker.RunWorkerCompleted += (s, e) =>
            {
                if (completedCallback != null && e.Error == null)
                    completedCallback(e.Result);
                if (errorCallback != null && e.Error != null)
                    errorCallback(e.Error);
                if (autoCanExecuteRequery)
                    CommandManager.InvalidateRequerySuggested();
                else
                    RaiseCanExecuteChanged();
            };

            if (canExecute != null)
            {
                _canExecute = delegate { return canExecute(); };
                _autoCanExecuteRequery = autoCanExecuteRequery;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action">The action to be executed</param>
        public AsyncDelegatingCommand(Action<object> action)
            : this(action, null, null, null, false)
        {
        }

        /// <summary>
        /// Constructor for no-parameter ICommand
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        public AsyncDelegatingCommand(Action<object> action, Func<object,bool> canExecute)
            : this(action, canExecute, null, null, true)
        {
        }

        /// <summary>
        /// Constructor for no-parameter ICommand
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        /// <param name="completedCallback">Will be invoked when the action is completed</param>
        public AsyncDelegatingCommand(Action<object> action, Func<object, bool> canExecute, Action<object> completedCallback)
            : this(action, canExecute, completedCallback, null, true)
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
            : this(action, canExecute, completed, error, true)
        {
        }

        /// <summary>
        /// Constructor for object-based parameter ICommand
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        /// <param name="completed">Will be invoked when the action is completed</param>
        /// <param name="error">Will be invoked if the action throws an error</param>
        /// <param name="autoCanExecuteRequery">True to use WPF CommandManager for CanExecute re-query operations</param>
        public AsyncDelegatingCommand(Action<object> action, Func<object, bool> canExecute, Action<object> completed, Action<Exception> error, bool autoCanExecuteRequery)
        {
            _worker.DoWork += (s, e) =>
            {
                if (autoCanExecuteRequery)
                    CommandManager.InvalidateRequerySuggested();
                else
                    RaiseCanExecuteChanged();

                action(e.Argument);
            };

            _worker.RunWorkerCompleted += (s, e) =>
            {
                if (completed != null && e.Error == null)
                    completed(e.Result);
                if (error != null && e.Error != null)
                    error(e.Error);
                
                if (autoCanExecuteRequery)
                    CommandManager.InvalidateRequerySuggested();
                else
                    RaiseCanExecuteChanged();
            };

            _canExecute = canExecute;
            _autoCanExecuteRequery = autoCanExecuteRequery;
        }

        /// <summary>
        /// Enable or Disable the automatic CanExecute re-query support using the 
        /// WPF CommandManager.
        /// </summary>
        public bool AutoCanExecuteRequery
        {
            get { return _autoCanExecuteRequery; }
            set
            {
                if (_autoCanExecuteRequery != value)
                {
                    _autoCanExecuteRequery = value;
                    EventHandler eCanExecuteChanged = _internalCanExecuteChanged;
                    if (eCanExecuteChanged != null)
                    {
                        if (_autoCanExecuteRequery)
                        {
                            foreach (EventHandler handler in eCanExecuteChanged.GetInvocationList())
                            {
                                CommandManager.RequerySuggested += handler;
                            }
                        }
                        else
                        {
                            foreach (EventHandler handler in eCanExecuteChanged.GetInvocationList())
                            {
                                CommandManager.RequerySuggested -= handler;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method can be used to raise the CanExecuteChanged handler.
        /// This will force WPF to re-query the status of this command directly.
        /// This is not necessary if you use the AutoCanExecuteRequery feature.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        /// <summary>
        /// This method is used to walk the delegate chain and well WPF that
        /// our command execution status has changed.
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            EventHandler eCanExecuteChanged = _internalCanExecuteChanged;
            if (eCanExecuteChanged != null)
                eCanExecuteChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event that is raised when the current state for our command has changed.
        /// Note that this is an instance event - unlike the CommandManager.RequerySuggested event
        /// and as such we don't need to manage weak references here.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                _internalCanExecuteChanged += value;
                if (_autoCanExecuteRequery)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                _internalCanExecuteChanged -= value;
                if (_autoCanExecuteRequery)
                {
                    CommandManager.RequerySuggested -= value;
                }
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
            return (_canExecute == null) ? !_worker.IsBusy : !_worker.IsBusy && _canExecute(parameter);
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
        private bool _autoCanExecuteRequery;
        private EventHandler _internalCanExecuteChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action">The action to be executed</param>
        public AsyncDelegatingCommand(Action<T> action)
            : this(action, null, null, null, false)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        public AsyncDelegatingCommand(Action<T> action, Func<T,bool> canExecute)
            : this(action, canExecute, null, null, true)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        /// <param name="completedCallback">Will be invoked when the action is completed</param>
        public AsyncDelegatingCommand(Action<T> action, Func<T,bool> canExecute, Action<object> completedCallback)
            : this(action, canExecute, completedCallback, null, true)
        {
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        /// <param name="completedCallback">Will be invoked when the action is completed</param>
        /// <param name="errorCallback">Will be invoked if the action throws an error</param>
        public AsyncDelegatingCommand(Action<T> action, Func<T,bool> canExecute, Action<object> completedCallback, Action<Exception> errorCallback)
            : this(action, canExecute, completedCallback, errorCallback, true)
        {
        }

        /// <summary>
        /// Constructor for no-parameter ICommand
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        /// <param name="completedCallback">Will be invoked when the action is completed</param>
        /// <param name="errorCallback">Will be invoked if the action throws an error</param>
        /// <param name="autoCanExecuteRequery">True to use WPF CommandManager for CanExecute re-query operations</param>
        public AsyncDelegatingCommand(Action<T> action, Func<T,bool> canExecute, Action<object> completedCallback, Action<Exception> errorCallback, bool autoCanExecuteRequery)
        {
            _worker.DoWork += (s, e) =>
            {
                if (autoCanExecuteRequery)
                    CommandManager.InvalidateRequerySuggested();
                else
                    RaiseCanExecuteChanged();

                action((T)e.Argument);
            };

            _worker.RunWorkerCompleted += (s, e) =>
            {
                if (completedCallback != null && e.Error == null)
                    completedCallback(e.Result);
                if (errorCallback != null && e.Error != null)
                    errorCallback(e.Error);
                
                if (autoCanExecuteRequery)
                    CommandManager.InvalidateRequerySuggested();
                else
                    RaiseCanExecuteChanged();
            };

            if (canExecute != null)
            {
                _canExecute = canExecute;
                _autoCanExecuteRequery = autoCanExecuteRequery;
            }
        }

        /// <summary>
        /// Enable or Disable the automatic CanExecute re-query support using the 
        /// WPF CommandManager.
        /// </summary>
        public bool AutoCanExecuteRequery
        {
            get { return _autoCanExecuteRequery; }
            set
            {
                if (_autoCanExecuteRequery != value)
                {
                    _autoCanExecuteRequery = value;
                    EventHandler eCanExecuteChanged = _internalCanExecuteChanged;
                    if (eCanExecuteChanged != null)
                    {
                        if (_autoCanExecuteRequery)
                        {
                            foreach (EventHandler handler in eCanExecuteChanged.GetInvocationList())
                            {
                                CommandManager.RequerySuggested += handler;
                            }
                        }
                        else
                        {
                            foreach (EventHandler handler in eCanExecuteChanged.GetInvocationList())
                            {
                                CommandManager.RequerySuggested -= handler;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method can be used to raise the CanExecuteChanged handler.
        /// This will force WPF to re-query the status of this command directly.
        /// This is not necessary if you use the AutoCanExecuteRequery feature.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        /// <summary>
        /// This method is used to walk the delegate chain and well WPF that
        /// our command execution status has changed.
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            EventHandler eCanExecuteChanged = _internalCanExecuteChanged;
            if (eCanExecuteChanged != null)
                eCanExecuteChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event that is raised when the current state for our command has changed.
        /// Note that this is an instance event - unlike the CommandManager.RequerySuggested event
        /// and as such we don't need to manage weak references here.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                _internalCanExecuteChanged += value;
                if (_autoCanExecuteRequery)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                _internalCanExecuteChanged -= value;
                if (_autoCanExecuteRequery)
                {
                    CommandManager.RequerySuggested -= value;
                }
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
            return (_canExecute == null) ? !_worker.IsBusy : !_worker.IsBusy && _canExecute((T)parameter);
        }

        /// <summary>
        /// Invoke the command
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            _worker.RunWorkerAsync((T)parameter);
        }
    }
}