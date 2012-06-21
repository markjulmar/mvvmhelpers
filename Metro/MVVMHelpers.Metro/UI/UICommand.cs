using System.Collections.Generic;
using Windows.UI.Popups;

namespace JulMar.Windows.UI
{
    /// <summary>
    /// A UI command to execute
    /// </summary>
    public sealed class UICommand : IUICommand
    {
        /// <summary>
        /// Id representing this command
        /// </summary>
        public object Id { get; set; }

        /// <summary>
        /// The handler
        /// </summary>
        public UICommandInvokedHandler Invoked { get; set; }

        /// <summary>
        /// The text to display
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public UICommand()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="label"></param>
        /// <param name="action"></param>
        public UICommand(object id, string label, UICommandInvokedHandler action = null)
        {
            Id = id;
            Label = label;
            Invoked = action;
        }

        /// <summary>
        /// OK command
        /// </summary>
        static readonly IUICommand OkCommand = new UICommand(0, "OK");
        public static IUICommand Ok
        {
            get { return OkCommand; }
        }

        /// <summary>
        /// Cancel command
        /// </summary>
        static readonly IUICommand CancelCommand = new UICommand(1, "Cancel");
        public static IUICommand Cancel
        {
            get { return CancelCommand; }
        }

        /// <summary>
        /// Method to generate OK/Cancel buttons
        /// </summary>
        public static IEnumerable<IUICommand> OkCancel
        {
            get { return new[] { Ok, Cancel }; }
        }

        /// <summary>
        /// Yes command
        /// </summary>
        static readonly IUICommand YesCommand = new UICommand(2, "Yes");
        public static IUICommand Yes
        {
            get { return YesCommand; }
        }

        /// <summary>
        /// No command
        /// </summary>
        static readonly IUICommand NoCommand = new UICommand(3, "No");
        public static IUICommand No
        {
            get { return NoCommand; }
        }

        /// <summary>
        /// Method to generate Yes/No buttons
        /// </summary>
        public static IEnumerable<IUICommand> YesNo
        {
            get { return new[] { Yes, No }; }
        }

        /// <summary>
        /// Method to generate Yes/No/Cancel buttons
        /// </summary>
        public static IEnumerable<IUICommand> YesNoCancel
        {
            get { return new[] { Yes, No, Cancel }; }
        }

        /// <summary>
        /// Allow command
        /// </summary>
        static readonly IUICommand AllowCommand = new UICommand(4, "Allow");
        public static IUICommand Allow
        {
            get { return AllowCommand; }
        }

        /// <summary>
        /// Block command
        /// </summary>
        static readonly IUICommand BlockCommand = new UICommand(5, "Block");
        public static IUICommand Block
        {
            get { return BlockCommand; }
        }

        /// <summary>
        /// Method to generate Allow/Block buttons
        /// </summary>
        public static IEnumerable<IUICommand> AllowBlock
        {
            get { return new[] {Allow, Block}; }
        }
    }
}