using System;
using System.Collections.Generic;

namespace JulMar.Windows.Extensions
{
    /// <summary>
    /// A set of collection extension methods
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Enumerates a collection and executes a predicate against each item
        /// </summary>
        /// <typeparam name="T">Type of elements</typeparam>
        /// <param name="collection">Collection</param>
        /// <param name="action">Action to execute on each element</param>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (action == null)
                throw new ArgumentNullException("action", "Missing Action<T> to execute.");
            if (collection != null)
                foreach (var o in collection) action(o);
        }

        /// <summary>
        /// Add a range of IEnumerable collection to an existing Collection.
        /// </summary>
        ///<typeparam name="T">Type of collection</typeparam>
        ///<param name="collection">Collection</param>
        /// <param name="items">Items to add</param>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
                collection.Add(item);
        }

        ///<summary>
        /// This method tests an enumerable sequence and returns the index of the first item that
        /// passes the test.
        ///</summary>
        ///<typeparam name="T">Type of collection</typeparam>
        ///<param name="collection">Collection</param>
        ///<param name="test">Predicate test</param>
        ///<returns>Index (zero based) of first element that passed test, -1 if none did</returns>
        public static int IndexOf<T>(this IEnumerable<T> collection, Predicate<T> test)
        {
            int pos = 0;
            foreach (var item in collection)
            {
                if (test(item))
                    return pos;
                pos++;
            }
            return -1;
        }
    }
}