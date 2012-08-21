using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Windows.UI.Xaml;

namespace JulMar.Windows.Interactivity
{
    public class TriggerCollection : ObservableCollection<TriggerBase>
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
                            foreach (var item in e.NewItems.Cast<TriggerBase>())
                            {
                                item.Attach(_owner);
                            }
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Reset:
                    {
                        if (e.OldItems != null)
                        {
                            foreach (var item in e.OldItems.Cast<TriggerBase>())
                            {
                                item.Detach();
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
            if (_owner != null)
                throw new ArgumentException("Cannot host TriggerCollection more than once.");

            _owner = owner;
            foreach (var item in this)
            {
                item.Attach(owner);
            }
        }

        /// <summary>
        /// This detaches the behavior collection from the owner object
        /// </summary>
        public void Detach()
        {
            _owner = null;
            foreach (var item in this)
            {
                item.Detach();
            }
        }
    }

}
