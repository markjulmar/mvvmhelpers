using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;

namespace JulMar.Windows.Extensions
{
    /// <summary>
    /// Extension methods for the Task type
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// This can be used to remove compiler warnings when using async/await
        /// without consuming the results.
        /// </summary>
        /// <param name="task">The task to ignore</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NoWarning(this Task task)
        {
            /* Do nothing */
        }

        /// <summary>
        /// This can be used to remove compiler warnings when using async/await
        /// without consuming the results.
        /// </summary>
        /// <param name="asyncInfo">The WinRT operation to ignore</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NoWarning(this IAsyncInfo asyncInfo)
        {
            /* Do nothing */
        }
    }
}
