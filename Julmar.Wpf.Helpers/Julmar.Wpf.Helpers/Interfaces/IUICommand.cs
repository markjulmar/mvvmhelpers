using System;

namespace JulMar.Windows.Interfaces
{
    /// <summary>
    /// A specific button or command to show in the prompt. This has been shaped to 
    /// match the Windows 8 Metro IUICommand interface for porting purposes.
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