using JulMar.Core;
using JulMar.Windows.Interfaces;

namespace TestMvvm.Services
{
    /// <summary>
    /// Replaced MessageVisulizer which dismisses dialog.
    /// </summary>
    [ExportService(typeof(IMessageVisualizer))]
    class InternalMessageVisualizer : IMessageVisualizer
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
            return MessageResult.Yes;
        }

        /// <summary>
        /// This displays a message to the user and prompts for a selection.
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="visualizerOptions">Optional parameters</param>
        /// <returns>Result</returns>
        public MessageResult Show(string title, string message, MessageVisualizerOptions visualizerOptions)
        {
            return MessageResult.Yes;
        }
    }
}
