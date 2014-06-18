using System;
using System.Collections.Generic;
using System.Reflection;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace JulMar.Extensions
{
    /// <summary>
    /// Dependency Object extensions
    /// </summary>
    public static class DependencyObjectExtensions
    {
        /// <summary>
        /// Searches the sub-tree of an element (including that element) 
        /// for an element of a particular type.
        /// </summary>
        public static T FindElement<T>(this DependencyObject element) where T : FrameworkElement
        {
            T correctlyTyped = element as T;
            if (correctlyTyped != null)
            {
                return correctlyTyped;
            }

            if (element != null)
            {
                int numChildren = VisualTreeHelper.GetChildrenCount(element);
                for (int i = 0; i < numChildren; i++)
                {
                    T child = FindElement<T>(VisualTreeHelper.GetChild(element, i) as FrameworkElement);
                    if (child != null)
                    {
                        return child;
                    }
                }

                // Popups continue in another window, jump to that tree
                Popup popup = element as Popup;
                if (popup != null)
                {
                    return FindElement<T>(popup.Child as FrameworkElement);
                }
            }

            return null;
        }

        /// <summary>
        /// This method locates the first visual parent of the given Type.
        /// </summary>
        /// <param name="fe">Framework Element</param>
        /// <param name="lookForType">Specific type to look for</param>
        /// <returns>Visual Parent</returns>
        public static DependencyObject FindVisualParent(this DependencyObject fe, Type lookForType)
        {
            fe = VisualTreeHelper.GetParent(fe);
            while (fe != null)
            {
                Type feType = fe.GetType();
                if (feType.GetTypeInfo().IsSubclassOf(lookForType))
                    return fe;

                fe = VisualTreeHelper.GetParent(fe);
            }
            return null;
        }

        /// <summary>
        /// This method locates the first visual parent of the given Type.
        /// </summary>
        /// <typeparam name="T">Type to search for</typeparam>
        /// <param name="fe">Framework Element</param>
        /// <returns>Visual Parent</returns>
        public static T FindVisualParent<T>(this DependencyObject fe) where T : DependencyObject
        {
            fe = VisualTreeHelper.GetParent(fe);
            while (fe != null)
            {
                var correctlyTyped = fe as T;
                if (correctlyTyped != null)
                    return correctlyTyped;
                fe = VisualTreeHelper.GetParent(fe);
            }
            return null;
        }

        /// <summary>
        /// This method locates the first visual child of the given Type.
        /// </summary>
        /// <typeparam name="T">Type to search for</typeparam>
        /// <param name="fe">Framework Element</param>
        /// <returns>Visual Child</returns>
        public static T FindVisualChild<T>(this DependencyObject fe) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(fe); i++)
            {
                DependencyObject dpo = VisualTreeHelper.GetChild(fe, i);
                if (dpo == null)
                    break;

                var tChild = dpo as T;
                if (tChild != null)
                    return tChild;

                var tdp = FindVisualChild<T>(dpo);
                if (tdp != null)
                    return tdp;
            }

            return null;
        }

        /// <summary>
        /// This method locates a visual child by NAME of the given Type.
        /// </summary>
        /// <typeparam name="T">Type to search for</typeparam>
        /// <param name="fe">Framework Element</param>
        /// <param name="name">Name to look for (assigned to Name property)</param>
        /// <returns>Visual Child or null</returns>
        public static T FindVisualChildByName<T>(this DependencyObject fe, string name) where T : DependencyObject
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(fe); i++)
            {
                var child = VisualTreeHelper.GetChild(fe, i);
                var controlName = child.GetValue(FrameworkElement.NameProperty) as string;

                if (controlName == name)
                    return child as T;

                var result = FindVisualChildByName<T>(child, name);
                if (result != null)
                    return result;
            }
            return null;
        }

        /// <summary>
        /// A simple iterator method to expose the visual tree to LINQ (parent to child).
        /// </summary>
        /// <param name="start">Starting root</param>
        /// <param name="predicate">Predicate called for each item to provide filter (can be null)</param>
        /// <returns>Enumerable list of visuals</returns>
        public static IEnumerable<T> EnumerateVisualTree<T>(this DependencyObject start, Predicate<T> predicate) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(start); i++)
            {
                var rawChild = VisualTreeHelper.GetChild(start, i);
                var child = rawChild as T;
                if (child != null && (predicate == null || predicate(child)))
                {
                    yield return child;
                    foreach (var childOfChild in EnumerateVisualTree(child, predicate))
                        yield return childOfChild;
                }
                else if (child == null)
                {
                    foreach (var childOfChild in EnumerateVisualTree(rawChild, predicate))
                        yield return childOfChild;
                }
            }
        }

        /// <summary>
        /// A simple iterator method to expose the visual tree to LINQ going backwards (child to parent)
        /// </summary>
        /// <param name="start">Starting child</param>
        /// <param name="predicate">Predicate called for each item to provide filter (can be null)</param>
        /// <returns>Enumerable list of visuals</returns>
        public static IEnumerable<T> ReverseEnumerateVisualTree<T>(this DependencyObject start, Predicate<T> predicate) where T : DependencyObject
        {
            for (; ; )
            {
                var parent = VisualTreeHelper.GetParent(start);
                if (parent == null)
                    break;
                var tParent = parent as T;
                if ((predicate == null || predicate(tParent)))
                    yield return tParent;
                start = parent;
            }
        }

        /// <summary>
        /// This enumerates the children of the given starting DPO.
        /// </summary>
        /// <param name="fe">Start</param>
        /// <returns>Collection of children (enumerator)</returns>
        public static IEnumerable<DependencyObject> VisualChildren(this DependencyObject fe)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(fe); i++)
                yield return VisualTreeHelper.GetChild(fe, i);
        }

        /// <summary>
        /// Performs a resource lookup, walking up the parent chain.
        /// </summary>
        /// <param name="fe"></param>
        /// <param name="name"></param>
        /// <param name="value"> </param>
        /// <returns></returns>
        public static bool TryFindResource(this FrameworkElement fe, string name, out object value)
        {
            if (fe == null)
                throw new ArgumentNullException("fe");

            while (fe != null)
            {
                if (fe.Resources.ContainsKey(name))
                {
                    value = fe.Resources["name"];
                    return true;
                }
                fe = VisualTreeHelper.GetParent(fe) as FrameworkElement;
            }

            value = null;
            return false;
        }
    }
}
