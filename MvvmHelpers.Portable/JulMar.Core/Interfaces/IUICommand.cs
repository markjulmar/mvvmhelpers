using System;

namespace JulMar.Interfaces
{
    /// <summary>
    /// A specific button or command to show in the prompt.
    /// </summary>
    public interface IUICommand
    {
        /// <summary>
        /// Id representing this command
        /// </summary>
        object Id { get; set; }
        /// <summary>
        /// The handler
        /// </summary>
        Action Invoked { get; set; }
        /// <summary>
        /// The text to display
        /// </summary>
        string Label { get; set; }
    }
}