namespace JulMar.Interfaces
{
    /// <summary>
    /// This interface is used to display an error.
    /// </summary>
    public interface IErrorVisualizer
    {
        /// <summary>
        /// This displays an error to the user
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message text</param>
        bool Show(string title, string message);
    }
}