using System;
using System.Collections.Generic;
using JulMar.Windows.Interfaces;

namespace JulMar.Windows.UI
{
    /// <summary>
    /// A UI command to execute
    /// </summary>
    public sealed class UICommand : IUICommand
    {
        static readonly IUICommand OkCommand = new UICommand(0, "OK");
        static readonly IUICommand CancelCommand = new UICommand(1, "Cancel");
        static readonly IUICommand YesCommand = new UICommand(2, "Yes");
        static readonly IUICommand NoCommand = new UICommand(3, "No");
        static readonly IUICommand AllowCommand = new UICommand(4, "Allow");
        static readonly IUICommand BlockCommand = new UICommand(5, "Block");

        /// <summary>
        /// Id representing this command
        /// </summary>
        public object Id { get; set; }

        /// <summary>
        /// The handler
        /// </summary>
        public Action Invoked { get; set; }

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
        public UICommand(object id, string label, Action action = null)
        {
            Id = id;
            Label = label;
            Invoked = action;
        }

        /// <summary>
        /// OK command
        /// </summary>
        public static IUICommand Ok
        {
            get { return OkCommand; }
        }

        /// <summary>
        /// Cancel command
        /// </summary>
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
        public static IUICommand Yes
        {
            get { return YesCommand; }
        }

        /// <summary>
        /// No command
        /// </summary>
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
        public static IUICommand Allow
        {
            get { return AllowCommand; }
        }

        /// <summary>
        /// Block command
        /// </summary>
        public static IUICommand Block
        {
            get { return BlockCommand; }
        }

        /// <summary>
        /// Method to generate Allow/Block buttons
        /// </summary>
        public static IEnumerable<IUICommand> AllowBlock
        {
            get { return new[] { Allow, Block }; }
        }
    }
}