using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace JulMar.Collections
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

            this.MaxCount = maxCount;
            this._baseCollection = baseCollection;
            this._baseCollection.CollectionChanged += this.ItemsCollectionChanged;
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
                    if (e.NewStartingIndex < this.MaxCount)
                    {
                        this.Insert(e.NewStartingIndex, this._baseCollection[e.NewStartingIndex]);
                        if (this.Count > this.MaxCount)
                        {
                            this.RemoveAt(this.MaxCount);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < this.MaxCount && e.NewStartingIndex < this.MaxCount)
                    {
                        this.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < this.MaxCount)
                    {
                        this.RemoveAt(e.OldStartingIndex);
                        this.Add(this._baseCollection[this.MaxCount - 1]);
                    }
                    else if (e.NewStartingIndex < this.MaxCount)
                    {
                        this.Insert(e.NewStartingIndex, this._baseCollection[e.NewStartingIndex]);
                        this.RemoveAt(this.MaxCount);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < this.MaxCount)
                    {
                        this.RemoveAt(e.OldStartingIndex);
                        if (this._baseCollection.Count >= this.MaxCount)
                        {
                            this.Add(this._baseCollection[this.MaxCount - 1]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < this.MaxCount)
                    {
                        this[e.OldStartingIndex] = this._baseCollection[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.Clear();
                    while (this.Count < this._baseCollection.Count && this.Count < this.MaxCount)
                    {
                        this.Add(this._baseCollection[this.Count]);
                    }
                    break;
            }
        }
    }
}
