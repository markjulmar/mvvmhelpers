using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace JulMar.Windows.Interfaces
{
    /// <summary>
    /// This interface abstracts the display of MessageBox style notifications
    /// </summary>
    public interface IMessageVisualizer
    {
        /// <summary>
        /// ShowAsync is used for simple notifications with an OK button.
        /// </summary>
        /// <param name="message">The content to display in the notification.</param>
        /// <param name="title">The optional title to be shown</param>
        Task<IUICommand> ShowAsync(string message, string title = "");

        /// <summary>
        /// Display a notification to the user with options.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="visualizerOptions"></param>
        /// <returns></returns>
        Task<IUICommand> ShowAsync(string message, string title, MessageVisualizerOptions visualizerOptions);
    }
}
