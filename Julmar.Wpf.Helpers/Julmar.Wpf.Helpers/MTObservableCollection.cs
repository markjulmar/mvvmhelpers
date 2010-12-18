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

namespace JulMar.Windows
{
    /// <summary>
    /// This class provides an ObservableCollection which supports background thread
    /// access and updates.  It allows for concurrent read access and single write access
    /// through the use of a MRSW lock.
    /// </summary>
    /// <remarks>
    /// This collection, while safe, is generally very slow because it does all the work on the
    /// UI thread.  You should prefer to use the better ThreadedCollection which doesn't use
    /// INotifyCollectionChanged and then maintain two collections in synch for performance.
    /// </remarks>
    /// <typeparam name="T">Type this collection holds</typeparam>
    public class MTObservableCollection<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        #region Private Data
        private const string CountString = "Count";
        private const string IndexerName = Binding.IndexerName;

        private readonly List<T> _masterList;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private Dispatcher _dispatcher;
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
            {
                change(this, new PropertyChangedEventArgs(propName));
            }
        }

        #endregion

        #region INotifyCollectionChanged

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// This acquires the writer lock.  It does this by first getting an upgradeable
        /// read lock and then getting the write lock. Later, the caller will downgrade to
        /// a read lock to notify the UI about the change.
        /// </summary>
        private void AcquireWriteLock()
        {
            // Background thread?  Just wait for the lock.
            if (Dispatcher == null || !Dispatcher.CheckAccess())
            {
                _lock.EnterUpgradeableReadLock();
                _lock.EnterWriteLock();
            }
            // UI thread? Spin waiting so background writers can send the notifications
            // properly.  Wait first for the upgradeable lock - this will block future
            // writers.  Then get the exclusive writer lock.
            else
            {
                while (!_lock.TryEnterUpgradeableReadLock(1))
                    Dispatcher.Invoke(DispatcherPriority.DataBind, ((Action)delegate { }));
                while (!_lock.TryEnterWriteLock(1))
                    Dispatcher.Invoke(DispatcherPriority.DataBind, ((Action)delegate { }));
            }
        }

        /// <summary>
        /// This method notifies the UI about changes to this collection.  It uses
        /// the first located dispatcher to do the notification.
        /// </summary>
        /// <param name="e"></param>
        private void OnNotifyCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (Dispatcher == null || Dispatcher.CheckAccess())
            {
                var changed = CollectionChanged;
                if (changed != null)
                {
                    changed(this, e);
                }
            }
            else if (CollectionChanged != null)
            {
                Dispatcher.Invoke(DispatcherPriority.DataBind, (Action<NotifyCollectionChangedEventArgs>) OnNotifyCollectionChanged, e);
            }
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
            _lock.EnterReadLock();
            try 
            {
                return _masterList.IndexOf(item);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        public void Insert(int index, T item)
        {
            bool mustDowngrade = true;
            AcquireWriteLock();
            try
            {
                try
                {
                    _masterList.Insert(index, item);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }

                _lock.EnterReadLock();
                _lock.ExitUpgradeableReadLock();
                mustDowngrade = false;
                try
                {
                    OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
                    OnPropertyChanged(CountString);
                    OnPropertyChanged(IndexerName);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            finally
            {
                if (mustDowngrade)
                    _lock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            bool mustDowngrade = true;
            AcquireWriteLock();
            try
            {
                var item = _masterList[index];
                try
                {
                    _masterList.RemoveAt(index);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }

                _lock.EnterReadLock();
                _lock.ExitUpgradeableReadLock();
                mustDowngrade = false;
                try
                {
                    OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
                    OnPropertyChanged(CountString);
                    OnPropertyChanged(IndexerName);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            finally
            {
                if (mustDowngrade)
                    _lock.ExitUpgradeableReadLock();
            }
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
                _lock.EnterReadLock();
                try
                {
                    return _masterList[index];
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                bool mustDowngrade = true;
                AcquireWriteLock();
                try
                {
                    var oldValue = _masterList[index];
                    try
                    {
                        _masterList[index] = value;
                    }
                    finally
                    {
                        _lock.ExitWriteLock();
                    }

                    _lock.EnterReadLock();
                    _lock.ExitUpgradeableReadLock();
                    mustDowngrade = false;
                    try
                    {
                        OnCollectionChanged(NotifyCollectionChangedAction.Replace, oldValue, value, index);
                        OnPropertyChanged(IndexerName);
                    }
                    finally
                    {
                        _lock.ExitReadLock();
                    }
                }
                finally
                {
                    if (mustDowngrade)
                        _lock.ExitUpgradeableReadLock();
                }
            }
        }

        #endregion

        #region ICollection<T> Members

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public void Add(T item)
        {
            bool mustDowngrade = true;
            AcquireWriteLock();
            try
            {
                int count = _masterList.Count;
                try
                {
                    _masterList.Add(item);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }

                _lock.EnterReadLock();
                _lock.ExitUpgradeableReadLock();
                mustDowngrade = false;
                try
                {
                    OnCollectionChanged(NotifyCollectionChangedAction.Add, item, count);
                    OnPropertyChanged(CountString);
                    OnPropertyChanged(IndexerName);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            finally
            {
                if (mustDowngrade)
                    _lock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        public void Clear()
        {
            bool mustDowngrade = true;
            AcquireWriteLock();
            try
            {
                try
                {
                    _masterList.Clear();
                }
                finally
                {
                    _lock.ExitWriteLock();
                }

                _lock.EnterReadLock();
                _lock.ExitUpgradeableReadLock();
                mustDowngrade = false;
                try
                {
                    OnCollectionReset();
                    OnPropertyChanged(CountString);
                    OnPropertyChanged(IndexerName);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            finally
            {
                if (mustDowngrade)
                    _lock.ExitUpgradeableReadLock();
            }
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
            _lock.EnterReadLock();
            try
            {
                return _masterList.Contains(item);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _lock.EnterReadLock();
            try
            {
                _masterList.CopyTo(array, arrayIndex);
            }
            finally
            {
                _lock.ExitReadLock();
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
            get 
            {
                _lock.EnterReadLock();
                try
                {
                    return _masterList.Count;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
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
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public bool Remove(T item)
        {
            bool mustDowngrade = true;
            AcquireWriteLock();
            try
            {
                int index = _masterList.IndexOf(item);

                try
                {
                    if (index >= 0)
                    {
                        bool rc = _masterList.Remove(item);
                        Debug.Assert(rc);
                    }
                }
                finally
                {
                    _lock.ExitWriteLock();
                }

                _lock.EnterReadLock();
                _lock.ExitUpgradeableReadLock();
                mustDowngrade = false;
                try
                {
                    if (index >= 0)
                    {
                        OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
                        OnPropertyChanged(CountString);
                        OnPropertyChanged(IndexerName);
                        return true;
                    }
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            finally
            {
                if (mustDowngrade)
                    _lock.ExitUpgradeableReadLock();
            }

            return false;
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection. </returns>
        public IEnumerator<T> GetEnumerator()
        {
            _lock.EnterReadLock();
            try
            {
                return _masterList.ToList().GetEnumerator();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            _lock.EnterReadLock();
            try
            {
                return _masterList.ToList().GetEnumerator();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        #endregion
    }
}