using System;
using System.Windows.Controls;
using System.Collections;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace JulMar.Windows.Internal
{
    /// <summary>
    /// Drag Utilities - this was taken from a sample posted by Bea Stollnitz
    /// See http://www.beacosta.com/blog/?p=53 for the original article.
    /// </summary>
    static class DragUtilities
    {
        public static bool DoesItemExists(ItemsControl itemsControl, object item)
        {
            return itemsControl.Items.Count > 0 && itemsControl.Items.Contains(item);
        }

        public static void AddItem(ItemsControl itemsControl, object item, int insertIndex)
        {
            if (itemsControl.ItemsSource != null)
            {
                IList iList = itemsControl.ItemsSource as IList;
                if (iList != null)
                {
                    iList.Insert(insertIndex, item);
                }
                else
                {
                    Type type = itemsControl.ItemsSource.GetType();
                    Type genericList = type.GetInterface("IList`1");
                    if (genericList != null)
                    {
                        type.GetMethod("Insert").Invoke(itemsControl.ItemsSource, new object[] { insertIndex, item });
                    }
                }
            }
            else
            {
                itemsControl.Items.Insert(insertIndex, item);
            }
        }

        public static void RemoveItem(ItemsControl itemsControl, object itemToRemove)
        {
            if (itemToRemove != null)
            {
                int index = itemsControl.Items.IndexOf(itemToRemove);
                if (index != -1)
                {
                    RemoveItem(itemsControl, index);
                }
            }
        }

        public static void RemoveItem(ItemsControl itemsControl, int removeIndex)
        {
            if (removeIndex != -1 && removeIndex < itemsControl.Items.Count)
            {
                if (itemsControl.ItemsSource != null)
                {
                    IList iList = itemsControl.ItemsSource as IList;
                    if (iList != null)
                    {
                        iList.RemoveAt(removeIndex);
                    }
                    else
                    {
                        Type type = itemsControl.ItemsSource.GetType();
                        Type genericList = type.GetInterface("IList`1");
                        if (genericList != null)
                        {
                            type.GetMethod("RemoveAt").Invoke(itemsControl.ItemsSource, new object[] { removeIndex });
                        }
                    }
                }
                else
                {
                    itemsControl.Items.RemoveAt(removeIndex);
                }
            }
        }

        public static object GetDataObjectFromItemsControl(ItemsControl itemsControl, Point p)
        {
            UIElement element = itemsControl.InputHitTest(p) as UIElement;
            while (element != null)
            {
                if (element == itemsControl)
                    return null;

                object data = itemsControl.ItemContainerGenerator.ItemFromContainer(element);
                if (data != DependencyProperty.UnsetValue)
                    return data;

                element = VisualTreeHelper.GetParent(element) as UIElement;
            }
            return null;
        }

        public static UIElement GetItemContainerFromPoint(ItemsControl itemsControl, Point p)
        {
            UIElement element = itemsControl.InputHitTest(p) as UIElement;
            while (element != null)
            {
                if (element == itemsControl)
                    return null;

                object data = itemsControl.ItemContainerGenerator.ItemFromContainer(element);
                if (data != DependencyProperty.UnsetValue)
                    return element;

                element = VisualTreeHelper.GetParent(element) as UIElement;
            }
            return null;
        }

        public static UIElement GetItemContainerFromItemsControl(ItemsControl itemsControl)
        {
            UIElement container = null;
            if (itemsControl != null && itemsControl.Items.Count > 0)
            {
                container = itemsControl.ItemContainerGenerator.ContainerFromIndex(0) as UIElement;
            }
            else
            {
                container = itemsControl;
            }
            return container;
        }

        public static bool IsPointInTopHalf(ItemsControl itemsControl, DragEventArgs e)
        {
            UIElement selectedItemContainer = GetItemContainerFromPoint(itemsControl, e.GetPosition(itemsControl));
            Point relativePosition = e.GetPosition(selectedItemContainer);
            if (IsItemControlOrientationHorizontal(itemsControl))
            {
                return relativePosition.Y < ((FrameworkElement)selectedItemContainer).ActualHeight / 2;
            }
            return relativePosition.X < ((FrameworkElement)selectedItemContainer).ActualWidth / 2;
        }

        public static bool IsItemControlOrientationHorizontal(ItemsControl itemsControl)
        {
            return !(itemsControl is TabControl);
        }

        public static bool? IsMousePointerAtTop(ItemsControl itemsControl, Point pt)
        {
            return pt.Y > 0.0 && pt.Y < 25
                ? true
                : (pt.Y > itemsControl.ActualHeight - 25 && pt.Y < itemsControl.ActualHeight
                        ? (bool?) false
                        : null);
        }

        public static ScrollViewer FindScrollViewer(ItemsControl itemsControl)
        {
            UIElement ele = itemsControl;
            while (ele != null)
            {
                if (VisualTreeHelper.GetChildrenCount(ele) == 0)
                {
                    ele = null;
                }
                else
                {
                    ele = VisualTreeHelper.GetChild(ele, 0) as UIElement;
                    if (ele != null && ele is ScrollViewer)
                        return ele as ScrollViewer;
                }
            }
            return null;
        }


        public static double ScrollOffsetUp(double verticaloffset, double offset)
        {
            return verticaloffset - offset < 0.0 ? 0.0 : verticaloffset - offset;
        }

        public static void SelectItem(ItemsControl itemsControl, int index)
        {
            Selector selector = itemsControl as Selector;
            if (selector != null)
            {
                selector.SelectedIndex = index;
            }
        }
    }
}
