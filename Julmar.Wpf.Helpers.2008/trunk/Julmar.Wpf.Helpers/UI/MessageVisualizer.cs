using System.Windows;
using JulMar.Windows.Interfaces;

namespace JulMar.Windows.UI
{
    /// <summary>
    /// This class implements the IMessageVisualizer for WPF
    /// </summary>
    public class MessageVisualizer : IMessageVisualizer
    {
        /// <summary>
        /// This displays a message to the user and prompts for a selection.
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="buttons">Buttons to dismiss box</param>
        /// <returns>Result</returns>
        public MessageResult Show(string title, string message, MessageButtons buttons)
        {
            return (MessageResult) MessageBox.Show(message, title, (MessageBoxButton) buttons);
        }
    }
}
