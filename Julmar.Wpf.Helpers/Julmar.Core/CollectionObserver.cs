using System;
using System.Linq;
using System.Collections.Specialized;
using System.Windows;

namespace JulMar.Core
{
    /// <summary>
    /// This class manages a notify-able collection, holding onto a WeakReference so
    /// the collection itself can go away.
    /// </summary>
    public sealed class CollectionObserver : IDisposable, IWeakEventListener
    {
        private readonly WeakReference _source;

        /// <summary>
        /// Event list - targets associated with this event are called when
        /// the NotifyCollectionChanged occurs.
        /// </summary>
        public event EventHandler<NotifyCollectionChangedEventArgs> CollectionChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="collection">Collection to monitor</param>
        public CollectionObserver(INotifyCollectionChanged collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            _source = new WeakReference(collection);
            CollectionChangedEventManager.AddListener(collection, this);
        }

        /// <summary>
        /// Retrieves the collection source
        /// </summary>
        private INotifyCollectionChanged GetSource()
        {
            try
            {
                return (_source.IsAlive) ? (INotifyCollectionChanged)_source.Target : null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            INotifyCollectionChanged source = GetSource();
            if (source != null)
                CollectionChangedEventManager.RemoveListener(source, this);
            CollectionChanged = null;
        }

        /// <summary>
        /// Receives events from the centralized event manager.
        /// </summary>
        /// <returns>
        /// true if the listener handled the event. It is considered an error by the <see cref="T:System.Windows.WeakEventManager"/> handling in WPF to register a listener for an event that the listener does not handle. Regardless, the method should return false if it receives an event that it does not recognize or handle.
        /// </returns>
        /// <param name="managerType">The type of the <see cref="T:System.Windows.WeakEventManager"/> calling this method.</param><param name="sender">Object that originated the event.</param><param name="e">Event data.</param>
        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(CollectionChangedEventManager))
            {
                var collectionChanged = CollectionChanged;
                if (collectionChanged != null)
                {
                    collectionChanged.GetInvocationList()
                        .Cast<EventHandler<NotifyCollectionChangedEventArgs>>()
                        .ToList().ForEach(callback =>
                                              {
                                                  try
                                                  {
                                                      callback.Invoke(sender, (NotifyCollectionChangedEventArgs) e);
                                                  }
                                                  catch
                                                  {
                                                      CollectionChanged -= callback;
                                                  }
                                              });
                }
            }

            return true;
        }
    }
}
