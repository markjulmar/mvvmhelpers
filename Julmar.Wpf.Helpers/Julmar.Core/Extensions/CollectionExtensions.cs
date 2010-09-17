using System;
using System.Linq;
using System.Collections.Generic;

namespace JulMar.Core.Extensions
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
        /// This is used to compare two collections.
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection">Collection Source</param>
        /// <param name="other">Collection to compare to</param>
        /// <param name="exactMatch">Require same-order elements (exact match)</param>
        /// <returns></returns>
        public static bool Compare<T>(this ICollection<T> collection, ICollection<T> other, bool exactMatch=false)
        {
            if (!ReferenceEquals(collection, other))
            {
                if (other == null)
                    throw new ArgumentNullException("other");

                // Not the same number of elements.  No match
                if (collection.Count != other.Count)
                    return false;

                // Require same-order; just defer to existing LINQ match
                if (exactMatch)
                    return collection.SequenceEqual(other);

                // Otherwise allow it to be any order, but require same count of each item type.
                var comparer = EqualityComparer<T>.Default;
                return !(from item in collection
                         let thisItem = item
                         where !other.Contains(item, comparer) || collection.Count(check => comparer.Equals(thisItem, check)) != other.Count(check => comparer.Equals(thisItem, check))
                         select item).Any();
            }

            return true;
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