using System;
using System.Collections.Generic;
using System.Linq;
using JulMar.Windows.Interfaces;

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
        /// Method to generate OK/Cancel buttons
        /// </summary>
        public static IEnumerable<IUICommand> OkCancel
        {
            get { return Generate("OK", "Cancel"); }
        }

        /// <summary>
        /// Method to generate Yes/No buttons
        /// </summary>
        public static IEnumerable<IUICommand> YesNo
        {
            get { return Generate("Yes", "No"); }
        }

        /// <summary>
        /// Method to generate Yes/No/Cancel buttons
        /// </summary>
        public static IEnumerable<IUICommand> YesNoCancel
        {
            get { return Generate("Yes", "No", "Cancel"); }
        }

        /// <summary>
        /// Generates a set of UICommands from a set of textual labels
        /// </summary>
        /// <param name="firstLabel">First text label</param>
        /// <param name="labels">Rest of the labels</param>
        /// <returns></returns>
        public static IEnumerable<IUICommand> Generate(string firstLabel, params string[] labels)
        {
            yield return new UICommand(0, firstLabel);
            foreach (UICommand command in labels.Select((label, index) => new UICommand(index+1, label)))
                yield return command;
        }
    }
}