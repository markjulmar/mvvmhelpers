using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JulMar.Core;
using JulMar.Extensions;
using JulMar.Interfaces;
using JulMar.UI;

using MonoTouch.UIKit;

[assembly:ExportService(typeof(IMessageVisualizer), typeof(MessageVisualizer), IsFallback = true)]
    
namespace JulMar.UI
{
    /// <summary>
    /// This class implements the IMessageVisualizer for iOS
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
            if (string.IsNullOrEmpty(message))
                message = string.Empty;
            if (string.IsNullOrEmpty(title))
                title = string.Empty;

            if (visualizerOptions == null)
            {
                visualizerOptions = new MessageVisualizerOptions(UICommand.Ok);
            }

            var alertDialog = new UIAlertView(
                title,
                message,
                null,
                visualizerOptions.Commands[0].Label,
                visualizerOptions.Commands.Skip(1).Select(c => c.Label).ToArray());

            IUICommand selectedCommand = null;
            CancellationTokenSource cts = new CancellationTokenSource();
            alertDialog.Dismissed += (sender, args) =>
                {
                    selectedCommand = visualizerOptions.Commands[args.ButtonIndex];
                    cts.Cancel();
                };

            await Task.Delay(TimeSpan.MaxValue, cts.Token);
            return selectedCommand;
        }
    }
}
