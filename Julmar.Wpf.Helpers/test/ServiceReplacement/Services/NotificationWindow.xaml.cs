using System;
using System.Windows;
using System.Windows.Input;
using JulMar.Core;
using JulMar.Windows.Interfaces;

namespace ServiceReplacement.Services
{
    /// <summary>
    /// Interaction logic for NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow
    {
        public NotificationWindow()
        {
            InitializeComponent();
        }
    }

    [ExportService(typeof(INotificationVisualizer))]
    class NotificationVisualizer : INotificationVisualizer
    {
        private static bool _showingNotification;

        internal class DummyWaiter : IDisposable
        {
            public void Dispose()
            {
            }
        }
        internal class CloseDialogWaiter : IDisposable
        {
            internal Window Win { get; set; }
            internal IInputElement FocusedElement { get; set; }

            public CloseDialogWaiter()
            {
                _showingNotification = true;
                Application.Current.MainWindow.IsEnabled = false;
            }

            public void Dispose()
            {
                _showingNotification = false;

                Win.Close();
                Application.Current.MainWindow.IsEnabled = true;
                FocusManager.SetFocusedElement(Application.Current.MainWindow, FocusedElement);
                Application.Current.MainWindow.Activate();
                FocusedElement.Focus();
            }
        }

        /// <summary>
        /// This provides a "Wait" support
        /// </summary>
        /// <param name="title">Title (if any)</param><param name="message">Message (if any)</param>
        /// <returns>
        /// Disposable element
        /// </returns>
        public IDisposable BeginWait(string title, string message)
        {
            // Do not allow reentry.
            if (_showingNotification)
                return new DummyWaiter();

            NotificationWindow win = new NotificationWindow();
            if (!string.IsNullOrEmpty(title))
                win.TitleText.Text = title;
            win.MessageText.Text = message;
            win.Owner = Application.Current.MainWindow;
            win.Show();

            return new CloseDialogWaiter
            {
                FocusedElement = FocusManager.GetFocusedElement(Application.Current.MainWindow),
                Win = win
            };
        }
    }
}
