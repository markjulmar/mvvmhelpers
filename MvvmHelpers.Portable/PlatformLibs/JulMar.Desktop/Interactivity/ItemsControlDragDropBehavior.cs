using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;

using JulMar.Interactivity.Internal;
using JulMar.Windows;

namespace JulMar.Interactivity
{
    /// <summary>
    /// ItemsControlDragDropBehavior can be used to add automatic Drag/Drop support
    /// to any ItemsControl based element.
    /// </summary>
    /// <remarks>
    /// This was originally taken from a sample posted by Bea Stollnitz
    /// See http://www.beacosta.com/blog/?p=53 for the original article. 
    /// I have also borrowed elements from http://code.google.com/p/gong-wpf-dragdrop/
    /// which was an extension of the above codebase. 
    /// </remarks>
    public class ItemsControlDragDropBehavior : Behavior<ItemsControl>
    {
        #region Private Data
        private bool _isMouseDown;
        private object _data;
        private Point _dragStartPosition;
        private bool _isDragging;
        private DragAdorner _dragAdorner;
        private InsertAdorner _insertAdorner;
        private int _dragScrollWaitCounter;
        private const int DragWaitCounterThreshold = 10;
        #endregion

        /// <summary>
        /// Key used for drag/drop operations
        /// </summary>
        public string ItemTypeKey { get; set; }

        /// <summary>
        /// Data template to represent drag items
        /// </summary>
        public DataTemplate DragTemplate { get; set; }

        /// <summary>
        /// True to only allow "Self" as drop target
        /// </summary>
        public bool AllowOnlySelf { get; set; }

        /// <summary>
        /// False to not allow drops on source.
        /// </summary>
        public bool AllowSelf { get; set; }

        /// <summary>
        /// True to show the insertion marker
        /// </summary>
        public bool ShowInsertAdorner { get; set; }

        /// <summary>
        /// This event is raised when a drag/drop operation starts
        /// </summary>
        public event EventHandler<DragDropEventArgs> DragInitiated;

        /// <summary>
        /// This event is raised when a target is identified
        /// </summary>
        public event EventHandler<DragDropEventArgs> DropEnter;
        
        /// <summary>
        /// This event is raised when a drop is initiated
        /// </summary>
        public event EventHandler<DragDropEventArgs> DropInitiated;

        /// <summary>
        /// Constructor
        /// </summary>
        public ItemsControlDragDropBehavior()
        {
            this._isMouseDown = false;
            this._isDragging = false;
            this._dragScrollWaitCounter = DragWaitCounterThreshold;
            this.ItemTypeKey = this.GetType().ToString();
            this.AllowSelf = true;
            this.ShowInsertAdorner = true;
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            this.AssociatedObject.AllowDrop = true;
            this.AssociatedObject.PreviewMouseLeftButtonDown += this.PreviewMouseLeftButtonDown;
            this.AssociatedObject.PreviewMouseMove += this.PreviewMouseMove;
            this.AssociatedObject.PreviewMouseLeftButtonUp += this.PreviewMouseLeftButtonUp;
            this.AssociatedObject.PreviewDrop += this.PreviewDrop;
            this.AssociatedObject.PreviewQueryContinueDrag += this.PreviewQueryContinueDrag;
            this.AssociatedObject.PreviewDragEnter += this.PreviewDragEnter;
            this.AssociatedObject.PreviewDragOver += this.PreviewDragOver;
            this.AssociatedObject.DragLeave += this.DragLeave;
        }

        /// <summary>
        /// Starts the drag/drop operation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var itemsControl = (ItemsControl)sender;
            Point p = e.GetPosition(itemsControl);
            this._data = DragUtilities.GetDataObjectFromItemsControl(itemsControl, p);
            if (this._data != null)
            {
                this._isMouseDown = true;
                this._dragStartPosition = p;
            }
        }

        /// <summary>
        /// Display the drag indicator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (this._isMouseDown)
            {
                var itemsControl = (ItemsControl)sender;
                Point currentPosition = e.GetPosition(itemsControl);
                if ((this._isDragging == false) && (Math.Abs(currentPosition.X - this._dragStartPosition.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                    (Math.Abs(currentPosition.Y - this._dragStartPosition.Y) > SystemParameters.MinimumVerticalDragDistance))
                {
                    // Do not allow if sorted and we only allow this element as
                    // the drop target.
                    if (!DragUtilities.CanReorderCollectionView(itemsControl)
                        && this.AllowOnlySelf)
                        return;

                    this.DragStarted(itemsControl);
                }
            }
        }

        /// <summary>
        /// Stops the drag/drop operation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.ResetState();
        }

