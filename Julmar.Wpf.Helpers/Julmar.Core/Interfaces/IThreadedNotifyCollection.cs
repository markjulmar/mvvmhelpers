using System;
using System.Collections.Generic;
using JulMar.Core.Collections;

namespace JulMar.Core.Interfaces
{
    /// <summary>
    /// Interface describing a thread-safe collection that performs notifications.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IThreadedNotifyCollection<T> : IList<T>
    {
        /// <summary>
        /// This event is raised when the collection is altered (add/remove/change)
        /// </summary>
        event EventHandler<CollectionChangedEventArgs<T>> CollectionChanged;

        /// <summary>
        /// This event is raised when an element in the collection is altered (changes state)
        /// It is only reported for items that support INotifyPropertyChanged
        /// </summary>
        event EventHandler<ElementChangedEventArgs<T>> ElementChanged;

        /// <summary>
        /// Used to enter/obtain the lock
        /// </summary>
        /// <returns></returns>
        IDisposable EnterLock();
    }
}