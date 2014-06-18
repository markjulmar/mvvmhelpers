using System;

namespace JulMar.Interfaces
{
    /// <summary>
    /// This is used to display a notification that the program is doing something
    /// </summary>
    public interface INotificationVisualizer
    {
        /// <summary>
        /// This provides a "Wait" support
        /// </summary>
        /// <param name="title">Title (if any)</param>
        /// <param name="message">Message (if any)</param>
        /// <returns>Disposable element</returns>
        IDisposable BeginWait(string title, string message);
    }
}