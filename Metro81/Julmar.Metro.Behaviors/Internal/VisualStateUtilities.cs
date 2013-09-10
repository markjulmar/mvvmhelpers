using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace JulMar.Windows.Interactivity.Internal
{
    internal static class VisualStateUtilities
    {
        /// <summary>
        /// Locate the nearest element with a VSM group
        /// </summary>
        /// <param name="contextElement"></param>
        /// <returns></returns>
        internal static FrameworkElement FindNearestStatefulControl(FrameworkElement contextElement)
        {
            FrameworkElement resolvedControl = null;
            TryFindNearestStatefulControl(contextElement, out resolvedControl);
            return resolvedControl;
        }

        /// <summary>
        /// Retrieve the parent object
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private static FrameworkElement FindTemplatedParent(FrameworkElement parent)
        {
            return (VisualTreeHelper.GetParent(parent) as FrameworkElement);
        }

        /// <summary>
        /// Retrieve the VSM groups from an object
        /// </summary>
        /// <param name="targetObject"></param>
        /// <returns></returns>
        public static IList<VisualStateGroup> GetVisualStateGroups(FrameworkElement targetObject)
        {
            IList<VisualStateGroup> visualStateGroups = new List<VisualStateGroup>();
            if (targetObject != null)
            {
                visualStateGroups = VisualStateManager.GetVisualStateGroups(targetObject);
                if ((visualStateGroups.Count == 0) && (VisualTreeHelper.GetChildrenCount(targetObject) > 0))
                {
                    FrameworkElement child = VisualTreeHelper.GetChild(targetObject, 0) as FrameworkElement;
                    visualStateGroups = VisualStateManager.GetVisualStateGroups(child);
                }
            }
            return visualStateGroups;
        }

        /// <summary>
        /// Goto a state
        /// </summary>
        /// <param name="element"></param>
        /// <param name="stateName"></param>
        /// <param name="useTransitions"></param>
        /// <returns></returns>
        public static bool GoToState(FrameworkElement element, string stateName, bool useTransitions)
        {
            if (string.IsNullOrEmpty(stateName))
                return false;

            Control control = element as Control;
            if (control != null)
            {
                control.ApplyTemplate();
                return VisualStateManager.GoToState(control, stateName, useTransitions);
            }

            return false;
        }

        /// <summary>
        /// Determine whether the element has VSM groups
        /// </summary>
        /// <param name="frameworkElement"></param>
        /// <returns></returns>
        private static bool HasVisualStateGroupsDefined(FrameworkElement frameworkElement)
        {
            return ((frameworkElement != null) && (VisualStateManager.GetVisualStateGroups(frameworkElement).Count != 0));
        }

        /// <summary>
        /// Helper to determine if we continue walking Visual Tree
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static bool ShouldContinueTreeWalk(FrameworkElement element)
        {
            if (element == null || element is UserControl)
                return false;

            if (element.Parent == null)
            {
                FrameworkElement templateParent = FindTemplatedParent(element);
                if ((templateParent == null) || (!(templateParent is Control) && !(templateParent is ContentPresenter)))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Find nearest control with VSM groups
        /// </summary>
        /// <param name="contextElement"></param>
        /// <param name="resolvedControl"></param>
        /// <returns></returns>
        public static bool TryFindNearestStatefulControl(FrameworkElement contextElement, out FrameworkElement resolvedControl)
        {
            FrameworkElement frameworkElement = contextElement;
            if (frameworkElement == null)
            {
                resolvedControl = null;
                return false;
            }
            
            FrameworkElement element = frameworkElement.Parent as FrameworkElement;
            
            bool flag = true;
            while (!HasVisualStateGroupsDefined(frameworkElement) && ShouldContinueTreeWalk(element))
            {
                frameworkElement = element;
                if (element != null)
                    element = element.Parent as FrameworkElement;
            }
            
            if (HasVisualStateGroupsDefined(frameworkElement))
            {
                FrameworkElement parent = VisualTreeHelper.GetParent(frameworkElement) as FrameworkElement;
                if ((parent != null) && (parent is Control))
                {
                    frameworkElement = parent;
                }
            }
            else
            {
                flag = false;
            }
            resolvedControl = frameworkElement;
            return flag;
        }
    }
}