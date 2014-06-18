using Windows.UI.Popups;
using IUICommand = JulMar.Interfaces.IUICommand;

namespace JulMar.Extensions
{
    static class UICommandExtensions
    {
        internal class WindowsCommand : global::Windows.UI.Popups.IUICommand
        {
            public IUICommand Command { get; set; }

            public WindowsCommand(IUICommand command)
            {
                this.Command = command;
                this.Invoked = uiCommand => this.Command.Invoked();
            }

            public object Id
            {
                get
                {
                    return this.Command.Id;
                }

                set
                {
                    this.Command.Id = value;
                }
            }

            /// <summary>
            /// Gets or sets the handler for the event that is fired when the user invokes the command.
            /// </summary>
            /// <returns>
            /// The event handler for the command.
            /// </returns>
            public UICommandInvokedHandler Invoked { get; set; }

            public string Label
            {
                get
                {
                    return this.Command.Label;
                }

                set
                {
                    this.Command.Label = value;
                }
            }
        }

        public static global::Windows.UI.Popups.IUICommand ToWindowsCommand(this IUICommand command)
        {
            return new WindowsCommand(command);    
        }
    }
}
