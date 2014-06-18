using System;
using System.Threading.Tasks;
using JulMar.Core;
using JulMar.Extensions;
using JulMar.Interfaces;
using JulMar.UI;

[assembly:ExportService(typeof(IMessageVisualizer), typeof(MessageVisualizer), IsFallback = true)]
    
namespace JulMar.UI
{
    /// <summary>
    /// This class implements the IMessageVisualizer for Metro
    /// </summary>
    sealed class MessageVisualizer : IMessageVisualizer
    {
        /// <summary>
        /// ShowAsync is used for simple notifications with an OK button.
        /// </summary>
        /// <param name="message">The content to display in the notification.</param>
        /// <param name="title">The optional title to be shown</param>
        public Task<IUICommand> ShowAsync(string message, string title = "")
        {
            return this.ShowAsync(message, title, null);
        }

        /// <summary>
        /// Display a notification to the user with options.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="visualizerOptions"></param>
        /// <returns></returns>
        public async Task<IUICommand> ShowAsync(string message, string title, MessageVisualizerOptions visualizerOptions)
        {
            // Cannot have nulls - throws exception.
            if (string.IsNullOrEmpty(message))
                message = string.Empty;
            if (string.IsNullOrEmpty(title))
                title = string.Empty;

            if (visualizerOptions == null)
            {
                visualizerOptions = new MessageVisualizerOptions(UICommand.Ok);
            }

            var messageDialog = new global::Windows.UI.Popups.MessageDialog(message, title)
            {
                DefaultCommandIndex = (uint) visualizerOptions.DefaultCommandIndex,
                CancelCommandIndex = (uint) visualizerOptions.CancelCommandIndex,
            };

            foreach (var command in visualizerOptions.Commands)
            {
                messageDialog.Commands.Add(command.ToWindowsCommand());
            }

            // Pull the original command back out.
            var result = await messageDialog.ShowAsync();
            return ((UICommandExtensions.WindowsCommand)result).Command;
        }
    }
}