        /// <summary>
        /// New drop target identified
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PreviewDragEnter(object sender, DragEventArgs e)
        {
            e.Handled = true;
            e.Effects = DragDropEffects.None;

            var itemsControl = (ItemsControl)sender;
            
            // Do not allow if sorted.
            if (!DragUtilities.CanReorderCollectionView(itemsControl))
                return;

            if (e.Data.GetDataPresent(this.ItemTypeKey))
            {
                DragInfo data = e.Data.GetData(this.ItemTypeKey) as DragInfo;
                if (data != null)
                {
                    if (!data.AllowOnlySelf || itemsControl == data.Source)
                    {
                        var allowedEffects = e.AllowedEffects;

                        if (itemsControl == data.Source && !this.AllowSelf)
                            allowedEffects = DragDropEffects.None;
                        else if (this.DropEnter != null)
                        {
                            DragDropEventArgs de = new DragDropEventArgs(data.Source, itemsControl, data.Data) {AllowedEffects = allowedEffects};
                            this.DropEnter(this, de);
                            allowedEffects = de.AllowedEffects;
                            if (de.Cancel)
                                return;
                        }

                        this.InitializeDragAdorner(itemsControl, data.Data, e.GetPosition(itemsControl));
                        e.Effects = GetDropEffectType(allowedEffects, e.KeyStates);

                        if (e.Effects != DragDropEffects.None)
                            this.InitializeInsertAdorner(itemsControl, e);
                    }
                }
            }
        }


        /// <summary>
        /// New drop target being dragged over
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
            e.Effects = DragDropEffects.None;

            var itemsControl = (ItemsControl)sender;
            
            // Do not allow if sorted.
            if (!DragUtilities.CanReorderCollectionView(itemsControl))
                return;

