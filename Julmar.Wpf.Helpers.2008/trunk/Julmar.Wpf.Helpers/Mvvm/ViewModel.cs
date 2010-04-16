using System;
using JulMar.Windows.Interfaces;
using System.ComponentModel.Composition;

namespace JulMar.Windows.Mvvm
{
    /// <summary>
    /// This class is used as the basis for all ViewModel objects
    /// </summary>
    public class ViewModel : SimpleViewModel, IDisposable
    {
        /// <summary>
        /// Service resolver for view models.  Allows derived types to add/remove
        /// services from mapping.
        /// </summary>
        public static readonly ServiceProvider ServiceProvider = new ServiceProvider();

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
        public ViewModel()
        {
            // Register with the message mediator - this will locate and bind all message
            // targets on this instance. Shouldn't really need to check for existence - it should
            // always be present, but just in case someone Unregisters it..
            var mediator = Resolve<IMessageMediator>();
            if (mediator != null)
            {
                mediator.Register(this);
            }

            // Hook up any MEF imports/exports
            IDynamicResolver loader = Resolve<IDynamicResolver>();
            if (loader != null)
            {
                try
                {
                    loader.Compose(this);
                }
                catch (CompositionException)
                {
                    // Can throw if in invalid state - i.e. creating VM on behalf of a view.
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