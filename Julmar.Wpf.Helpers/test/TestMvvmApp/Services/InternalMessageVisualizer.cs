using JulMar.Core;
using JulMar.Windows.Interfaces;

namespace TestMvvm.Services
{
    [ExportService(typeof(IMessageVisualizer))]
    class InternalMessageVisualizer : IMessageVisualizer
    {
        public MessageResult Show(string title, string message, MessageButtons buttons)
        {
            return MessageResult.Yes;
        }
    }
}
