using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace JulMar.Interactivity
{
    /// <summary>
    /// Behavior to support multi-select in a traditional WPF TreeView control.
    /// </summary>
    /// <example>
    /// <![CDATA[
    ///   <TreeView ...>
    ///      <i:Interaction.Behaviors>
    ///         <b:MultiSelectTreeViewBehavior SelectedItems="{Binding SelectedItems}" />
    ///      </i:Interaction.Behaviors>
    ///   </TreeView>
    /// ]]>
    /// </example>
	public class MultiSelectTreeViewBehavior : Behavior<TreeView>
    {
        private TreeViewItem _anchorItem;

        /// <summary>
        /// Selected Items collection
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(IList), typeof(MultiSelectTreeViewBehavior), new PropertyMetadata(null));

        /// <summary>
        /// Selected Items collection (intended to be data bound)
        /// </summary>
        public IList SelectedItems
        {
            get { return (IList)this.GetValue(SelectedItemsProperty); }
            set { this.SetValue(SelectedItemsProperty, value); }
        }

        /// <summary>
        /// Selection attached property - can be used for styling TreeViewItem elements.
        /// </summary>
	    public static readonly DependencyProperty IsSelectedProperty =
	        DependencyProperty.RegisterAttached("IsSelected", typeof (bool), typeof (MultiSelectTreeViewBehavior), 
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedChanged));

        /// <summary>
        /// Returns whether the TreeViewItem is selected
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetIsSelected(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSelectedProperty);
        }

        /// <summary>
        /// Changes the selection state of the TreeViewItem.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetIsSelected(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSelectedProperty, value);
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            this.AssociatedObject.AddHandler(TreeViewItem.UnselectedEvent, new RoutedEventHandler(this.OnTreeViewItemUnselected), true);
            this.AssociatedObject.AddHandler(TreeViewItem.SelectedEvent, new RoutedEventHandler(this.OnTreeViewItemSelected), true);
            this.AssociatedObject.AddHandler(UIElement.KeyDownEvent, new KeyEventHandler(this.OnKeyDown), true);
            base.OnAttached();
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>
        /// Override this to unhook functionality from the AssociatedObject.
        /// </remarks>
        protected override void OnDetaching()
        {
            this.AssociatedObject.RemoveHandler(UIElement.KeyDownEvent, new KeyEventHandler(this.OnKeyDown));
            this.AssociatedObject.RemoveHandler(TreeViewItem.UnselectedEvent, new RoutedEventHandler(this.OnTreeViewItemUnselected));
            this.AssociatedObject.RemoveHandler(TreeViewItem.SelectedEvent, new RoutedEventHandler(this.OnTreeViewItemSelected));
            base.OnDetaching();
        }

        /// <summary>
        /// This is called when the a tree item is unselected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTreeViewItemUnselected(object sender, RoutedEventArgs e)
        {
            if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == ModifierKeys.None)
            {
                SetIsSelected((TreeViewItem)e.OriginalSource, false);
            }
        }

        /// <summary>
        /// This is called when the tree item is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem) e.OriginalSource;

            // Look for a disconnected item.  We can get this if the data source changes underneath us,
            // in which case we want to ignore this selection. This is actually a bug in WPF4, see:
            // http://connect.microsoft.com/VisualStudio/feedback/details/619658/wpf-virtualized-control-disconnecteditem-reference-when-datacontext-switch
            // Unfortunately, there's no way to reliably see this so we just look for the magic string here.
            // in WPF 4.5 they have a new static property which exposes this object off the BindingExpression.
            //
            // Could also check against this object, but not any safer than the string really.
            //var disconnectedItemSingleton = typeof(System.Windows.Data.BindingExpressionBase).GetField("DisconnectedItem", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null); 
            if (item.DataContext != null && item.DataContext.ToString() == "{DisconnectedItem}")
                return;

            if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) !=
                (ModifierKeys.Shift | ModifierKeys.Control))
            {
                switch ((Keyboard.Modifiers & ModifierKeys.Control))
                {
                    case ModifierKeys.Control:
                        this.ToggleSelect(item);
                        break;
                    default:
                        if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                            this.AnchorMultiSelect(item);
                        else
                            this.SingleSelect(item);
                        break;
                }
            }
        }

        /// <summary>
        /// This method locates the TreeView parent for a given TreeViewItem.
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>Parent</returns>
        static TreeView GetTree(TreeViewItem item)
		{
			FrameworkElement currentItem = item;
            while (!(VisualTreeHelper.GetParent(currentItem) is TreeView))
                currentItem = (FrameworkElement) VisualTreeHelper.GetParent(currentItem);
            
            return (TreeView) VisualTreeHelper.GetParent(currentItem);
		}

        /// <summary>
        /// This method is invoked when the attached selection is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
		static void OnSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			TreeViewItem item = (TreeViewItem)sender;
		    TreeView tree = GetTree(item);
            Debug.Assert(tree != null);

		    var msb = Interaction.GetBehaviors(tree).Single(b => b.GetType() == typeof (MultiSelectTreeViewBehavior)) as MultiSelectTreeViewBehavior;
            if (msb != null)
            {
                if (msb.SelectedItems != null)
                {
                    var isSelected = GetIsSelected(item);
                    if (isSelected)
                        msb.SelectedItems.Add(item.DataContext ?? item);
                    else
                        msb.SelectedItems.Remove(item.DataContext ?? item);
                }
            }
		}

        /// <summary>
        /// This method is invoked when you press a key while the TreeView has focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			TreeView tree = (TreeView)sender;
            Debug.Assert(tree == this.AssociatedObject);

            // If you press CTRL+A, do a full select.
            if (e.Key == Key.A && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                this.GetExpandedTreeViewItems(tree)
                    .ToList()
                    .ForEach(tvi => SetIsSelected(tvi, true));

                e.Handled = true;
			}
		}

        /// <summary>
        /// Returns the entire TreeView set of nodes.  Unfortunately, in WPF the TreeView
        /// doesn't manage a selection state globally - it's singular, and compartmentalized into
        /// each ItemsControl expansion.  This is a heavy-handed approach, but for reasonably sized
        /// tree views it should be ok.
        /// </summary>
        /// <returns></returns>
		private IEnumerable<TreeViewItem> GetExpandedTreeViewItems(ItemsControl container = null)
		{
            if (container == null)
                container = this.AssociatedObject;

			for (int i = 0; i < container.Items.Count; i++)
			{
				var item = container.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem;
				if (item == null)
					continue;
				
                // Hand back this child
                yield return item;

                // Hand back all the children
                foreach (var subItem in this.GetExpandedTreeViewItems(item))
                    yield return subItem;
			}
		}

        /// <summary>
        /// This is used to perform a multi-select operation using an anchor position.
        /// </summary>
        /// <param name="newItem"></param>
		private void AnchorMultiSelect(TreeViewItem newItem)
		{
			if (this._anchorItem == null)
			{
				var selectedItems = this.GetExpandedTreeViewItems().Where(GetIsSelected).ToList();
			    this._anchorItem = (selectedItems.Count > 0
			                      ? selectedItems[selectedItems.Count - 1]
			                      : this.GetExpandedTreeViewItems().FirstOrDefault());
			    if (this._anchorItem == null)
					return;
			}

		    var anchor = this._anchorItem;
			var items = this.GetExpandedTreeViewItems();
			bool inSelectionRange = false;

            foreach (var item in items)
			{
				bool isEdge = item == anchor || item == newItem;
                if (isEdge)
					inSelectionRange = !inSelectionRange;
				
                SetIsSelected(item, (inSelectionRange || isEdge));
			}
		}

        /// <summary>
        /// This performs a single-select operation
        /// </summary>
        /// <param name="item"></param>
		private void SingleSelect(TreeViewItem item)
		{
			foreach (TreeViewItem selectedItem in this.GetExpandedTreeViewItems().Where(ti => ti != null))
    			SetIsSelected(selectedItem, selectedItem == item);

		    this._anchorItem = item;
		}

        /// <summary>
        /// This toggles the selection
        /// </summary>
        /// <param name="item"></param>
		private void ToggleSelect(TreeViewItem item)
		{
			SetIsSelected(item, !GetIsSelected(item));
            if (this._anchorItem == null)
		        this._anchorItem = item;
		}
	}
}
