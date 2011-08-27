using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.ComponentModel;
using System.Collections.Generic;
using JulMar.Core.Extensions;
using JulMar.Core.Interfaces;

namespace JulMar.Core.Collections
{
    /// <summary>
    /// Simple thread-safe collection that performs deferred notification on both
    /// collection and child elements.
    /// </summary>
    /// <typeparam name="T">Type added to collection</typeparam>
    public class ThreadedCollection<T> : Collection<T>, IThreadedNotifyCollection<T>
    {
        /// <summary>
        /// This event is raised when the collection is altered (add/remove/change)
        /// </summary>
        public event EventHandler<CollectionChangedEventArgs<T>> CollectionChanged;

        /// <summary>
        /// This event is raised when an element in the collection is altered (changes state)
        /// It is only reported for items that support INotifyPropertyChanged
        /// </summary>
        public event EventHandler<ElementChangedEventArgs<T>> ElementChanged;

        /// <summary>
        /// This class is used to obtain/release the lock with a Dispose pattern.
        /// </summary>
        internal class CollectionLock : IDisposable
        {
            private readonly ThreadedCollection<T> _parent;
            public CollectionLock(ThreadedCollection<T> parent)
            {
                _parent = parent;
                _parent.NotifyChanges = false;
                Monitor.Enter(_parent._collectionLock);
                Interlocked.Increment(ref _parent._lockCount);
            }

            /// <summary>
            /// Releases the lock on the collection
            /// </summary>
            public void Dispose()
            {
                Debug.Assert(_parent != null);
                if (Interlocked.Decrement(ref _parent._lockCount) == 0)
                    _parent.NotifyChanges = true;
                Monitor.Exit(_parent._collectionLock);
            }
        }
        
        // Lock object
        private readonly object _collectionLock = new object();
        private int _lockCount;

        // Change notification reporting
        private bool _notifyChanges = true;
        private bool NotifyChanges
        {
            get { return _notifyChanges; }
            set
            {
                if (_notifyChanges != value)
                {
                    _notifyChanges = value;
                    if (_notifyChanges)
                    {
                        if (_deferAdd.Count > 0 || _deferRemove.Count > 0)
                        {
                            // Notify about adds/removes
                            var deferAdd = _deferAdd.ToList();
                            var deferRemove = _deferRemove.ToList();
                            _deferAdd.Clear();
                            _deferRemove.Clear();
                            OnCollectionChanged(new CollectionChangedEventArgs<T>(deferAdd, deferRemove));
                        }

                        if (_deferItemChangedNotify.Count > 0)
                        {
                            // Notify about item changes
                            var deferItems = _deferItemChangedNotify.ToList();
                            _deferItemChangedNotify.Clear();
                            foreach (var notifyItem in deferItems)
                                OnElementChanged(notifyItem);
                        }
                    }
                }
            }
        }

        // Lists holding deferred item changes
        private readonly List<ElementChangedEventArgs<T>> _deferItemChangedNotify = new List<ElementChangedEventArgs<T>>();
        private readonly List<T> _deferAdd = new List<T>();
        private readonly List<T> _deferRemove = new List<T>();

        /// <summary>
        /// Used to enter/obtain the lock
        /// </summary>
        /// <returns>Disposable object</returns>
        public IDisposable EnterLock()
        {
            return new CollectionLock(this);
        }

        /// <summary>
        /// Removes all elements from the <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
        /// </summary>
        protected override void ClearItems()
        {
            lock (_collectionLock)
            {
                var oldList = this.ToList();
                base.ClearItems();
                if (NotifyChanges)
                    OnCollectionChanged(new CollectionChangedEventArgs<T>(null, oldList));
                else
                    _deferRemove.AddRange(oldList);
            }
        }

        /// <summary>
        /// Inserts an element into the <see cref="T:System.Collections.ObjectModel.Collection`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        protected override void InsertItem(int index, T item)
        {
            lock (_collectionLock)
            {
                INotifyPropertyChanged inpc = item as INotifyPropertyChanged;
                if (inpc != null)
                    inpc.PropertyChanged += OnItemChanged;

                base.InsertItem(index, item);

                if (NotifyChanges)
                    OnCollectionChanged(new CollectionChangedEventArgs<T>(new List<T>(new[] { item }), null));
                else
                    _deferAdd.Add(item);
            }
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            lock (_collectionLock)
            {
                T item = this[index];

                INotifyPropertyChanged inpc = item as INotifyPropertyChanged;
                if (inpc != null)
                    inpc.PropertyChanged -= OnItemChanged;

                base.RemoveItem(index);

                if (NotifyChanges)
                    OnCollectionChanged(new CollectionChangedEventArgs<T>(null,new List<T>(new[] { item })));
                else
                {
                    if (_deferAdd.Contains(item))
                        _deferAdd.Remove(item);
                    else
                        _deferRemove.Add(item);
                }
            }
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index. The value can be null for reference types.</param>
        protected override void SetItem(int index, T item)
        {
            lock (_collectionLock)
            {
                T oldItem = this[index];

                INotifyPropertyChanged inpc = oldItem as INotifyPropertyChanged;
                if (inpc != null)
                    inpc.PropertyChanged -= OnItemChanged;
                inpc = item as INotifyPropertyChanged;
                if (inpc != null)
                    inpc.PropertyChanged += OnItemChanged;

                base.SetItem(index, item);

                if (NotifyChanges)
                    OnCollectionChanged(new CollectionChangedEventArgs<T>(new List<T>(new[] { item }), new List<T>(new[] { oldItem })));
                else
                {
                    if (_deferAdd.Contains(oldItem))
                        _deferAdd.Remove(oldItem);
                    else
                        _deferRemove.Add(oldItem);
                    _deferAdd.Add(item);
                }
            }
        }

        /// <summary>
        /// EventHandler invoked when an element in the collection changes internal state.
        /// Reported through INotifyPropertyChanged
        /// </summary>
        /// <param name="sender">Child element</param>
        /// <param name="e">PropertyChange notification</param>
        private void OnItemChanged(object sender, PropertyChangedEventArgs e)
        {
            ElementChangedEventArgs<T> ecea = null;
            var ex = e as PropertyChangedEventArgsEx;
            if (ex != null)
            {
                if (ex.HasOldValue && ex.HasNewValue)
                    ecea = new ElementChangedEventArgs<T>(this, (T)sender, e.PropertyName, ex.OldValue, ex.NewValue);
                else if (ex.HasNewValue)
                    ecea = new ElementChangedEventArgs<T>(this, (T)sender, e.PropertyName, ex.NewValue);                
            }

            if (ecea == null)
                ecea = new ElementChangedEventArgs<T>(this, (T) sender, e.PropertyName);

            if (NotifyChanges)
                OnElementChanged(ecea);
            else
                _deferItemChangedNotify.Add(ecea);
        }

        /// <summary>
        /// Raises the ElementChanged event
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected virtual void OnElementChanged(ElementChangedEventArgs<T> e)
        {
            EventHandler<ElementChangedEventArgs<T>> handler = ElementChanged;
            if (handler != null) 
                handler(this, e);
        }

        /// <summary>
        /// Raises the CollectionChanged event
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected virtual void OnCollectionChanged(CollectionChangedEventArgs<T> e)
        {
            EventHandler<CollectionChangedEventArgs<T>> handler = CollectionChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("ThreadedCollection: {0} items", Count);
        }
    }
}