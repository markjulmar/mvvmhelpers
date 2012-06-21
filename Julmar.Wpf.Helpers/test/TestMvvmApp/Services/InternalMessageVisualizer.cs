using System;
using System.Linq;
using JulMar.Core;
using JulMar.Windows.Interfaces;

namespace TestMvvm.Services
{
    /// <summary>
    /// Replaced MessageVisulizer which dismisses dialog.
    /// </summary>
    [ExportService(typeof(IMessageVisualizer))]
    class InternalMessageVisualizer : IMessageVisualizer
    {
        public void Show(string title, string message)
        {
            throw new NotSupportedException();
        }

        public IUICommand Show(string title, string message, MessageVisualizerOptions visualizerOptions)
        {
            return visualizerOptions.Commands.FirstOrDefault();
        }
    }
}
