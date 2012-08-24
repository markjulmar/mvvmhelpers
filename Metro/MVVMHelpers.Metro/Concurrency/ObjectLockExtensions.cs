using System;
using System.Diagnostics;
using System.Threading;

namespace JulMar.Core.Concurrency
{
    /// <summary>
    /// This class provides a functional way to obtain a monitor and
    /// invoke an action.  This allows for an elegant syntax to Monitor.TryEnter.
    /// </summary>
    /// <example>
    /// <![CDATA[
    ///   object myLock = new object();
    ///   ...
    ///   myLock.UsingLock(() => DoSomeThreadSafeWorkHere());
    ///   myLock.TryUsingLock(TimeSpan.FromSeconds(10), () => CalculateValues());
    /// ]]>
    /// </example>
    public static class ObjectLockExtensions
    {
        /// <summary>
        /// Performs an action while holding a Monitor; infinite wait
        /// </summary>
        /// <param name="monitor">Object to lock</param>
        /// <param name="action">Action to invoke</param>
        public static void UsingLock(this object monitor, Action action)
        {
            TryUsingLock(monitor, Timeout.Infinite, action);
        }

        /// <summary>
        /// Performs an action while holding a Monitor; infinite wait
        /// </summary>
        /// <typeparam name="T">Result type</typeparam>
        /// <param name="monitor">Object to lock</param>
        /// <param name="action">Action to invoke</param>
        /// <returns>Result from action</returns>
        public static T UsingLock<T>(this object monitor, Func<T> action)
        {
            T result;
            bool rc = TryUsingLock(monitor, Timeout.Infinite, action, out result);
            Debug.Assert(rc == true);
            return result;
        }

        /// <summary>
        /// Attempts to obtain monitor and performs action.
        /// </summary>
        /// <param name="monitor">Object to lock</param>
        /// <param name="timeout">Timeout</param>
        /// <param name="action">Action to invoke</param>
        /// <returns>True if action invoked</returns>
        public static bool TryUsingLock(this object monitor, int timeout, Action action)
        {
            return TryUsingLock(monitor, new TimeSpan(0, 0, 0, 0, timeout), action);
        }

        /// <summary>
        /// Attempts to obtain monitor and performs action.
        /// </summary>
        /// <param name="monitor">Object to lock</param>
        /// <param name="timeSpan">Timeout</param>
        /// <param name="action">Action to invoke</param>
        /// <returns>True if action invoked</returns>
        public static bool TryUsingLock(this object monitor, TimeSpan timeSpan, Action action)
        {
            if (monitor == null)
                throw new ArgumentNullException("monitor");
            if (action == null)
                throw new ArgumentNullException("action");

            bool rc = Monitor.TryEnter(monitor, timeSpan);
            if (rc)
            {
                try
                {
                    action.Invoke();
                }
                finally
                {
                    Monitor.Exit(monitor);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempts to obtain monitor and performs action that returns result.
        /// </summary>
        /// <typeparam name="T">Result type</typeparam>
        /// <param name="monitor">Object to lock</param>
        /// <param name="timeout">Timeout to wait</param>
        /// <param name="action">Action to perform</param>
        /// <param name="result">Result from action</param>
        /// <returns>True if action invoked</returns>
        public static bool TryUsingLock<T>(this object monitor, int timeout, Func<T> action, out T result)
        {
            return TryUsingLock(monitor, new TimeSpan(0, 0, 0, 0, timeout), action, out result);
        }

        /// <summary>
        /// Attempts to obtain monitor and performs action that returns result.
        /// </summary>
        /// <typeparam name="T">Result type</typeparam>
        /// <param name="monitor">Object to lock</param>
        /// <param name="timeSpan">Timeout to wait</param>
        /// <param name="action">Action to perform</param>
        /// <param name="result">Result from action</param>
        /// <returns>True if action invoked</returns>
        public static bool TryUsingLock<T>(this object monitor, TimeSpan timeSpan, Func<T> action, out T result)
        {
            if (monitor == null)
                throw new ArgumentNullException("monitor");
            if (action == null)
                throw new ArgumentNullException("action");

            bool rc = Monitor.TryEnter(monitor, timeSpan);
            if (rc)
            {
                try
                {
                    result = action.Invoke();
                }
                finally
                {
                    Monitor.Exit(monitor);
                }
                return true;
            }
            
            result = default(T);
            return false;
        }

    }
}
