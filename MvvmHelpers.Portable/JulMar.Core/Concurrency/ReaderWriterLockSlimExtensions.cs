using System;
using System.Diagnostics;
using System.Threading;

namespace JulMar.Concurrency
{
    /// <summary>
    /// A set of extensions applied over ReaderWriterLockSlim to ensure the
    /// lock is properly released through the use of delegates/anonymous methods.
    /// </summary>
    public static class ReaderWriterLockSlimExtensions
    {
        /// <summary>
        /// Used to perform some action while holding the ReaderWriterLock read-lock
        /// </summary>
        /// <param name="rwl">Reader/Writer Lock</param>
        /// <param name="action">Action to perform</param>
        public static void UsingReadLock(this ReaderWriterLockSlim rwl, Action action)
        {
            bool rc = TryUsingReadLock(rwl, Timeout.Infinite, action);
            Debug.Assert(rc == true);
        }

        /// <summary>
        /// Used to perform some action while holding the ReaderWriterLock read-lock
        /// </summary>
        /// <param name="rwl">Reader/Writer Lock</param>
        /// <param name="timeout">Timeout</param>
        /// <param name="action">Action to perform</param>
        public static bool TryUsingReadLock(this ReaderWriterLockSlim rwl, int timeout, Action action)
        {
            return TryUsingReadLock(rwl, new TimeSpan(0,0,0,0,timeout), action);
        }

