using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Interactivity;
using JulMar.Windows.Internal;

namespace JulMar.Windows.Interactivity
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
            _isMouseDown = false;
            _isDragging = false;
            _dragScrollWaitCounter = DragWaitCounterThreshold;
            ItemTypeKey = GetType().ToString(); 
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            AssociatedObject.AllowDrop = true;
            AssociatedObject.PreviewMouseLeftButtonDown += PreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseMove += PreviewMouseMove;
            AssociatedObject.PreviewMouseLeftButtonUp += PreviewMouseLeftButtonUp;
            AssociatedObject.PreviewDrop += PreviewDrop;
            AssociatedObject.PreviewQueryContinueDrag += PreviewQueryContinueDrag;
            AssociatedObject.PreviewDragEnter += PreviewDragEnter;
            AssociatedObject.PreviewDragOver += PreviewDragOver;
            AssociatedObject.DragLeave += DragLeave;
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
            _data = DragUtilities.GetDataObjectFromItemsControl(itemsControl, p);
            if (_data != null)
            {
                _isMouseDown = true;
                _dragStartPosition = p;
            }
        }

        /// <summary>
        /// Display the drag indicator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                var itemsControl = (ItemsControl)sender;
                Point currentPosition = e.GetPosition(itemsControl);
                if ((_isDragging == false) && (Math.Abs(currentPosition.X - _dragStartPosition.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                    (Math.Abs(currentPosition.Y - _dragStartPosition.Y) > SystemParameters.MinimumVerticalDragDistance))
                {
                    DragStarted(itemsControl);
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
            ResetState();
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
            if (e.Data.GetDataPresent(ItemTypeKey))
            {
                DragInfo data = e.Data.GetData(ItemTypeKey) as DragInfo;
                if (data != null)
                {
                    if (!data.AllowOnlySelf || itemsControl == data.Source)
                    {
                        var allowedEffects = e.AllowedEffects;
                        if (DropEnter != null)
                        {
                            DragDropEventArgs de = new DragDropEventArgs(data.Source, itemsControl, data.Data) {AllowedEffects = allowedEffects};
                            DropEnter(this, de);
                            allowedEffects = de.AllowedEffects;
                            if (de.Cancel)
                                return;
                        }

                        InitializeDragAdorner(itemsControl, data.Data, e.GetPosition(itemsControl));
                        InitializeInsertAdorner(itemsControl, e);
                        e.Effects = GetDropEffectType(allowedEffects, e.KeyStates);
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
            if (e.Data.GetDataPresent(ItemTypeKey))
            {
                DragInfo data = e.Data.GetData(ItemTypeKey) as DragInfo;
                if (data != null)
                {
                    if (!data.AllowOnlySelf || itemsControl == data.Source)
                    {
                        var allowedEffects = e.AllowedEffects;
                        if (DropEnter != null)
                        {
                            DragDropEventArgs de = new DragDropEventArgs(data.Source, itemsControl, data.Data) { AllowedEffects = allowedEffects };
                            DropEnter(this, de);
                            allowedEffects = de.AllowedEffects;
                            if (de.Cancel)
                                return;
                        }

                        UpdateDragAdorner(e.GetPosition(itemsControl));
                        UpdateInsertAdorner(itemsControl, e);
                        HandleDragScrolling(itemsControl, e);
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
            DestroyAdorners();
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
                ResetState();
                DestroyAdorners();
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
            DestroyAdorners();
            e.Handled = true;
            if (e.Data.GetDataPresent(ItemTypeKey))
            {
                DragInfo data = e.Data.GetData(ItemTypeKey) as DragInfo;
                if (data != null && (!data.AllowOnlySelf || itemsControl == data.Source))
                {
                    var allowedEffects = e.AllowedEffects;
                    if (DropInitiated != null)
                    {
                        DragDropEventArgs de = new DragDropEventArgs(data.Source, itemsControl, data.Data) { AllowedEffects = allowedEffects };
                        DropInitiated(this, de);
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
                    }

                    int index = FindInsertionIndex(itemsControl, e);
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
            if (DragInitiated != null)
            {
                var dde = new DragDropEventArgs(itemsControl, null, _data) { AllowedEffects = allowedEffects };
                DragInitiated(this, dde);
                if (dde.Cancel)
                {
                    ResetState();
                    return;
                }
                allowedEffects = dde.AllowedEffects;
            }

            UIElement draggedItemContainer = DragUtilities.GetItemContainerFromPoint(itemsControl, _dragStartPosition);
            _isDragging = true;
            
            DataObject dObject = new DataObject(ItemTypeKey, new DragInfo { Data = _data, Source = itemsControl, AllowOnlySelf = AllowOnlySelf } );
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
                    DragUtilities.RemoveItem(itemsControl, _data);
                }
            }

            ResetState();
        }

        /// <summary>
        /// Returns the drop effects allowed
        /// </summary>
        /// <param name="allowedEffects"></param>
        /// <param name="keyStates"></param>
        /// <returns></returns>
        static DragDropEffects GetDropEffectType(DragDropEffects allowedEffects, DragDropKeyStates keyStates)
        {
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
                if (_dragScrollWaitCounter == DragWaitCounterThreshold)
                {
                    _dragScrollWaitCounter = 0;

                    ScrollViewer scrollViewer = DragUtilities.FindScrollViewer(itemsControl);
                    if (scrollViewer != null && scrollViewer.CanContentScroll
                        && scrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible)
                    {
                        scrollViewer.ScrollToVerticalOffset((isMouseAtTop.Value) ? scrollViewer.VerticalOffset - 1.0 : scrollViewer.VerticalOffset + 1.0);
                        e.Effects = DragDropEffects.Scroll;
                    }
                }
                else
                    _dragScrollWaitCounter++;
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
            _isMouseDown = false;
            _isDragging = false;
            _data = null;
            _dragScrollWaitCounter = DragWaitCounterThreshold;
        }

        /// <summary>
        /// Initialize the drag adorner -- this is the DataTemplate instantiation of the item
        /// </summary>
        /// <param name="itemsControl"></param>
        /// <param name="dragData"></param>
        /// <param name="startPosition"></param>
        private void InitializeDragAdorner(ItemsControl itemsControl, object dragData, Point startPosition)
        {
            if (DragTemplate != null)
            {
                if (_dragAdorner == null)
                {
                    var adornerLayer = AdornerLayer.GetAdornerLayer(itemsControl);
                    _dragAdorner = new DragAdorner(dragData, DragTemplate, itemsControl, adornerLayer);
                    UpdateDragAdorner(startPosition);
                }
            }
        }

        /// <summary>
        /// Update the position of the drag adorner
        /// </summary>
        /// <param name="currentPosition"></param>
        private void UpdateDragAdorner(Point currentPosition)
        {
            if (_dragAdorner != null)
                _dragAdorner.SetPosition(currentPosition.X + SystemParameters.CursorWidth, currentPosition.Y);
        }

        /// <summary>
        /// Initialize the insertion point marker
        /// </summary>
        /// <param name="itemsControl"></param>
        /// <param name="e"></param>
        private void InitializeInsertAdorner(ItemsControl itemsControl, DragEventArgs e)
        {
            if (_insertAdorner == null)
            {
                UIElement itemContainer = DragUtilities.GetItemContainerFromPoint(itemsControl, e.GetPosition(itemsControl));
                if (itemContainer != null)
                {
                    _insertAdorner = new InsertAdorner(DragUtilities.IsPointInTopHalf(itemsControl, e), 
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
            if (_insertAdorner != null)
            {
                _insertAdorner.InTopHalf = DragUtilities.IsPointInTopHalf(itemsControl, e);
                _insertAdorner.InvalidateVisual();
            }
        }

        /// <summary>
        /// Remove all the adorners
        /// </summary>
        private void DestroyAdorners()
        {
            if (_insertAdorner != null)
            {
                _insertAdorner.Dispose();
                _insertAdorner = null;
            }

            if (_dragAdorner != null)
            {
                _dragAdorner.Dispose();
                _dragAdorner = null;
            }
        }
    }
}
