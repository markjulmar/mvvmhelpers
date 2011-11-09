using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Data;
using System.Windows.Threading;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using JulMar.Core.Concurrency;

namespace JulMar.Windows.Collections
{
    /// <summary>
    /// This class provides an ObservableCollection which supports background thread
    /// access and updates.  It allows for concurrent read access and single write access
    /// through the use of a read/writer lock.
    /// </summary>
    /// <example>
    /// Use this in place of a normal ObservableCollection(Of(T)) - see the MTObservableTest program"/>
    /// </example>
    /// <remarks>
    /// This collection, while safe, is generally somewhat slower because it does all the write work on the UI thread.
    /// </remarks>
    /// <typeparam name="T">Type this collection holds</typeparam>
    public class MTObservableCollection<T> : IList<T>, IList, INotifyCollectionChanged, INotifyPropertyChanged
    {
        #region Private Data
        private const string CountString = "Count";
        private const string IndexerName = Binding.IndexerName;

        private readonly List<T> _masterList;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private Dispatcher _dispatcher;
        private bool _enableNotifications = true;
        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public MTObservableCollection()
        {
            _masterList = new List<T>();
        }

        /// <summary>
        /// Convenient constructor to allow LINQ sources
        /// </summary>
        /// <param name="collection">IEnumerable source</param>
        public MTObservableCollection(IEnumerable<T> collection)
        {
            _masterList = new List<T>(collection);
        }

        #region Dispatcher support
        /// <summary>
        /// Retrieves the UI dispatcher using the collection event
        /// </summary>
        private Dispatcher Dispatcher
        {
            get
            {
                if (_dispatcher == null)
                {
                    var eh = CollectionChanged;
                    if (eh != null)
                    {
                        _dispatcher = (from NotifyCollectionChangedEventHandler nh in eh.GetInvocationList()
                                       let dpo = nh.Target as DispatcherObject
                                       where dpo != null && dpo.Dispatcher != null
                                       select dpo.Dispatcher).FirstOrDefault();
                    }
                }
                return _dispatcher;
            }
        }
        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propName)
        {
            var change = PropertyChanged;
            if (change != null)
                change(this, new PropertyChangedEventArgs(propName));
        }

        #endregion