        /// <summary>
        /// Used to perform some action while holding the ReaderWriterLock read-lock
        /// </summary>
        /// <param name="rwl">Reader/Writer Lock</param>
        /// <param name="timeSpan">Timeout</param>
        /// <param name="action">Action to perform</param>
        public static bool TryUsingReadLock(this ReaderWriterLockSlim rwl, TimeSpan timeSpan, Action action)
        {
            if (rwl == null)
                throw new ArgumentNullException("rwl");
            if (action == null)
                throw new ArgumentNullException("action");

            if (rwl.TryEnterReadLock(timeSpan))
            {
                try
                {
                    action.Invoke();
                }
                finally
                {
                    rwl.ExitReadLock();
                }
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Used to perform some action while holding the ReaderWriterLock read-lock
        /// </summary>
        /// <param name="rwl">Reader/Writer Lock</param>
        /// <param name="action">Action to perform</param>
        public static T UsingReadLock<T>(this ReaderWriterLockSlim rwl, Func<T> action)
        {
            T result;
            bool rc = TryUsingReadLock(rwl, Timeout.Infinite, action, out result);
            Debug.Assert(rc == true);
            return result;
        }

        /// <summary>
        /// Used to perform some action while holding the ReaderWriterLock read-lock
        /// </summary>
        /// <param name="rwl">Reader/Writer Lock</param>
        /// <param name="timeout">Timeout</param>
        /// <param name="action">Action to perform</param>
        /// <param name="result">Result</param>
        public static bool TryUsingReadLock<T>(this ReaderWriterLockSlim rwl, int timeout, Func<T> action, out T result)
        {
            return TryUsingReadLock(rwl, new TimeSpan(0, 0, 0, 0, timeout), action, out result);
        }

        /// <summary>
        /// Used to perform some action while holding the ReaderWriterLock read-lock
        /// </summary>
        /// <param name="rwl">Reader/Writer Lock</param>
        /// <param name="timeSpan">Timeout</param>
        /// <param name="action">Action to perform</param>
        /// <param name="result">Result</param>
        public static bool TryUsingReadLock<T>(this ReaderWriterLockSlim rwl, TimeSpan timeSpan, Func<T> action, out T result)
        {
            if (rwl == null)
                throw new ArgumentNullException("rwl");
            if (action == null)
                throw new ArgumentNullException("action");

            if (rwl.TryEnterReadLock(timeSpan))
            {
                try
                {
                    result = action.Invoke();
                }
                finally
                {
                    rwl.ExitReadLock();
                }
                return true;
            }

            result = default(T);
            return false;
        }

        /// <summary>
        /// Used to perform some action while holding the ReaderWriterLock upgradeable read-lock
        /// </summary>
        /// <param name="rwl">Reader/Writer Lock</param>
        /// <param name="action">Action to perform</param>
        public static void UsingUpgradeableReadLock(this ReaderWriterLockSlim rwl, Action action)
        {
            bool rc = TryUsingUpgradeableReadLock(rwl, Timeout.Infinite, action);
            Debug.Assert(rc == true);
        }

        /// <summary>
        /// Used to perform some action while holding the ReaderWriterLock upgradeable read-lock
        /// </summary>
        /// <param name="rwl">Reader/Writer Lock</param>
        /// <param name="timeout">Timeout</param>
        /// <param name="action">Action to perform</param>
        public static bool TryUsingUpgradeableReadLock(this ReaderWriterLockSlim rwl, int timeout, Action action)
        {
            return TryUsingUpgradeableReadLock(rwl, new TimeSpan(0, 0, 0, 0, timeout), action);
        }

        /// <summary>
        /// Used to perform some action while holding the ReaderWriterLock upgradeable read-lock
        /// </summary>
        /// <param name="rwl">Reader/Writer Lock</param>
        /// <param name="timeSpan">Timeout</param>
        /// <param name="action">Action to perform</param>
        public static bool TryUsingUpgradeableReadLock(this ReaderWriterLockSlim rwl, TimeSpan timeSpan, Action action)
        {
            if (rwl == null)
                throw new ArgumentNullException("rwl");
            if (action == null)
                throw new ArgumentNullException("action");

            if (rwl.TryEnterUpgradeableReadLock(timeSpan))
            {
                try
                {
                    action.Invoke();
                }
                finally
                {
                    rwl.ExitUpgradeableReadLock();
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Used to perform some action while holding the ReaderWriterLock upgradeable read-lock
        /// </summary>
        /// <param name="rwl">Reader/Writer Lock</param>
        /// <param name="action">Action to perform</param>
        public static T UsingUpgradeableReadLock<T>(this ReaderWriterLockSlim rwl, Func<T> action)
        {
            T result;
            bool rc = TryUsingUpgradeableReadLock(rwl, Timeout.Infinite, action, out result);
            Debug.Assert(rc == true);
            return result;
        }

        /// <summary>
        /// Used to perform some action while holding the ReaderWriterLock upgradable read-lock
        /// </summary>
        /// <param name="rwl">Reader/Writer Lock</param>
        /// <param name="timeout">Timeout</param>
        /// <param name="action">Action to perform</param>
        /// <param name="result">Result</param>
        public static bool TryUsingUpgradeableReadLock<T>(this ReaderWriterLockSlim rwl, int timeout, Func<T> action, out T result)
        {
            return TryUsingUpgradeableReadLock(rwl, new TimeSpan(0, 0, 0, 0, timeout), action, out result);
        }

        /// <summary>
        /// Used to perform some action while holding the ReaderWriterLock upgradable read-lock
        /// </summary>
        /// <param name="rwl">Reader/Writer Lock</param>
        /// <param name="timeSpan">Timeout</param>
        /// <param name="action">Action to perform</param>
        /// <param name="result">Result</param>
        public static bool TryUsingUpgradeableReadLock<T>(this ReaderWriterLockSlim rwl, TimeSpan timeSpan, Func<T> action, out T result)
        {
            if (rwl == null)
                throw new ArgumentNullException("rwl");
            if (action == null)
                throw new ArgumentNullException("action");

            if (rwl.TryEnterUpgradeableReadLock(timeSpan))
            {
                try
                {
                    result = action.Invoke();
                }
                finally
                {
                    rwl.ExitUpgradeableReadLock();
                }
                return true;
            }

            result = default(T);
            return false;
        }

        /// <summary>
        /// Used to perform some action while holding the ReaderWriterLock write-lock
        /// </summary>
        /// <param name="rwl">Reader/Writer Lock</param>
        /// <param name="action">Action to perform</param>
        public static void UsingWriteLock(this ReaderWriterLockSlim rwl, Action action)
        {
            bool rc = TryUsingWriteLock(rwl, Timeout.Infinite, action);
            Debug.Assert(rc == true);
        }

        /// <summary>
        /// Used to perform some action while holding the ReaderWriterLock write-lock
        /// </summary>
        /// <param name="rwl">Reader/Writer Lock</param>
        /// <param name="timeout">Timeout</param>
        /// <param name="action">Action to perform</param>
        public static bool TryUsingWriteLock(this ReaderWriterLockSlim rwl, int timeout, Action action)
        {
            return TryUsingWriteLock(rwl, new TimeSpan(0, 0, 0, 0, timeout), action);
        }

        /// <summary>
        /// Used to perform some action while holding the ReaderWriterLock write-lock
        /// </summary>
        /// <param name="rwl">Reader/Writer Lock</param>
        /// <param name="timeSpan">Timeout</param>
        /// <param name="action">Action to perform</param>
        public static bool TryUsingWriteLock(this ReaderWriterLockSlim rwl, TimeSpan timeSpan, Action action)
        {
            if (rwl == null)
                throw new ArgumentNullException("rwl");
            if (action == null)
                throw new ArgumentNullException("action");

            if (rwl.TryEnterWriteLock(timeSpan))
            {
                try
                {
                    action.Invoke();
                }
                finally
                {
                    rwl.ExitWriteLock();
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Used to perform some action while holding the ReaderWriterLock write-lock
        /// </summary>
        /// <param name="rwl">Reader/Writer Lock</param>
        /// <param name="action">Action to perform</param>
        public static T UsingWriteLock<T>(this ReaderWriterLockSlim rwl, Func<T> action)
        {
            T result;
            bool rc = TryUsingWriteLock(rwl, Timeout.Infinite, action, out result);
            Debug.Assert(rc == true);
            return result;
        }

        /// <summary>
        /// Used to perform some action while holding the ReaderWriterLock write-lock
        /// </summary>
        /// <param name="rwl">Reader/Writer Lock</param>
        /// <param name="timeout">Timeout</param>
        /// <param name="action">Action to perform</param>
        /// <param name="result">Result</param>
        public static bool TryUsingWriteLock<T>(this ReaderWriterLockSlim rwl, int timeout, Func<T> action, out T result)
        {
            return TryUsingWriteLock(rwl, new TimeSpan(0, 0, 0, 0, timeout), action, out result);
        }

        /// <summary>
        /// Used to perform some action while holding the ReaderWriterLock write-lock
        /// </summary>
        /// <param name="rwl">Reader/Writer Lock</param>
        /// <param name="timeSpan">Timeout</param>
        /// <param name="action">Action to perform</param>
        /// <param name="result">Result</param>
        public static bool TryUsingWriteLock<T>(this ReaderWriterLockSlim rwl, TimeSpan timeSpan, Func<T> action, out T result)
        {
            if (rwl == null)
                throw new ArgumentNullException("rwl");
            if (action == null)
                throw new ArgumentNullException("action");

            if (rwl.TryEnterWriteLock(timeSpan))
            {
                try
                {
                    result = action.Invoke();
                }
                finally
                {
                    rwl.ExitWriteLock();
                }
                return true;
            }

            result = default(T);
            return false;
        }
    }
}
