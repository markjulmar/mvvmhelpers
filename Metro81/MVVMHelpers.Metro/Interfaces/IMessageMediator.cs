using System;

namespace JulMar.Core.Interfaces
{
    ///<summary>
    /// The interface definition for our Message mediator.  This allows loose-event coupling between components
    /// in an application by sending messages to registered elements.
    ///</summary>
    public interface IMessageMediator
    {
        /// <summary>
        /// This registers a Type with the mediator.  Any methods decorated with <seealso cref="MessageMediatorTargetAttribute"/> will be 
        /// registered as target method handlers for the given message key.
        /// </summary>
        /// <param name="view">Object to register</param>
        void Register(object view);

        /// <summary>
        /// This method unregisters a type from the message mediator.
        /// </summary>
        /// <param name="view">Object to unregister</param>
        void Unregister(object view);

        /// <summary>
        /// This registers a specific method as a message handler for a specific type.
        /// </summary>
        /// <param name="key">Message key</param>
        /// <param name="handler">Handler method</param>
        void RegisterHandler<T>(string key, Action<T> handler);

        /// <summary>
        /// This registers a specific method as a message handler for a specific type.
        /// </summary>
        /// <param name="handler">Handler method</param>
        void RegisterHandler<T>(Action<T> handler);

        /// <summary>
        /// This unregisters a method as a handler.
        /// </summary>
        /// <param name="key">Message key</param>
        /// <param name="handler">Handler</param>
        void UnregisterHandler<T>(string key, Action<T> handler);

        /// <summary>
        /// This unregisters a method as a handler for a specific type
        /// </summary>
        /// <param name="handler">Handler</param>
        void UnregisterHandler<T>(Action<T> handler);

        /// <summary>
        /// This method broadcasts a message to all message targets for a given
        /// message key and passes a parameter.
        /// </summary>
        /// <param name="key">Message key</param>
        /// <param name="message">Message parameter</param>
        /// <returns>True/False if any handlers were invoked.</returns>
        bool SendMessage<T>(string key, T message);

        /// <summary>
        /// This method broadcasts a message to all message targets for a given parameter type.
        /// If a derived type is passed, any handlers for interfaces or base types will also be
        /// invoked.
        /// </summary>
        /// <param name="message">Message parameter</param>
        /// <returns>True/False if any handlers were invoked.</returns>
        bool SendMessage<T>(T message);

        /// <summary>
        /// This method broadcasts a message to all message targets for a given
        /// message key and passes a parameter.  The message targets are all called
        /// asynchronously and any resulting exceptions are ignored.
        /// </summary>
        /// <param name="key">Message key</param>
        /// <param name="message">Message parameter</param>
        void SendMessageAsync<T>(string key, T message);

        /// <summary>
        /// This method broadcasts a message to all message targets for a given parameter type.
        /// If a derived type is passed, any handlers for interfaces or base types will also be
        /// invoked.  The message targets are all called asynchronously and any resulting exceptions
        /// are ignored.
        /// </summary>
        /// <param name="message">Message parameter</param>
        void SendMessageAsync<T>(T message);
    }
}