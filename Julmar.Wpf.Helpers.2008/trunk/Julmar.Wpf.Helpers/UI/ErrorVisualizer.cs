using System.Windows;
using JulMar.Windows.Interfaces;
using System;
using JulMar.Windows.Mvvm;

namespace JulMar.Windows.UI
{
    /// <summary>
    /// This implements the IErrorVisualizer for WPF.
    /// </summary>
    [ExportServiceProvider(typeof(IErrorVisualizer))]
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
                MessageBox.Show(message, title, MessageBoxButton.OK);
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