using System.Collections.Generic;

namespace JulMar.Interfaces
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
            this.DefaultCommandIndex = 0;
            this.CancelCommandIndex = 1;
            this.Commands = new List<IUICommand> {command};
        }

        /// <summary>
        /// Constructor to create simple prompt
        /// </summary>
        public MessageVisualizerOptions(IEnumerable<IUICommand> commands = null)
        {
            this.DefaultCommandIndex = 0;
            this.CancelCommandIndex = 1;
            this.Commands = commands != null ? new List<IUICommand>(commands) : new List<IUICommand>();
        }
    }
}