using System;

using JulMar.Core;
using JulMar.Interfaces;
using JulMar.UI;

[assembly:ExportService(typeof(INotificationVisualizer), typeof(NotificationVisualizer), IsFallback = true)]

namespace JulMar.UI
{
    /// <summary>
    /// This implements the INotificationVisualizer interface for WPF
    /// </summary>
    sealed class NotificationVisualizer : INotificationVisualizer
    {
        /// <summary>
        /// This provides a "Wait" support
        /// </summary>
        /// <param name="title">Title (if any)</param>
        /// <param name="message">Message (if any)</param>
        /// <returns>Disposable element</returns>
        public IDisposable BeginWait(string title, string message)
        {
            return new WaitCursor();
        }
    }
}