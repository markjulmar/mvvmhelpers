using Windows.ApplicationModel;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace JulMar.Windows.Interactivity
{
    /// <summary>
    /// This behavior provides a multi-selection binder to tie a ViewModel collection to
    /// a multi-select list.  This includes ListBox, ListView, GridView, and anything
    /// else which supports the Selector or MultiSelector base class.
    /// 
    /// This can be attached through the normal Interaction.Behaviors mechanism 
    /// or through the IsEnabled attached property (set to a collection).
    /// </summary>
    public class SynchronizedCollectionBehavior : DependencyObject, IBehavior
    {
        private Selector _associatedObject;
        private volatile bool _updatingList;

        /// <summary>
        /// Property to associate selected items
        /// </summary>
        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register("Collection", typeof(IEnumerable), typeof(SynchronizedCollectionBehavior),
                                new PropertyMetadata(null, OnBackingCollectionChanged));

        /// <summary>
        /// Instance wrapper for selected items backing storage
        /// </summary>
        public IEnumerable Collection
        {
            get { return (IEnumerable)GetValue(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }

        /// <summary>
        /// Property to associate selected items
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(IEnumerable), typeof(SynchronizedCollectionBehavior),
                                new PropertyMetadata(null, OnEnabledChanged));

        /// <summary>
        /// Get the collection value
        /// </summary>
        public static IEnumerable GetIsEnabled(Selector control)
        {
            return (IEnumerable)control.GetValue(IsEnabledProperty);
        }

        /// <summary>
        /// Set the collection value
        /// </summary>
        /// <param name="control"></param>
        /// <param name="collection"></param>
        public static void SetIsEnabled(Selector control, IEnumerable collection)
        {
            control.SetValue(IsEnabledProperty, collection);
        }

        /// <summary>
        /// True/False whether to go both directions in selection
        /// </summary>
        public static readonly DependencyProperty BindsTwoWayProperty =
            DependencyProperty.Register("BindsTwoWay", typeof(bool), typeof(SynchronizedCollectionBehavior), new PropertyMetadata(true));

        /// <summary>
        /// True/False whether to go both directions in selection
        /// </summary>
        public bool BindsTwoWay
        {
            get { return (bool)GetValue(BindsTwoWayProperty); }
            set { SetValue(BindsTwoWayProperty, value); }
        }

        /// <summary>
        /// This is invoked when the behavior is attached through a property
        /// instead of the full interaction syntax.
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="e"></param>
        private static void OnEnabledChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            Selector selector = dpo as Selector;
            if (selector != null)
            {
                var behaviors = Interaction.GetBehaviors(selector);

                IEnumerable oldCollection = e.OldValue as IEnumerable;
                if (oldCollection != null)
                {
                    var existingBehavior = behaviors.FirstOrDefault(
                        b => b.GetType() == typeof (SynchronizedCollectionBehavior)
                             && ReferenceEquals(((SynchronizedCollectionBehavior) b).Collection, oldCollection));
                    if (existingBehavior != null)
                    {
                        SynchronizedCollectionBehavior scb = (SynchronizedCollectionBehavior) existingBehavior;
                        scb.Collection = null;
                        behaviors.Remove(existingBehavior);
                    }
                }

                IEnumerable newCollection = e.NewValue as IEnumerable;
                if (newCollection != null)
                {
                    SynchronizedCollectionBehavior scb = new SynchronizedCollectionBehavior {Collection = newCollection};
                    behaviors.Add(scb);
                }
            }
        }

        /// <summary>
        /// Retrieve the selected items collection from the associated
        /// object if it supports multiple selection.
        /// </summary>
        /// <returns></returns>
        private IList<object> GetSelectedItemsCollection()
        {
            IList<object> selectedItems = null;

            // Special case ListBox.
            if (AssociatedObject is ListBox && ((ListBox)AssociatedObject).SelectionMode != SelectionMode.Single)
            {
                selectedItems = ((ListBox)AssociatedObject).SelectedItems;
            }
            // Special case ListView/GridView
            else if (AssociatedObject is ListViewBase)
            {
                ListViewBase lvb = (ListViewBase)AssociatedObject;
                if (lvb.SelectionMode != ListViewSelectionMode.None
                    && lvb.SelectionMode != ListViewSelectionMode.Single)
                    selectedItems = lvb.SelectedItems;
            }

            return selectedItems;
        }

        /// <summary>
        /// Called when the SelectedItems property is changed.
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="e"></param>
        private static void OnBackingCollectionChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            ((SynchronizedCollectionBehavior)dpo).OnBackingCollectionChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
        }

        /// <summary>
        /// Called when the SelectedItems property is changed.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        private void OnBackingCollectionChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            // First, unwire from any existing collection.
            if (oldValue != null)
            {
                INotifyCollectionChanged notifyCollection = oldValue as INotifyCollectionChanged;
                if (notifyCollection != null)
                    notifyCollection.CollectionChanged -= OnBackingStoreElementsChanged;
            }

            if (newValue != null)
            {
                if (AssociatedObject != null)
                {
                    SynchronizeBackingStore(true);
                }

                // Wire up collection change events
                if (BindsTwoWay)
                {
                    // See if it supports change notification ala ObservableCollection.
                    INotifyCollectionChanged notifyCollection = newValue as INotifyCollectionChanged;
                    if (notifyCollection != null)
                        notifyCollection.CollectionChanged += OnBackingStoreElementsChanged;
                }
            }
        }

        /// <summary>
        /// This synchronizes the backing collection with the ItemsControl
        /// </summary>
        private void SynchronizeBackingStore(bool selectorToCollection)
        {
            // No collection or no Selector bound?
            if (Collection == null)
                return;
            if (_updatingList)
                return;

            _updatingList = true;

            try
            {
                var selectedItems = GetSelectedItemsCollection();
                if (selectorToCollection)
                {
                    InvokeMethod(Collection, "Clear");

                    if (selectedItems != null)
                    {
                        foreach (var item in selectedItems)
                        {
                            InvokeMethod(Collection, "Add", item);
                        }
                    }
                    else
                    {
                        object item = _associatedObject.SelectedItem;
                        if (item != null)
                            InvokeMethod(Collection, "Add", item);
                    }
                }
                else
                {
                    if (selectedItems != null)
                    {
                        selectedItems.Clear();
                        foreach (var item in Collection)
                        {
                            selectedItems.Add(item);
                        }
                    }
                    else
                    {
                        _associatedObject.SelectedItem = Collection.Cast<object>().FirstOrDefault();
                    }

                }
            }
            finally
            {
                _updatingList = false;
            }

        }

        /// <summary>
        /// Used to add/remove/clear items from unknown collection types
        /// This is done through reflection so we can support IList, IList(OfT), Collection(OfT), etc.
        /// </summary>
        private void InvokeMethod(object obj, string methodName, params object[] parms)
        {
            MethodInfo mi = GetDeclaredMethod(obj, methodName);
            if (mi != null)
            {
                mi.Invoke(obj, parms);
            }
        }

        /// <summary>
        /// Lookup a method name via reflection that also searches
        /// the base class type(s).
        /// </summary>
        private MethodInfo GetDeclaredMethod(object target, string methodName)
        {
            Type currentType = target.GetType();
            while (currentType != typeof(object))
            {
                TypeInfo typeInfo = currentType.GetTypeInfo();
                var methodInfo = typeInfo.GetDeclaredMethod(methodName);
                if (methodInfo != null)
                    return methodInfo;

                currentType = typeInfo.BaseType;
            }

            return null;
        }

        /// <summary>
        /// This is called when the elements inside the backed storage collection change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBackingStoreElementsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Collection bound isn't the same as the sender?
            // .. or we are no longer binding two way?
            if (!ReferenceEquals(sender, Collection) || !BindsTwoWay)
            {
                INotifyCollectionChanged notifyCollection = sender as INotifyCollectionChanged;
                if (notifyCollection != null)
                    notifyCollection.CollectionChanged -= OnBackingStoreElementsChanged;
                return;
            }

            SynchronizeBackingStore(false);
        }

        /// <summary>
        /// Called when the Selection changed event is raised by the associated selector
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SynchronizeBackingStore(true);
        }

        /// <summary>
        /// Attaches to the specified object.
        /// </summary>
        /// <param name="associatedObject">The <see cref="T:Windows.UI.Xaml.DependencyObject"/> to which the <seealso cref="T:Microsoft.Xaml.Interactivity.IBehavior"/> will be attached.</param>
        public void Attach(DependencyObject associatedObject)
        {
            if ((associatedObject != AssociatedObject) && !DesignMode.DesignModeEnabled)
            {
                if (_associatedObject != null)
                    throw new InvalidOperationException("Cannot attach behavior multiple times.");

                _associatedObject = associatedObject as Selector;
                if (_associatedObject == null)
                    throw new InvalidOperationException("Can only attach SynchronizedCollectionBehavior to Selector-base control.");

                _associatedObject.SelectionChanged += OnSelectionChanged;
                if (Collection != null)
                    SynchronizeBackingStore(true);
            }
        }

        /// <summary>
        /// Detaches this instance from its associated object.
        /// </summary>
        public void Detach()
        {
            _associatedObject.SelectionChanged += OnSelectionChanged;
            _associatedObject = null;
        }

        /// <summary>
        /// Gets the <see cref="T:Windows.UI.Xaml.DependencyObject"/> to which the <seealso cref="T:Microsoft.Xaml.Interactivity.IBehavior"/> is attached.
        /// </summary>
        public DependencyObject AssociatedObject { get { return _associatedObject; }}
    }
}
