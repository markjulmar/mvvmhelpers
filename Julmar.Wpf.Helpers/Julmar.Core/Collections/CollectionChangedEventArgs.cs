using System;
using System.Collections.Generic;

namespace JulMar.Core.Collections
{
    /// <summary>
    /// This is passed when the ThreadedCollection changes as the argument indicating the change type.
    /// </summary>
    /// <typeparam name="T">Collection type</typeparam>
    public class CollectionChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// List of added items (may be empty)
        /// </summary>
        public IList<T> AddedItems { get; private set; }

        /// <summary>
        /// List of removed items (may be empty)
        /// </summary>
        public IList<T> RemovedItems { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="addedItems">Items that have been added</param>
        /// <param name="removedItems">Items that have been removed</param>
        public CollectionChangedEventArgs(IList<T> addedItems, IList<T> removedItems)
        {
            AddedItems = addedItems ?? new List<T>();
            RemovedItems = removedItems ?? new List<T>();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Added: {0} items, Removed: {1} items", AddedItems.Count, RemovedItems.Count);
        }
    }
}