        #region INotifyCollectionChanged

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// This method notifies the UI about changes to this collection.  It uses
        /// the first located dispatcher to do the notification.
        /// </summary>
        /// <param name="e"></param>
        private void OnNotifyCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var changed = CollectionChanged;
            if (changed != null && EnableNotifications)
                changed(this, e);
        }

        /// <summary>
        /// Helper to raise CollectionChanged event to any listeners
        /// </summary> 
        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        {
            OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
        }

        /// <summary>
        /// Helper to raise CollectionChanged event to any listeners
        /// </summary> 
        private void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index)
        {
            OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
        }

        /// <summary>
        /// Helper to raise CollectionChanged event with action == Reset to any listeners
        /// </summary>
        private void OnCollectionReset()
        {
            OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #endregion

        #region Internal Thread Management Methods
        /// <summary>
        /// This method performs the given action on the dispatcher thread.
        /// </summary>
        /// <param name="work"></param>
        private void DispatcherInvoke(Action work)
        {
            if (Dispatcher == null || Dispatcher.CheckAccess())
                work();
            else
                Dispatcher.Invoke(DispatcherPriority.DataBind, work);
        }

        /// <summary>
        /// This method performs the given action on the dispatcher thread.
        /// </summary>
        /// <param name="work"></param>
        private TRv DispatcherInvoke<TRv>(Func<TRv> work)
        {
            if (Dispatcher == null || Dispatcher.CheckAccess())
                return work();

            return (TRv)Dispatcher.Invoke(DispatcherPriority.DataBind, work);
        }

        /// <summary>
        /// This method performs a write action with notification.
        /// </summary>
        /// <param name="work">Work to perform</param>
        /// <param name="notify">Notification to perform</param>
        private void PerformWriteAction(Action work, Action notify)
        {
            // Obtain an upgradable read lock
            _lock.EnterUpgradeableReadLock();

            // Followed by a write lock
            _lock.EnterWriteLock();

            // We have both upgradable read and write lock here.
            bool needsDowngrade = true;
            try
            {
                // Perform the write action
                try
                {
                    work();
                }
                // Release the write lock. We now have an upgradable read lock.
                finally
                {
                    _lock.ExitWriteLock();
                }

                // We hold a read lock while doing the notification. 
                // This ensures other threads cannot modify the collection before or during the 
                // notification.  Big warning here though - if the UI thread attempts to *modify*
                // the collection as a result of the update then this will deadlock.
                _lock.UsingReadLock(() =>
                {
                    // Release the upgradable read lock; now we hold a ReadLock only.
                    _lock.ExitUpgradeableReadLock();
                    needsDowngrade = false;

                    notify();
                });
            }
            // Release the upgradable read lock.
            finally
            {
                if (needsDowngrade)
                    _lock.ExitUpgradeableReadLock();
            }
        }
        #endregion

        /// <summary>
        /// Turns the NotifyCollectionChanged on and off.
        /// </summary>
        bool EnableNotifications
        {
            get { return _enableNotifications; }
            set
            {
                if (value != _enableNotifications)
                {
                    _enableNotifications = value;
                    if (_enableNotifications)
                    {
                        OnNotifyCollectionChanged(
                            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                        OnPropertyChanged(CountString);
                        OnPropertyChanged(IndexerName);
                    }
                }
            }
        }

        /// <summary>
        /// Method to add a series of items into the observable collection as a set.
        /// Use this if you have a bunch of items to add at once - it will turn off
        /// notifications while adding and then turn them back on when completed.
        /// </summary>
        /// <param name="items"></param>
        public void AddRange(IEnumerable<T> items)
        {
            DispatcherInvoke(() =>
                PerformWriteAction(
                    () =>
                    {
                        EnableNotifications = false;
                        foreach (T item in items)
                            _masterList.Add(item);
                    },
                    () => EnableNotifications = true
                ));
        }

        /// <summary>
        /// Iterates through a copied collection of the items and performs an action on them.
        /// The iteration is performed on a copy of the actual list (it is possible for the 
        /// real list to be changed during iteration).
        /// </summary>
        /// <param name="action">Action to perform</param>
        public void ForEach(Action<T> action)
        {
            T[] items = null;
            _lock.UsingReadLock(() => items = _masterList.ToArray());
            foreach (T item in items)
            {
                action(item);
            }
        }

        #region IList<T> Members

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        public int IndexOf(T item)
        {
            return _lock.UsingReadLock(() => _masterList.IndexOf(item));
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        public void Insert(int index, T item)
        {
            DispatcherInvoke(() =>
                PerformWriteAction(
                    () => _masterList.Insert(index, item),
                    () =>
                    {
                        OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
                        OnPropertyChanged(CountString);
                        OnPropertyChanged(IndexerName);
                    }));
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <param name="value">The object to remove from the <see cref="T:System.Collections.IList"/>. </param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception><filterpriority>2</filterpriority>
        void IList.Remove(object value)
        {
            Remove((T)value);
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            DispatcherInvoke(() =>
            {
                T item = default(T);
                PerformWriteAction(
                    () =>
                    {
                        item = _masterList[index];
                        _masterList.RemoveAt(index);
                    },
                    () =>
                    {
                        OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
                        OnPropertyChanged(CountString);
                        OnPropertyChanged(IndexerName);
                    }
                );
            });
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <param name="index">The zero-based index of the element to get or set. </param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>. </exception><exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.IList"/> is read-only. </exception><filterpriority>2</filterpriority>
        object IList.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (T)value; }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        public T this[int index]
        {
            get
            {
                return _lock.UsingReadLock(() => _masterList[index]);
            }
            set
            {
                DispatcherInvoke(() =>
                {
                    T oldValue = default(T);
                    PerformWriteAction(
                        () =>
                        {
                            oldValue = _masterList[index];
                            _masterList[index] = value;
                        },

                        () =>
                        {
                            OnCollectionChanged(NotifyCollectionChangedAction.Replace, oldValue, value, index);
                            OnPropertyChanged(IndexerName);
                        });
                });
            }
        }

        #endregion

        #region ICollection<T> Members

        /// <summary>
        /// Internal (shared) method for Add logic
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <returns>Position (not valid after return)</returns>
        private int InternalAdd(T item)
        {
            return DispatcherInvoke(() =>
            {
                int pos = -1;

                PerformWriteAction(
                    () =>
                    {
                        pos = _masterList.Count;
                        _masterList.Add(item);
                    },
                    () =>
                    {
                        OnCollectionChanged(NotifyCollectionChangedAction.Add, item, pos);
                        OnPropertyChanged(CountString);
                        OnPropertyChanged(IndexerName);
                    });

                return pos;
            });
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public void Add(T item)
        {
            InternalAdd(item);
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <returns>
        /// The position into which the new element was inserted, or -1 to indicate that the item was not inserted into the collection,
        /// </returns>
        /// <param name="value">The object to add to the <see cref="T:System.Collections.IList"/>. </param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception><filterpriority>2</filterpriority>
        int IList.Add(object value)
        {
            return InternalAdd((T)value);
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.IList"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Object"/> is found in the <see cref="T:System.Collections.IList"/>; otherwise, false.
        /// </returns>
        /// <param name="value">The object to locate in the <see cref="T:System.Collections.IList"/>. </param><filterpriority>2</filterpriority>
        bool IList.Contains(object value)
        {
            return Contains((T)value);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        public void Clear()
        {
            DispatcherInvoke(() =>
                PerformWriteAction(
                    () => _masterList.Clear(),
                    () =>
                    {
                        OnCollectionReset();
                        OnPropertyChanged(CountString);
                        OnPropertyChanged(IndexerName);
                    }));
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <returns>
        /// The index of <paramref name="value"/> if found in the list; otherwise, -1.
        /// </returns>
        /// <param name="value">The object to locate in the <see cref="T:System.Collections.IList"/>. </param><filterpriority>2</filterpriority>
        int IList.IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.IList"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="value"/> should be inserted. </param><param name="value">The object to insert into the <see cref="T:System.Collections.IList"/>. </param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>. </exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception><exception cref="T:System.NullReferenceException"><paramref name="value"/> is null reference in the <see cref="T:System.Collections.IList"/>.</exception><filterpriority>2</filterpriority>
        void IList.Insert(int index, object value)
        {
            Insert(index, (T)value);
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
            return _lock.UsingReadLock(() => _masterList.Contains(item));
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _lock.UsingReadLock(() => _masterList.CopyTo(array, arrayIndex));
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"/>. The <see cref="T:System.Array"/> must have zero-based indexing. </param><param name="index">The zero-based index in <paramref name="array"/> at which copying begins. </param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null. </exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero. </exception><exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.-or- The number of elements in the source <see cref="T:System.Collections.ICollection"/> is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>. </exception><exception cref="T:System.ArgumentException">The type of the source <see cref="T:System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <paramref name="array"/>. </exception><filterpriority>2</filterpriority>
        public void CopyTo(Array array, int index)
        {
            _lock.UsingReadLock(() => Array.Copy(_masterList.ToArray(), index, array, 0, Math.Min(_masterList.Count - index, array.Length)));
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count
        {
            get { return _lock.UsingReadLock(() => _masterList.Count); }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <returns>
        /// An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        object ICollection.SyncRoot
        {
            get { return _masterList; }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
        /// </summary>
        /// <returns>
        /// true if access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe); otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        bool ICollection.IsSynchronized
        {
            get { return false; }
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
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> has a fixed size.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.IList"/> has a fixed size; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        bool IList.IsFixedSize
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public bool Remove(T item)
        {
            return DispatcherInvoke(() =>
            {
                int index = -1;

                PerformWriteAction(
                    () =>
                    {
                        index = _masterList.IndexOf(item);
                        if (index >= 0)
                        {
                            bool rc = _masterList.Remove(item);
                            Debug.Assert(rc);
                        }
                    },
                    () =>
                    {
                        if (index >= 0)
                        {
                            OnCollectionChanged(NotifyCollectionChangedAction.Remove,
                                                item, index);
                            OnPropertyChanged(CountString);
                            OnPropertyChanged(IndexerName);
                        }
                    });

                return index >= 0;
            });
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection. </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _lock.UsingReadLock(() => _masterList.ToList().GetEnumerator());
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _lock.UsingReadLock(() => _masterList.ToList().GetEnumerator());
        }

        #endregion
    }
}