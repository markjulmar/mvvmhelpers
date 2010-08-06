using System.ComponentModel.Composition;

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
        OK = 0,
        /// <summary>
        /// OK + Cancel buttons
        /// </summary>
        OKCancel = 1,
        /// <summary>
        /// Yes + No + Cancel buttons
        /// </summary>
        YesNoCancel = 3,
        /// <summary>
        /// Yes + No buttons
        /// </summary>
        YesNo = 4,
    }

    /// <summary>
    /// Result of a IMessageVisualizer.Show call
    /// </summary>
    public enum MessageResult
    {
        /// <summary>
        /// No result - dialog closed without button
        /// </summary>
        None = 0,
        /// <summary>
        /// OK button clicked to dismiss dialog
        /// </summary>
        OK = 1,
        /// <summary>
        /// Cancel button clicked to dismiss dialog
        /// </summary>
        Cancel = 2,
        /// <summary>
        /// Yes button clicked to dismiss dialog
        /// </summary>
        Yes = 6,
        /// <summary>
        /// No button clicked to dismiss dialog
        /// </summary>
        No = 7
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
    }
}
