using System.Threading.Tasks;

namespace JulMar.Extensions
{
    /// <summary>
    /// Task extension
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Removes compiler warning about not consuming awaitable task.
        /// </summary>
        /// <param name="task"></param>
        public static void IgnoreResult(this Task task)
        {
            // Do nothing
        }
    }
}
