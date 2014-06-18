using System;
using System.Text;

namespace JulMar.Extensions
{
    /// <summary>
    /// Extensions for the global Exception type
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Flatten the exception and inner exception data.
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="message">Any string prefix to add</param>
        /// <param name="includeStackTrace">True to include stack trace at end</param>
        /// <returns>String with Message and all InnerException messages appended together</returns>
        public static string Flatten(this Exception ex, string message = "", bool includeStackTrace = false)
        {
            StringBuilder sb = new StringBuilder(message);

            Exception current = ex;
            while (current != null)
            {
                sb.AppendLine(current.Message);
                if (includeStackTrace)
                    sb.Append(ex.StackTrace);
                
                current = current.InnerException;
                if (current != null && includeStackTrace)
                    sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
