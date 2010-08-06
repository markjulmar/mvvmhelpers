using System;
using System.Collections.Generic;
using System.Collections;

namespace JulMar.Windows
{
    /// <summary>
    /// A List(Of T) that implements weak reference semantics.
    /// Elements in the list can be collected if no other reference exists to the object.
    /// The list automatically cleans up when any item is added or removed.
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public sealed class WeakReferenceList<T> : IList<T>
    {
        private readonly List<WeakReference> _items = new List<WeakReference>();

        /// <summary>
        /// Default constructor
        /// </summary>
        public WeakReferenceList()
        {
        }

        /// <summary>
        /// Constructor which takes an existing list
        /// </summary>
        /// <param name="existingData"></param>
        public WeakReferenceList(ICollection<T> existingData)
        {
            if (existingData != null && existingData.Count > 0)
            {
                foreach (T item in existingData)
                    Add(item);
            }
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public void Add(T item)
        {
            RemoveDeadItems();
            _items.Add(new WeakReference(item));
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
        public void Clear()
        {
            _items.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public bool Contains(T item)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                T value;
                if (GetItem( i, out value) && item.Equals(value))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from 
        /// <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                T value;
                if (GetItem(i, out value))
                    array[arrayIndex++] = value;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count
        {
            get { return _items.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public bool Remove(T item)
        {
            RemoveDeadItems();

            for (int i = 0; i < _items.Count; i++)
            {
                T value;
                if (GetItem(i, out value) && item.Equals(value))
                {
                    _items.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        public int IndexOf(T item)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                T value;
                if (GetItem(i, out value) && item.Equals(value))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param><param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        public void Insert(int index, T item)
        {
            RemoveDeadItems();
            _items.Insert(index, new WeakReference(item));
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        public void RemoveAt(int index)
        {
            T item;
            GetItem(index, out item);
            _items.RemoveAt(index);
            RemoveDeadItems();
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <param name="index">The zero-based index of the element to get or set.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        public T this[int index]
        {
            get
            {
                T item;
                return GetItem(index, out item) ? item : default(T);
            }
            set
            {
                _items[index] = new WeakReference(value);
            }
        }

        /// <summary>
        /// Converts to a List with strong references to all active items
        /// </summary>
        /// <returns></returns>
        public List<T> ToList()
        {
            List<T> retList = new List<T>(Count);
            for (int i = 0; i < _items.Count; i++)
            {
                T value;
                if (GetItem(i, out value))
                    retList.Add(value);
            }
            return retList;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator()
        {
            return DoEnumeration();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return DoEnumeration();
        }

        /// <summary>
        /// Private method to run enumeration
        /// </summary>
        /// <returns></returns>
        private IEnumerator<T> DoEnumeration()
        {
            RemoveDeadItems();

            var items = _items.ToArray();
            for (int i = 0; i < items.Length; i++)
            {
                WeakReference wr = items[i];
                if (wr != null && wr.IsAlive)
                {
                    var item = (T) wr.Target;
                    GC.KeepAlive(item);
                    if (wr.IsAlive)
                        yield return item;
                }
            }
        }

        /// <summary>
        /// This method removes all items which have been collected.
        /// </summary>
        private void RemoveDeadItems()
        {
            int current = 0;
            while (current < _items.Count)
            {
                T value;
                if (!GetItem(current, out value))
                    _items.RemoveAt(current);
                else
                    current++;
            }
        }

        /// <summary>
        /// This retrieves a specific item by index.
        /// </summary>
        /// <param name="index">Index to retrieve</param>
        /// <param name="value">Returning value, null if item was collected</param>
        /// <returns>True if item was present at index</returns>
        private bool GetItem(int index, out T value)
        {
            WeakReference weakref = _items[index];
            if (weakref != null && weakref.IsAlive)
            {
                value = (T)weakref.Target;
                return true;
            }
            value = default(T);
            return false;
        }
    }
}