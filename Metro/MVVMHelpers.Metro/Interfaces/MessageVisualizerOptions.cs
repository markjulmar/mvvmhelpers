using System.Collections.Generic;
using Windows.UI.Popups;

namespace JulMar.Windows.Interfaces
{
    /// <summary>
    /// Options passed to IMessageVisualizer.  Can create derived class to pass
    /// custom data into private implementation.
    /// </summary>
    public class MessageVisualizerOptions
    {
        /// <summary>
        /// Button handlers
        /// </summary>
        public IList<IUICommand> Commands { get; private set; }

        /// <summary>
        /// The default index used to dismiss the dialog
        /// </summary>
        public int DefaultCommandIndex { get; set; }

        /// <summary>
        /// The cancel index used to dismiss the dialog
        /// </summary>
        public int CancelCommandIndex { get; set; }

        /// <summary>
        /// Constructor for single-button messages
        /// </summary>
        public MessageVisualizerOptions(IUICommand command)
        {
            DefaultCommandIndex = 0;
            CancelCommandIndex = 1;
            Commands = new List<IUICommand> {command};
        }

        /// <summary>
        /// Constructor to create simple prompt
        /// </summary>
        public MessageVisualizerOptions(IEnumerable<IUICommand> commands = null)
        {
            DefaultCommandIndex = 0;
            CancelCommandIndex = 1;
            Commands = commands != null ? new List<IUICommand>(commands) : new List<IUICommand>();
        }
    }
}