using System;
using System.Windows;

using JulMar.Core;
using JulMar.Interfaces;

[assembly:ExportService(typeof(IErrorVisualizer), typeof(IErrorVisualizer), IsFallback = true)]

namespace JulMar.UI
{
    /// <summary>
    /// This implements the IErrorVisualizer for WPF.
    /// </summary>
    sealed class ErrorVisualizer : IErrorVisualizer
    {
        /// <summary>
        /// This displays an error to the user
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message text</param>
        public bool Show(string title, string message)
        {
            try
            {
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return true;
            }
            catch (InvalidOperationException)
            {
                // Catch re-entrancy warning.  This happens if you issue an error dialog
                // while initializing some other modal dialog (ShowDialog) on this thread.
                return false;
            }
        }
    }
}