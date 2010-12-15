using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace JulMar.Windows.Interactivity
{
    /// <summary>
    /// This behavior synchronizes a collection with the Multiple Selection of a ListBox or MultiSelector.
    /// </summary>
    public class SelectedItemsCollectionSynchronizer : Behavior<Selector>
    {
        /// <summary>
        /// Dependency Property to manage collection
        /// </summary>
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(IList), typeof(SelectedItemsCollectionSynchronizer));

        /// <summary>
        /// Collection to synchronize
        /// </summary>
        public IList Items
        {
            get { return (IList) base.GetValue(ItemsProperty); }
            set { base.SetValue(ItemsProperty, value); }
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            if (AssociatedObject is MultiSelector)
            {
                MultiSelector ms = (MultiSelector) AssociatedObject;
                ms.SelectionChanged += SelectionChanged;
            }
            else if (AssociatedObject is ListBox)
            {
                ListBox lb = (ListBox) AssociatedObject;
                lb.SelectionChanged += SelectionChanged;
            }

            base.OnAttached();
        }

        /// <summary>
        /// Processes the selection change event from the ListBox/MultiSelector.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Items != null)
            {
                foreach (var ob in e.RemovedItems)
                    Items.Remove(ob);
                foreach (var ob in e.AddedItems)
                    Items.Add(ob);
            }
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>
        /// Override this to unhook functionality from the AssociatedObject.
        /// </remarks>
        protected override void OnDetaching()
        {
            if (AssociatedObject is MultiSelector)
            {
                MultiSelector ms = (MultiSelector)AssociatedObject;
                ms.SelectionChanged -= SelectionChanged;
            }
            else if (AssociatedObject is ListBox)
            {
                ListBox lb = (ListBox)AssociatedObject;
                lb.SelectionChanged -= SelectionChanged;
            }
            base.OnDetaching();
        }
    }
}
