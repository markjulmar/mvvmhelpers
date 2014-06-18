using System.Linq;
using System.Threading.Tasks;

using Android.App;

using JulMar.Core;
using JulMar.Interfaces;
using JulMar.UI;

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
        public Task<IUICommand> ShowAsync(string message, string title, MessageVisualizerOptions visualizerOptions)
        {
            if (string.IsNullOrEmpty(message))
                message = string.Empty;
            if (string.IsNullOrEmpty(title))
                title = string.Empty;

            if (visualizerOptions == null)
            {
                visualizerOptions = new MessageVisualizerOptions(UICommand.Ok);
            }

            var tcs = new TaskCompletionSource<IUICommand>();
            var alert = new AlertDialog.Builder(Application.Context)
                .SetTitle(title)
                .SetMessage(message)
                .SetItems(visualizerOptions.Commands.Select(c => c.Label).ToArray(), (s,e) => tcs.SetResult(visualizerOptions.Commands[e.Which]));
            alert.Create().Show();

            return tcs.Task;
        }
    }
}
