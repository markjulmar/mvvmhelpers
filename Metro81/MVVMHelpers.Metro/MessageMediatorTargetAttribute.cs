using System;
using JulMar.Core.Services;

namespace JulMar.Core
{
    /// <summary>
    /// This attribute allows a method to be targeted as a recipient for a message.
    /// It requires that the Type is registered with the MessageMediator through the
    /// <seealso cref="MessageMediator.Register"/> method
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// [MessageMediatorTarget("DoBackgroundCheck")]
    /// void OnBackgroundCheck(object parameter) { ... }
    /// 
    /// [MessageMediatorTarget(typeof(SomeDataClass))]
    /// void OnNotifyDataRecieved(SomeDataClass parameter) { ... }
    /// ...
    /// 
    /// mediator.SendMessage("DoBackgroundCheck", new CheckParameters());
    /// ...
    /// mediator.SendMessage(new SomeDataClass(...));
    /// 
    /// ]]>
    /// </example>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class MessageMediatorTargetAttribute : Attribute
    {
        /// <summary>
        /// Message key
        /// </summary>
        public object MessageKey { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MessageMediatorTargetAttribute()
        {
            MessageKey = null;
        }

        /// <summary>
        /// Constructor that takes a message key
        /// </summary>
        /// <param name="messageKey">Message Key</param>
        public MessageMediatorTargetAttribute(string messageKey)
        {
            MessageKey = messageKey;
        }
    }
}