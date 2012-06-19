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
        }

        public object Show(string title, string message, MessageVisualizerOptions visualizerOptions)
        {
            return visualizerOptions.Commands[0].Id;
        }
    }
}
