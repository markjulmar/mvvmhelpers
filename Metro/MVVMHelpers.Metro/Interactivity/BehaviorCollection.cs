using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Windows.UI.Xaml;

namespace JulMar.Windows.Interactivity
{
    /// <summary>
    /// Collection of behaviors to apply to an object
    /// </summary>
    public class BehaviorCollection : ObservableCollection<Behavior>
    {
        private FrameworkElement _owner;

        /// <summary>
        /// Raises the <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged"/> event with the provided arguments.
        /// </summary>
        /// <param name="e">Arguments of the event being raised.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_owner == null)
                return;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (e.NewItems != null)
                        {
                            foreach (var i in e.NewItems.Cast<Behavior>())
                            {
                                i.AssociatedObjectInternal = _owner;
                            }
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Reset:
                    {
                        if (e.OldItems != null)
                        {
                            foreach (var i in e.OldItems.Cast<Behavior>())
                            {
                                i.AssociatedObjectInternal = null;
                            }
                        }
                        break;
                    }
            }

            base.OnCollectionChanged(e);
        }

        /// <summary>
        /// This attaches the behavior collection to a specific element
        /// </summary>
        /// <param name="owner"></param>
        public void Attach(FrameworkElement owner)
        {
            _owner = owner;
            foreach (var i in this)
            {
                i.AssociatedObjectInternal = _owner;
            }
        }

        /// <summary>
        /// This detaches the behavior collection from the owner object
        /// </summary>
        public void Detach()
        {
            _owner = null;
            foreach (var i in this)
            {
                i.AssociatedObjectInternal = null;
            }
        }

    }
}
