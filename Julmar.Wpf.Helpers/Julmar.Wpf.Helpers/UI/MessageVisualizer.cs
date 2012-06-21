using System.Windows;
using JulMar.Core;
using JulMar.Windows.Interfaces;
using System.Windows.Controls;

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
        /// <returns>Result</returns>
        public void Show(string title, string message)
        {
            Show(title, message, null);
        }

        /// <summary>
        /// This displays a message to the user and prompts for a selection.
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="visualizerOptions">Options for the message</param>
        /// <returns>Result</returns>
        public IUICommand Show(string title, string message, MessageVisualizerOptions visualizerOptions)
        {
            if (visualizerOptions == null)
            {
                visualizerOptions = new MessageVisualizerOptions(UICommand.Ok);
            }

            Window popup = new Window
                {
                    Owner = visualizerOptions.Owner,
                    WindowStartupLocation = (visualizerOptions.Owner == null) 
                                ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    Background = SystemColors.ControlLightBrush,
                    Foreground = SystemColors.ControlTextBrush,
                    Title = title,
                    MinWidth = 200, MinHeight = 100,
                };

            StackPanel rootPanel = new StackPanel();
            TextBlock messageText = new TextBlock
                {
                    Text = message, 
                    Margin = new Thickness(20),
                    TextWrapping = TextWrapping.Wrap,
                    MaxWidth = SystemParameters.PrimaryScreenWidth / 2,
                    MaxHeight = SystemParameters.FullPrimaryScreenWidth / 2,
                    HorizontalAlignment = HorizontalAlignment.Center, 
                    VerticalAlignment = VerticalAlignment.Center
                };
            rootPanel.Children.Add(messageText);

            var commands = visualizerOptions.Commands;
            if (commands.Count == 0)
                commands = new[] { UICommand.Ok };

            IUICommand finalCommand = null;
            WrapPanel buttonPanel = new WrapPanel() {Margin = new Thickness(10), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center};
            for (int index = 0; index < commands.Count; index++)
            {
                var oneCommand = commands[index];
                IUICommand command = oneCommand;
                Button commandButton = new Button
                {
                    Content = command.Label,
                    Tag = command,
                    MinWidth = 75,
                    Margin = new Thickness(5),
                    Padding = new Thickness(10, 5, 10, 5),
                    IsDefault = index == visualizerOptions.DefaultCommandIndex,
                    IsCancel = index == visualizerOptions.CancelCommandIndex,
                };
                commandButton.Click += (s, e) =>
                {
                    if (command.Invoked != null)
                        command.Invoked();
                    finalCommand = ((Button) s).Tag as IUICommand;
                    popup.DialogResult = !((Button)s).IsCancel;
                };
                buttonPanel.Children.Add(commandButton);
            }

            rootPanel.Children.Add(buttonPanel);

            popup.Content = rootPanel;
            popup.ShowDialog();

            return finalCommand;
        }
    }
}
