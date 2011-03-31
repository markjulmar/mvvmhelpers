using System;
using System.Windows.Threading;
using JulMar.Core.Interfaces;
using JulMar.Core.Services;
using System.Windows;
using JulMar.Windows.Extensions;

namespace JulMar.Windows.Mvvm
{
    /// <summary>
    /// This class is used as the basis for all ViewModel objects
    /// </summary>
    public class ViewModel : SimpleViewModel, IDisposable
    {
        /// <summary>
        /// Service provider used by ViewModels.
        /// </summary>
        public static IServiceProviderEx ServiceProvider;

        /// <summary>
        /// Static constructor - executed prior to any ViewModel being used.
        /// </summary>
        static ViewModel()
        {
            ServiceProvider = MefComposer.Instance.GetExportedValue<IServiceProviderEx>();
            if (ServiceProvider == null)
                throw new InvalidOperationException("Unable to locate Service Locator Service (IServiceProviderEx");
        }

        /// <summary>
        /// This event should be raised to close the view.  Any view tied to this
        /// ViewModel should register a handler on this event and close itself when
        /// this event is raised.  If the view is not bound to the lifetime of the
        /// ViewModel then this event can be ignored.
        /// </summary>
        public event EventHandler<CloseRequestEventArgs> CloseRequest;

        /// <summary>
        /// This event should be raised to activate the UI.  Any view tied to this
        /// ViewModel should register a handler on this event and close itself when
        /// this event is raised.  If the view is not bound to the lifetime of the
        /// ViewModel then this event can be ignored.
        /// </summary>
        public event EventHandler ActivateRequest;

        /// <summary>
        /// Constructor - registers with the message mediator and hooks up any imports/exports
        /// with the default MEF catalog
        /// </summary>
        protected ViewModel() : this(true, true)
        {
        }

        /// <summary>
        /// Constructor - registers with the message mediator and hooks up any imports/exports
        /// with the default MEF catalog
        /// </summary>
        protected ViewModel(bool registerWithMediator, bool composeImports)
        {
            // Register with the message mediator - this will locate and bind all message
            // targets on this instance. Shouldn't really need to check for existence - it should
            // always be present, but just in case someone Unregisters it..
            if (registerWithMediator)
            {
                var mediator = Resolve<IMessageMediator>();
                if (mediator != null)
                {
                    mediator.Register(this);
                }
            }

            // Hook up any MEF imports/exports
            if (composeImports)
            {
                try
                {
                    MefComposer.Instance.ComposeOnce(this);
                }
                catch
                {
                    // Can throw if in invalid state - i.e. creating VM on behalf of a view.
                    // .. or if we are already composing the parts and this was created as a result.
                }
            }
        }

        /// <summary>
        /// This resolves a service type and returns the implementation.
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>Implementation</returns>
        protected T Resolve<T>()
        {
            return ServiceProvider.Resolve<T>();
        }

        /// <summary>
        /// This raises the CloseRequest event to close the UI.
        /// </summary>
        public virtual void RaiseCloseRequest()
        {
            RaiseCloseRequest(null);
        }

        /// <summary>
        /// This raises the CloseRequest event to close the UI.
        /// </summary>
        public virtual void RaiseCloseRequest(bool? dialogResult)
        {
            var closeRequest = CloseRequest;
            if (closeRequest != null)
                closeRequest(this, new CloseRequestEventArgs(dialogResult));
        }

        /// <summary>
        /// This raises the ActivateRequest event to activate the UI.
        /// </summary>
        public virtual void RaiseActivateRequest()
        {
            var activateRequest = ActivateRequest;
            if (activateRequest != null)
                activateRequest(this, EventArgs.Empty);
        }

        /// <summary>
        /// This sends a message via the MessageMediator.
        /// </summary>
        /// <param name="key">Message key</param>
        /// <param name="message">Message parameter</param>
        /// <returns>True if at least one recipient received the message</returns>
        protected bool SendMessage<T>(string key, T message)
        {
            var mediator = Resolve<IMessageMediator>();
            return mediator != null && mediator.SendMessage(key, message);
        }

