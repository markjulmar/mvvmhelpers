using System;
using JulMar.Core.Interfaces;
using JulMar.Core.Services;
using System.Runtime.Serialization;

namespace JulMar.Windows.Mvvm
{
    /// <summary>
    /// This class is used as the basis for all ViewModel objects
    /// </summary>
    [DataContract]
    public class ViewModel : SimpleViewModel, IDisposable
    {
        /// <summary>
        /// Constructor - registers with the message mediator and hooks up any imports/exports
        /// with the default MEF catalog
        /// </summary>
        protected ViewModel()
        {
            Initialize(new StreamingContext());
        }

        /// <summary>
        /// Setup the view model
        /// </summary>
        [OnDeserialized]
        private void Initialize(StreamingContext context)
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
            try
            {
                DynamicComposer.Instance.Compose(this);
            }
            catch
            {
                // Can throw if in invalid state - i.e. creating VM on behalf of a view.
                // .. or if we are already composing the parts and this was created as a result.
            }
        }

        /// <summary>
        /// This resolves a service type and returns the implementation.
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>Implementation</returns>
        protected T Resolve<T>()
        {
            return ServiceLocator.Instance.Resolve<T>();
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