using System.Security.Permissions;
using System.Windows.Threading;

namespace JulMar.Wpf.Helpers.UnitTests.Extensions
{
    public static class DispatcherExtensions
    {
        /// <summary>
        /// Method to force UI events to get processed
        /// </summary>
        /// <param name="currentDispatcher">Dispatcher to run</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents(this Dispatcher currentDispatcher)
        {
            DispatcherFrame frame = new DispatcherFrame();
            currentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        private static object ExitFrame(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;
            return null;
        }
    }
}