        /// <summary>
        /// This sends a message via the MessageMediator.
        /// </summary>
        /// <param name="message">Message parameter</param>
        /// <returns>True if at least one recipient received the message</returns>
        protected bool SendMessage<T>(T message)
        {
            var mediator = Resolve<IMessageMediator>();
            return mediator != null && mediator.SendMessage(message);
        }

        /// <summary>
        /// Helper method to run logic on Dispatcher (UI) thread. Useful for adding to 
        /// ObservableCollection(T), etc.
        /// </summary>
        /// <param name="action">Method to execute</param>
        /// <returns></returns>
        protected void DispatcherInvoke(Action action)
        {
            DispatcherInvoke(DispatcherPriority.Normal, action);
        }

        /// <summary>
        /// Helper method to run logic on Dispatcher (UI) thread. Useful for adding to 
        /// ObservableCollection(T), etc.
        /// </summary>
        /// <param name="priority">Priority of method</param>
        /// <param name="action">Method to execute</param>
        /// <returns>Result</returns>
        protected void DispatcherInvoke(DispatcherPriority priority, Action action)
        {
            // No application? Just run it.
            if (Application.Current != null)
                Application.Current.Dispatcher.Invoke(priority, action);
            else 
                action();
        }

        /// <summary>
        /// Helper method to run logic on Dispatcher (UI) thread. Useful for adding to 
        /// ObservableCollection(T), etc.
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="func">Method to execute</param>
        /// <returns>Result</returns>
        protected T DispatcherInvoke<T>(Func<T> func)
        {
            return DispatcherInvoke(DispatcherPriority.Normal, func);
        }

        /// <summary>
        /// Helper method to run logic on Dispatcher (UI) thread. Useful for adding to 
        /// ObservableCollection(T), etc.
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="priority">Priority</param>
        /// <param name="func">Method to execute</param>
        /// <returns>Result</returns>
        protected T DispatcherInvoke<T>(DispatcherPriority priority, Func<T> func)
        {
            return Application.Current != null ? (T) Application.Current.Dispatcher.Invoke(priority, func) : func();
        }

        /// <summary>
        /// Helper method to run logic on Dispatcher (UI) thread. Useful for adding to 
        /// ObservableCollection(T), etc.
        /// </summary>
        /// <param name="action">Method to execute</param>
        protected void DispatcherBeginInvoke(Action action)
        {
            DispatcherBeginInvoke(DispatcherPriority.Normal, action);
        }

        /// <summary>
        /// Helper method to run logic on Dispatcher (UI) thread. Useful for adding to 
        /// ObservableCollection(T), etc.
        /// </summary>
        /// <param name="priority">Priority</param>
        /// <param name="action">Method to execute</param>
        protected void DispatcherBeginInvoke(DispatcherPriority priority, Action action)
        {
            // If no application then just run it.
            if (Application.Current != null)
            {
                if (Application.Current.Dispatcher.CheckAccess() &&
                    priority == DispatcherPriority.Normal)
                    action();
                else
                    Application.Current.Dispatcher.BeginInvoke(priority, action);
            }
            else
                action();
        }

        /// <summary>
        /// Returns true if we are currently in design mode.
        /// Useful for design-time property return tests.
        /// </summary>
        protected bool InDesignMode
        {
            get { return Designer.InDesignMode; }
        }

        #region IDisposable Members

        /// <summary>
        /// This disposes of the view model.  It unregisters from the message mediator.
        /// </summary>
        /// <param name="isDisposing">True if IDisposable.Dispose was called</param>
        protected virtual void Dispose(bool isDisposing)
        {
            var mediator = Resolve<IMessageMediator>();
            if (mediator != null)
                mediator.Unregister(this);
        }

        /// <summary>
        /// Implementation of IDisposable.Dispose.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}