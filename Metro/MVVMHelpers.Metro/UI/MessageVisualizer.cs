using System;
using System.Threading.Tasks;
using JulMar.Core.Internal;
using JulMar.Windows.Interfaces;
using Windows.UI.Popups;

namespace JulMar.Windows.UI
{
    /// <summary>
    /// This class implements the IMessageVisualizer for Metro
    /// </summary>
    [DefaultExport(typeof(IMessageVisualizer))]
    public sealed class MessageVisualizer : IMessageVisualizer
    {
        /// <summary>
        /// ShowAsync is used for simple notifications with an OK button.
        /// </summary>
        /// <param name="message">The content to display in the notification.</param>
        /// <param name="title">The optional title to be shown</param>
        public Task<IUICommand> ShowAsync(string message, string title = "")
        {
            return ShowAsync(message, title, null);
        }

        /// <summary>
        /// Display a notification to the user with options.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="visualizerOptions"></param>
        /// <returns></returns>
        public Task<IUICommand> ShowAsync(string message, string title, MessageVisualizerOptions visualizerOptions)
        {
            if (visualizerOptions == null)
            {
                visualizerOptions = new MessageVisualizerOptions(UICommand.Ok);
            }

            MessageDialog messageDialog = new MessageDialog(message, title)
            {
                DefaultCommandIndex = (uint) visualizerOptions.DefaultCommandIndex,
                CancelCommandIndex = (uint) visualizerOptions.CancelCommandIndex,
            };

            foreach (var command in visualizerOptions.Commands)
            {
                messageDialog.Commands.Add(command);
            }

            return messageDialog.ShowAsync().AsTask();
        }
    }
}
