using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace System.Windows.Collections
{
    /// <summary>
    /// A collection subset for grouping
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CollectionSubset<T> : IReadOnlyList<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly ObservableCollection<T> _innerCollection;
        private readonly ObservableCollection<T> _baseCollection;

        /// <summary>
        /// Maximum count to display
        /// </summary>
        public int MaxCount { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="baseCollection">Collection to wrap</param>
        /// <param name="maxCount">Count to display</param>
        public CollectionSubset(ObservableCollection<T> baseCollection, int maxCount)
        {
            if (maxCount < 1 || maxCount > 100)
                throw new ArgumentOutOfRangeException("maxCount", "MaxCount must be between 1 and 100");

            MaxCount = maxCount;
            _innerCollection = new ObservableCollection<T>();
            _innerCollection.CollectionChanged += (s, e) => CollectionChanged(this, e);
            ((INotifyPropertyChanged) _innerCollection).PropertyChanged += (s, e) => PropertyChanged(this, e);

            _baseCollection = baseCollection;
            _baseCollection.CollectionChanged += ItemsCollectionChanged;
        }

        /// <summary>
        /// This tracks the next 12 items in the base collection and shows them in this collection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < MaxCount)
                    {
                        _innerCollection.Insert(e.NewStartingIndex, _baseCollection[e.NewStartingIndex]);
                        if (_innerCollection.Count > MaxCount)
                        {
                            _innerCollection.RemoveAt(MaxCount);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < MaxCount && e.NewStartingIndex < MaxCount)
                    {
                        _innerCollection.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < MaxCount)
                    {
                        _innerCollection.RemoveAt(e.OldStartingIndex);
                        _innerCollection.Add(_baseCollection[MaxCount-1]);
                    }
                    else if (e.NewStartingIndex < MaxCount)
                    {
                        _innerCollection.Insert(e.NewStartingIndex, _baseCollection[e.NewStartingIndex]);
                        _innerCollection.RemoveAt(MaxCount);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < MaxCount)
                    {
                        _innerCollection.RemoveAt(e.OldStartingIndex);
                        if (_baseCollection.Count >= MaxCount)
                        {
                            _innerCollection.Add(_baseCollection[MaxCount-1]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < MaxCount)
                    {
                        this[e.OldStartingIndex] = _baseCollection[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    _innerCollection.Clear();
                    while (_innerCollection.Count < _baseCollection.Count && _innerCollection.Count < MaxCount)
                    {
                        _innerCollection.Add(_baseCollection[_innerCollection.Count]);
                    }
                    break;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _innerCollection.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        /// <returns>
        /// The number of elements in the collection. 
        /// </returns>
        public int Count
        {
            get { return _innerCollection.Count; }
        }

        /// <summary>
        /// Gets the element at the specified index in the read-only list.
        /// </summary>
        /// <returns>
        /// The element at the specified index in the read-only list.
        /// </returns>
        /// <param name="index">The zero-based index of the element to get. </param>
        public T this[int index]
        {
            get { return _innerCollection[index]; }
            private set { _innerCollection[index] = value; }
        }

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged = delegate { };

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
