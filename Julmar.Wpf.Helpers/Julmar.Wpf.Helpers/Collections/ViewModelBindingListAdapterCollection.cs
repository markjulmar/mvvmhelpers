using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace JulMar.Windows.Collections
{
    /// <summary>
    /// This class wraps an IBindingList and turns it into an ObservableCollection(Of TT) 
    /// where TS is a source presentation model (data) and TT is a ViewModel (view state)
    /// </summary>
    /// <typeparam name="TSource">Model object type</typeparam>
    /// <typeparam name="TViewModel">VM object type</typeparam>
    public sealed class ViewModelBindingListAdapterCollection<TSource, TViewModel> : ObservableCollection<TViewModel>, IDisposable
    {
        private readonly IBindingList _sourceList;
        private readonly Func<TSource, TViewModel> _viewModelCreator;
        private readonly Action<ListChangedType, TSource, TViewModel> _changeNotify;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sourceList">Original IBindingList</param>
        /// <param name="viewModelCreator">Function to create wrapper view models</param>
        /// <param name="changeNotify">Method to call when changes occur</param>
        public ViewModelBindingListAdapterCollection(
            IBindingList sourceList,
            Func<TSource, TViewModel> viewModelCreator,
            Action<ListChangedType, TSource, TViewModel> changeNotify = null)
        {
            _sourceList = sourceList;
            _viewModelCreator = viewModelCreator;
            _changeNotify = changeNotify;
            _sourceList.ListChanged += OnSourceListChanged;

            ReloadTarget();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        public void Dispose()
        {
            // Unsubscribe from the source list
            _sourceList.ListChanged -= OnSourceListChanged;

            // Get the list and clear our external representation.
            var underlyingList = this.ToList();
            this.Clear();

            // See if the child items need to be disposed.
            foreach (var item in underlyingList)
            {
                IDisposable id = item as IDisposable;
                if (id != null)
                    id.Dispose();
            }

            // If the source list needs to be disposed, do that as well.
            if (_sourceList is IDisposable)
                ((IDisposable)_sourceList).Dispose();
        }

        /// <summary>
        /// Called when the source IBindingList changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSourceListChanged(object sender, ListChangedEventArgs e)
        {
            TSource source = default(TSource);
            TViewModel dest = default(TViewModel);
            IDisposable disposeAtEnd = null;

            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    {
                        source = (TSource) _sourceList[e.NewIndex];
                        dest = _viewModelCreator(source);
                        
                        if (e.NewIndex >= Count)
                            Add(dest);
                        else
                            Insert(e.NewIndex, dest);
                    }
                    break;
                case ListChangedType.ItemDeleted:
                    if (e.NewIndex <= Count)
                    {
                        dest = this[e.NewIndex];
                        disposeAtEnd = dest as IDisposable;
                        RemoveAt(e.NewIndex);
                    }
                    break;
                case ListChangedType.ItemMoved:
                    if (e.OldIndex <= Count)
                    {
                        source = (TSource) _sourceList[e.OldIndex];
                        dest = this[e.OldIndex];
                        Move(e.OldIndex, e.NewIndex);
                    }
                    break;
                case ListChangedType.Reset:
                    ReloadTarget();
                    break;
                case ListChangedType.ItemChanged:
                default:
                    break;
            }

            if (_changeNotify != null)
            {
                _changeNotify(e.ListChangedType, source, dest);
            }
            if (disposeAtEnd != null)
                disposeAtEnd.Dispose();
        }

        /// <summary>
        /// Populates the ObservableCollection(TT) with VMs wrapping the source
        /// collection elements.
        /// </summary>
        private void ReloadTarget()
        {
            if (Count > 0)
            {
                // Get the list and clear our external representation.
                var underlyingList = this.ToList();
                this.Clear();

                // See if the child items need to be disposed.
                foreach (var item in underlyingList)
                {
                    IDisposable id = item as IDisposable;
                    if (id != null)
                        id.Dispose();
                }
            }

            foreach (var item in _sourceList)
                Add(_viewModelCreator((TSource)item));
        }
    }
}
