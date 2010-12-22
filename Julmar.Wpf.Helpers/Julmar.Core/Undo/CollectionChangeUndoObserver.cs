using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using JulMar.Core.Interfaces;

namespace JulMar.Core.Undo
{
    /// <summary>
    /// This class provides a simple INotifyCollectionChanged observer that will add 
    /// undo/redo support to a collection class automatically by monitoring the collection
    /// changed events.
    /// </summary>
    public class CollectionChangeUndoObserver : IDisposable, IWeakEventListener
    {
        private IUndoService _undoService;
        private readonly WeakReference _collection;

        /// <summary>
        /// Set this to ignore changes temporarily while you perform
        /// some action to the tracked collection.
        /// </summary>
        public bool IgnoreChanges
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="collection">Collection to monitor</param>
        /// <param name="undoService">Undo service</param>
        public CollectionChangeUndoObserver(IList collection, IUndoService undoService)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (undoService == null)
                throw new ArgumentNullException("undoService");

            var incc = collection as INotifyCollectionChanged;
            if (incc == null)
                throw new ArgumentException("Collection must implement INotifyCollectionChanged.");

            // Save off the data.
            _undoService = undoService;
            _collection = new WeakReference(collection);

            // Add a listener to the collection.
            CollectionChangedEventManager.AddListener(incc, this);
        }

        /// <summary>
        /// Returns the IList object held by this observer.
        /// </summary>
        /// <returns></returns>
        private IList GetListObject()
        {
            try
            {
                return (IList)_collection.Target;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// This is invoked when the collection changes.
        /// </summary>
        /// <param name="sender">Collection</param>
        /// <param name="e">Change event</param>
        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IgnoreChanges)
                return;
            var undoService = _undoService;
            if (undoService == null)
                return;

            IList collection = (IList) sender;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        var item = e.NewItems[i];
                            undoService.Add(
                                new CollectionChangeUndo(collection, CollectionChangeType.Add, 
                                    -1, e.NewStartingIndex + i, null, item));
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        var item = e.OldItems[i];
                            undoService.Add(
                                new CollectionChangeUndo(collection, CollectionChangeType.Remove, 
                                    e.OldStartingIndex + i, -1, item, null));
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                        undoService.Add(
                            new CollectionChangeUndo(collection, CollectionChangeType.Replace, 
                                e.OldStartingIndex, e.NewStartingIndex, e.OldItems[0], e.NewItems[0]));
                    break;
                case NotifyCollectionChangedAction.Move:
                        undoService.Add(
                            new CollectionChangeUndo(collection, CollectionChangeType.Move,
                                e.OldStartingIndex, e.NewStartingIndex, e.NewItems[0], e.NewItems[0]));
                    break;
            }
        }

        /// <summary>
        /// Receives events from the centralized event manager.
        /// </summary>
        /// <returns>
        /// true if the listener handled the event. It is considered an error by the <see cref="T:System.Windows.WeakEventManager"/> handling in WPF to register a listener for an event that the listener does not handle. Regardless, the method should return false if it receives an event that it does not recognize or handle.
        /// </returns>
        /// <param name="managerType">The type of the <see cref="T:System.Windows.WeakEventManager"/> calling this method.</param><param name="sender">Object that originated the event.</param><param name="e">Event data.</param>
        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(CollectionChangedEventManager))
                OnCollectionChanged(sender, (NotifyCollectionChangedEventArgs)e);

            return true;
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            IList source = GetListObject();
            if (source != null)
            {
                CollectionChangedEventManager.RemoveListener(source as INotifyCollectionChanged, this);
                _collection.Target = null;
            }

            _undoService = null;
        }

        #endregion
    }
}