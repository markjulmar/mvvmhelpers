using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace JulMar.Core.Collections
{
    /// <summary>
    /// A collection subset for grouping
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CollectionSubset<T> : ObservableCollection<T>
    {
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
            _baseCollection = baseCollection;
            _baseCollection.CollectionChanged += ItemsCollectionChanged;
        }

        /// <summary>
        /// This tracks the next MaxCount items in the base collection and shows them in this collection.
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
                        this.Insert(e.NewStartingIndex, _baseCollection[e.NewStartingIndex]);
                        if (this.Count > MaxCount)
                        {
                            this.RemoveAt(MaxCount);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < MaxCount && e.NewStartingIndex < MaxCount)
                    {
                        this.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < MaxCount)
                    {
                        this.RemoveAt(e.OldStartingIndex);
                        this.Add(_baseCollection[MaxCount-1]);
                    }
                    else if (e.NewStartingIndex < MaxCount)
                    {
                        this.Insert(e.NewStartingIndex, _baseCollection[e.NewStartingIndex]);
                        this.RemoveAt(MaxCount);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < MaxCount)
                    {
                        this.RemoveAt(e.OldStartingIndex);
                        if (_baseCollection.Count >= MaxCount)
                        {
                            this.Add(_baseCollection[MaxCount-1]);
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
                    this.Clear();
                    while (this.Count < _baseCollection.Count && this.Count < MaxCount)
                    {
                        this.Add(_baseCollection[this.Count]);
                    }
                    break;
            }
        }
    }
}
