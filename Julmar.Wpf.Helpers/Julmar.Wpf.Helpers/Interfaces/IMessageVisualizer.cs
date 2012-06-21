using System.ComponentModel.Composition;

namespace JulMar.Windows.Interfaces
{
    /// <summary>
    /// This interface abstracts the display of MessageBox style notifications
    /// </summary>
    [InheritedExport]
    public interface IMessageVisualizer
    {
        /// <summary>
        /// This displays a message to the user with an OK button selection.
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        void Show(string title, string message);

        /// <summary>
        /// This displays a message to the user and prompts for a selection.
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="visualizerOptions">Optional parameters</param>
        /// <returns>Command which dismissed dialog</returns>
        IUICommand Show(string title, string message, MessageVisualizerOptions visualizerOptions);
    }
}
