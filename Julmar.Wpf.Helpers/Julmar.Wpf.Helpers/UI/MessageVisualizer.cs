using System.Windows;
using JulMar.Core;
using JulMar.Windows.Interfaces;

namespace JulMar.Windows.UI
{
    /// <summary>
    /// This class implements the IMessageVisualizer for WPF
    /// </summary>
    [ExportService(typeof(IMessageVisualizer))]
    sealed class MessageVisualizer : IMessageVisualizer
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
            return Convert(MessageBox.Show(message, title, Convert(buttons)));
        }

        /// <summary>
        /// This displays a message to the user and prompts for a selection.
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="visualizerOptions">Options for the message</param>
        /// <returns>Result</returns>
        public MessageResult Show(string title, string message, MessageVisualizerOptions visualizerOptions)
        {
            return (visualizerOptions.Owner == null)
                ? Convert(MessageBox.Show(message, title, Convert(visualizerOptions.Prompt), Convert(visualizerOptions.Icon), Convert(visualizerOptions.DefaultPrompt)))
                : Convert(MessageBox.Show(visualizerOptions.Owner, message, title, Convert(visualizerOptions.Prompt), Convert(visualizerOptions.Icon), Convert(visualizerOptions.DefaultPrompt)));
        }

        private static MessageBoxImage Convert(MessageIcon icon)
        {
            switch (icon)
            {
                case MessageIcon.Information:
                    return MessageBoxImage.Information;
                case MessageIcon.Question:
                    return MessageBoxImage.Question;
                case MessageIcon.Warning:
                    return MessageBoxImage.Warning;
                case MessageIcon.Error:
                    return MessageBoxImage.Error;
            }
            return MessageBoxImage.None;
        }

        private static MessageBoxButton Convert(MessageButtons button)
        {
            switch (button)
            {
                case MessageButtons.OKCancel:
                    return MessageBoxButton.OKCancel;
                case MessageButtons.YesNoCancel:
                    return MessageBoxButton.YesNoCancel;
                case MessageButtons.YesNo:
                    return MessageBoxButton.YesNo;
                default:
                    break;
            }

            return MessageBoxButton.OK;
        }

        private static MessageBoxResult Convert(MessageResult result)
        {
            switch (result)
            {
                case MessageResult.OK:
                    return MessageBoxResult.OK;
                case MessageResult.Cancel:
                    return MessageBoxResult.Cancel;
                case MessageResult.Yes:
                    return MessageBoxResult.Yes;
                case MessageResult.No:
                    return MessageBoxResult.No;
                default:
                    break;
            }
            return MessageBoxResult.None;
        }

        private static MessageResult Convert(MessageBoxResult result)
        {
            switch (result)
            {
                case MessageBoxResult.OK:
                    return MessageResult.OK;
                case MessageBoxResult.Cancel:
                    return MessageResult.Cancel;
                case MessageBoxResult.Yes:
                    return MessageResult.Yes;
                case MessageBoxResult.No:
                    return MessageResult.No;
                default:
                    break;
            }
            return MessageResult.None;
        }
    }
}
