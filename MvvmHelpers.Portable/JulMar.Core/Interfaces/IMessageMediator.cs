using System;

namespace JulMar.Services
{
    /// <summary>
    /// Message Mediator interface.
    /// </summary>
    public interface IMessageMediator
    {
        /// <summary>
        /// Registers a specific method with no parameters as a handler
        /// </summary>
        /// <param name="key">Message key</param>
        /// <param name="handler">Handler method</param>
        IDisposable Subscribe(string key, Action handler);

        /// <summary>
        /// This registers a specific method as a message handler for a specific type.
        /// </summary>
        /// <param name="key">Message key</param>
        /// <param name="handler">Handler method</param>
        IDisposable Subscribe<T>(string key, Action<T> handler);

        /// <summary>
        /// This registers a specific method as a message handler for a specific type.
        /// </summary>
        /// <param name="handler">Handler method</param>
        IDisposable Subscribe<T>(Action<T> handler);

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