            if (e.Data.GetDataPresent(this.ItemTypeKey))
            {
                DragInfo data = e.Data.GetData(this.ItemTypeKey) as DragInfo;
                if (data != null)
                {
                    if (!data.AllowOnlySelf || itemsControl == data.Source)
                    {
                        var allowedEffects = e.AllowedEffects;
                        
                        if (itemsControl == data.Source && !this.AllowSelf)
                            allowedEffects = DragDropEffects.None;
                        else if (this.DropEnter != null)
                        {
                            DragDropEventArgs de = new DragDropEventArgs(data.Source, itemsControl, data.Data) { AllowedEffects = allowedEffects };
                            this.DropEnter(this, de);
                            allowedEffects = de.AllowedEffects;
                            if (de.Cancel)
                                return;
                        }

                        this.UpdateDragAdorner(e.GetPosition(itemsControl));
                        this.UpdateInsertAdorner(itemsControl, e);
                        this.HandleDragScrolling(itemsControl, e);
                        e.Effects = GetDropEffectType(allowedEffects, e.KeyStates);
                    }
                }
            }
        }

        /// <summary>
        /// Drop target has left control airspace
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DragLeave(object sender, DragEventArgs e)
        {
            this.DestroyAdorners();
            e.Handled = true;
        }

        /// <summary>
        /// Query to continue dragging operation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PreviewQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            // If the user presses ESC then cancel the operation
            if (e.EscapePressed)
            {
                e.Action = DragAction.Cancel;
                this.ResetState();
                this.DestroyAdorners();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Drop started
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PreviewDrop(object sender, DragEventArgs e)
        {
            var itemsControl = (ItemsControl)sender;
            this.DestroyAdorners();
            e.Handled = true;
            if (e.Data.GetDataPresent(this.ItemTypeKey))
            {
                int index = FindInsertionIndex(itemsControl, e);

                DragInfo data = e.Data.GetData(this.ItemTypeKey) as DragInfo;
                if (data != null && (!data.AllowOnlySelf || itemsControl == data.Source))
                {
                    var allowedEffects = e.AllowedEffects;
                    if (this.DropInitiated != null)
                    {
                        DragDropEventArgs de = new DragDropEventArgs(data.Source, itemsControl, data.Data, index) { AllowedEffects = allowedEffects };
                        this.DropInitiated(this, de);
                        allowedEffects = de.AllowedEffects;
                        if (de.Cancel)
                        {
                            e.Effects = DragDropEffects.None;
                            return;
                        }
                    }

                    object itemToAdd = data.Data;
                    e.Effects = GetDropEffectType(allowedEffects, e.KeyStates);

                    if (DragUtilities.DoesItemExists(itemsControl, itemToAdd)
                        && (e.Effects == DragDropEffects.Move
                             || (e.Effects & DragDropEffects.Move) > 0 && (e.KeyStates & DragDropKeyStates.ControlKey) > 0))
                    {
                        DragUtilities.RemoveItem(itemsControl, itemToAdd);
                        // Recalc our position based on the removal
                        index = FindInsertionIndex(itemsControl, e);
                    }

                    DragUtilities.AddItem(itemsControl, itemToAdd, index);
                    DragUtilities.SelectItem(itemsControl, index);
                }
                else
                    e.Effects = DragDropEffects.None;
            }
            else
                e.Effects = DragDropEffects.None;
        }

        /// <summary>
        /// This starts the drag operation from the given ItemsControl
        /// </summary>
        /// <param name="itemsControl"></param>
        private void DragStarted(ItemsControl itemsControl)
        {
            DragDropEffects allowedEffects = DragDropEffects.Copy | DragDropEffects.Move;
            if (this.DragInitiated != null)
            {
                var dde = new DragDropEventArgs(itemsControl, null, this._data) { AllowedEffects = allowedEffects };
                this.DragInitiated(this, dde);
                if (dde.Cancel)
                {
                    this.ResetState();
                    return;
                }
                allowedEffects = dde.AllowedEffects;
            }

            UIElement draggedItemContainer = DragUtilities.GetItemContainerFromPoint(itemsControl, this._dragStartPosition);
            this._isDragging = true;
            
            DataObject dObject = new DataObject(this.ItemTypeKey, new DragInfo { Data = this._data, Source = itemsControl, AllowOnlySelf = this.AllowOnlySelf } );
            DragDropEffects e = DragDrop.DoDragDrop(itemsControl, dObject, allowedEffects);
            
            if ((e & DragDropEffects.Move) != 0)
            {
                if (draggedItemContainer != null)
                {
                    int dragItemIndex = itemsControl.ItemContainerGenerator.IndexFromContainer(draggedItemContainer);
                    DragUtilities.RemoveItem(itemsControl, dragItemIndex);
                }
                else
                {
                    DragUtilities.RemoveItem(itemsControl, this._data);
                }
            }

            this.ResetState();
        }

        /// <summary>
        /// Returns the drop effects allowed
        /// </summary>
        /// <param name="allowedEffects"></param>
        /// <param name="keyStates"></param>
        /// <returns></returns>
        static DragDropEffects GetDropEffectType(DragDropEffects allowedEffects, DragDropKeyStates keyStates)
        {
            // None?
            if (allowedEffects == DragDropEffects.None)
                return allowedEffects;

            return (allowedEffects & (DragDropEffects.Move | DragDropEffects.Copy)) ==
                   (DragDropEffects.Move | DragDropEffects.Copy)
                       ? (((keyStates & DragDropKeyStates.ControlKey) != 0)
                              ? DragDropEffects.Copy
                              : DragDropEffects.Move)
                       : ((allowedEffects & DragDropEffects.Copy) != 0 ? DragDropEffects.Copy : DragDropEffects.Move);
        }

        /// <summary>
        /// This scrolls the items control when we hit a boundary
        /// </summary>
        /// <param name="itemsControl"></param>
        /// <param name="e"></param>
        private void HandleDragScrolling(ItemsControl itemsControl, DragEventArgs e)
        {
            bool? isMouseAtTop = DragUtilities.IsMousePointerAtTop(itemsControl, e.GetPosition(itemsControl));
            if (isMouseAtTop.HasValue)
            {
                if (this._dragScrollWaitCounter == DragWaitCounterThreshold)
                {
                    this._dragScrollWaitCounter = 0;

                    ScrollViewer scrollViewer = DragUtilities.FindScrollViewer(itemsControl);
                    if (scrollViewer != null && scrollViewer.CanContentScroll
                        && scrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible)
                    {
                        scrollViewer.ScrollToVerticalOffset((isMouseAtTop.Value) ? scrollViewer.VerticalOffset - 1.0 : scrollViewer.VerticalOffset + 1.0);
                        e.Effects = DragDropEffects.Scroll;
                    }
                }
                else
                    this._dragScrollWaitCounter++;
            }
            else
            {
                e.Effects = GetDropEffectType(e.AllowedEffects, e.KeyStates);
            }
        }

        /// <summary>
        /// Determine the proper insertion index
        /// </summary>
        /// <param name="itemsControl"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private static int FindInsertionIndex(ItemsControl itemsControl, DragEventArgs e)
        {
            UIElement dropTargetContainer = DragUtilities.GetItemContainerFromPoint(itemsControl, e.GetPosition(itemsControl));
            if (dropTargetContainer != null)
            {
                int index = itemsControl.ItemContainerGenerator.IndexFromContainer(dropTargetContainer);
                return DragUtilities.IsPointInTopHalf(itemsControl, e) ? index : index + 1;
            }
            return itemsControl.Items.Count;
        }

        /// <summary>
        /// Resets the drag/drop operation
        /// </summary>
        private void ResetState()
        {
            this._isMouseDown = false;
            this._isDragging = false;
            this._data = null;
            this._dragScrollWaitCounter = DragWaitCounterThreshold;
        }

        /// <summary>
        /// Initialize the drag adorner -- this is the DataTemplate instantiation of the item
        /// </summary>
        /// <param name="itemsControl"></param>
        /// <param name="dragData"></param>
        /// <param name="startPosition"></param>
        private void InitializeDragAdorner(ItemsControl itemsControl, object dragData, Point startPosition)
        {
            if (this.DragTemplate != null)
            {
                if (this._dragAdorner == null)
                {
                    var adornerLayer = AdornerLayer.GetAdornerLayer(itemsControl);
                    this._dragAdorner = new DragAdorner(dragData, this.DragTemplate, itemsControl, adornerLayer);
                    this.UpdateDragAdorner(startPosition);
                }
            }
        }

        /// <summary>
        /// Update the position of the drag adorner
        /// </summary>
        /// <param name="currentPosition"></param>
        private void UpdateDragAdorner(Point currentPosition)
        {
            if (this._dragAdorner != null)
                this._dragAdorner.SetPosition(currentPosition.X, currentPosition.Y);
        }

        /// <summary>
        /// Initialize the insertion point marker
        /// </summary>
        /// <param name="itemsControl"></param>
        /// <param name="e"></param>
        private void InitializeInsertAdorner(ItemsControl itemsControl, DragEventArgs e)
        {
            if (this._insertAdorner == null && this.ShowInsertAdorner)
            {
                UIElement itemContainer = DragUtilities.GetItemContainerFromPoint(itemsControl, e.GetPosition(itemsControl));
                if (itemContainer != null)
                {
                    this._insertAdorner = new InsertAdorner(DragUtilities.IsPointInTopHalf(itemsControl, e), 
                        DragUtilities.IsItemControlOrientationHorizontal(itemsControl),
                        itemContainer, AdornerLayer.GetAdornerLayer(itemsControl), itemsControl);
                }
            }
        }

        /// <summary>
        /// Update the position of the insertion point marker
        /// </summary>
        /// <param name="itemsControl"></param>
        /// <param name="e"></param>
        private void UpdateInsertAdorner(ItemsControl itemsControl, DragEventArgs e)
        {
            if (this._insertAdorner != null)
            {
                this._insertAdorner.InTopHalf = DragUtilities.IsPointInTopHalf(itemsControl, e);
                this._insertAdorner.InvalidateVisual();
            }
        }

        /// <summary>
        /// Remove all the adorners
        /// </summary>
        private void DestroyAdorners()
        {
            if (this._insertAdorner != null)
            {
                this._insertAdorner.Dispose();
                this._insertAdorner = null;
            }

            if (this._dragAdorner != null)
            {
                this._dragAdorner.Dispose();
                this._dragAdorner = null;
            }
        }
    }
}
