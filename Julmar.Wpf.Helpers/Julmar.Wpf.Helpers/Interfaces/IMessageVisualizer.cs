using System.ComponentModel.Composition;
using System.Windows;

namespace JulMar.Windows.Interfaces
{
    /// <summary>
    /// Button types that may be used to dismiss message prompts
    /// </summary>
    public enum MessageButtons
    {
        /// <summary>
        /// OK button
        /// </summary>
        OK,
        /// <summary>
        /// OK + Cancel buttons
        /// </summary>
        OKCancel,
        /// <summary>
        /// Yes + No + Cancel buttons
        /// </summary>
        YesNoCancel,
        /// <summary>
        /// Yes + No buttons
        /// </summary>
        YesNo,
    }

    /// <summary>
    /// Result of a IMessageVisualizer.Show call
    /// </summary>
    public enum MessageResult
    {
        /// <summary>
        /// No result - dialog closed without button
        /// </summary>
        None,
        /// <summary>
        /// OK button clicked to dismiss dialog
        /// </summary>
        OK,
        /// <summary>
        /// Cancel button clicked to dismiss dialog
        /// </summary>
        Cancel,
        /// <summary>
        /// Yes button clicked to dismiss dialog
        /// </summary>
        Yes,
        /// <summary>
        /// No button clicked to dismiss dialog
        /// </summary>
        No
    }

    /// <summary>
    /// Icon to use
    /// </summary>
    public enum MessageIcon
    {
        /// <summary>
        /// No icon
        /// </summary>
        None,
        /// <summary>
        /// Error icon
        /// </summary>
        Error,
        /// <summary>
        /// Question icon
        /// </summary>
        Question,
        /// <summary>
        /// Warning icon
        /// </summary>
        Warning,
        /// <summary>
        /// Information icon
        /// </summary>
        Information,
    }

    /// <summary>
    /// Options passed to IMessageVisualizer.  Can create derived class to pass
    /// custom data into private implementation.
    /// </summary>
    public class MessageVisualizerOptions
    {
        /// <summary>
        /// Window Owner
        /// </summary>
        public Window Owner { get; set; }

        /// <summary>
        /// Icon to use
        /// </summary>
        public MessageIcon Icon { get; set; }

        /// <summary>
        /// Prompt to use
        /// </summary>
        public MessageButtons Prompt { get; set; }

        /// <summary>
        /// Default choice for dismissal
        /// </summary>
        public MessageResult DefaultPrompt { get; set; }

        /// <summary>
        /// Constructor to create simple prompt
        /// </summary>
        public MessageVisualizerOptions()
        {
            Owner = null;
            Icon = MessageIcon.None;
            Prompt = MessageButtons.OK;
            DefaultPrompt = MessageResult.OK;
        }
    }

    /// <summary>
    /// This interface abstracts the display of MessageBox style notifications
    /// </summary>
    [InheritedExport]
    public interface IMessageVisualizer
    {
        /// <summary>
        /// This displays a message to the user and prompts for a selection.
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="buttons">Buttons to dismiss box</param>
        /// <returns>Result</returns>
        MessageResult Show(string title, string message, MessageButtons buttons);

        /// <summary>
        /// This displays a message to the user and prompts for a selection.
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="visualizerOptions">Optional parameters</param>
        /// <returns>Result</returns>
        MessageResult Show(string title, string message, MessageVisualizerOptions visualizerOptions);
    }
}
