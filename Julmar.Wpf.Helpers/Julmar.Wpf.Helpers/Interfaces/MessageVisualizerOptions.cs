using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace JulMar.Windows.Interfaces
{
    /// <summary>
    /// Options passed to IMessageVisualizer.  Can create derived class to pass
    /// custom data into private implementation.
    /// </summary>
    public class MessageVisualizerOptions
    {
        /// <summary>
        /// Window Owner
        /// </summary>
        public Window Owner { get; set; }

        /// <summary>
        /// Button handlers
        /// </summary>
        public IList<IUICommand> Commands { get; private set; }

        /// <summary>
        /// Constructor to create simple prompt
        /// </summary>
        public MessageVisualizerOptions(IEnumerable<IUICommand> commands = null)
        {
            Owner = null;
            Commands = commands != null ? new List<IUICommand>(commands) : new List<IUICommand>();
        }
    }
}