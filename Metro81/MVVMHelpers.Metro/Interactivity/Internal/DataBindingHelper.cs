using System;
using System.Collections.Generic;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Microsoft.Xaml.Interactivity;

namespace JulMar.Windows.Interactivity.Internal
{
    /// <summary>
    /// Binding helper
    /// </summary>
    internal static class DataBindingHelper
    {
        private static readonly Dictionary<Type, IList<DependencyProperty>> DependenciesPropertyCache = new Dictionary<Type, IList<DependencyProperty>>();

        public static void EnsureBindingUpToDate(DependencyObject target, DependencyProperty dp)
        {
            BindingBase expression = target.ReadLocalValue(dp) as BindingBase;
            if (expression != null)
            {
                target.ClearValue(dp);
                target.SetValue(dp, expression);
            }
        }

        public static void EnsureDataBindingOnActionsUpToDate(ActionCollection actions)
        {
            foreach (var action in actions)
            {
                EnsureDataBindingUpToDateOnMembers(action);
            }
        }

        public static void EnsureDataBindingUpToDateOnMembers(DependencyObject dpObject)
        {
            IList<DependencyProperty> list = null;
            if (!DependenciesPropertyCache.TryGetValue(dpObject.GetType(), out list))
            {
                list = new List<DependencyProperty>();
                for (Type type = dpObject.GetType(); type != null; type = type.GetTypeInfo().BaseType)
                {
                    foreach (FieldInfo info in type.GetTypeInfo().DeclaredFields)
                    {
                        if (info.IsPublic && (info.FieldType == typeof(DependencyProperty)))
                        {
                            DependencyProperty item = info.GetValue(null) as DependencyProperty;
                            if (item != null)
                            {
                                list.Add(item);
                            }
                        }
                    }
                }
                DependenciesPropertyCache[dpObject.GetType()] = list;
            }

            if (list != null)
            {
                foreach (DependencyProperty dp in list)
                {
                    EnsureBindingUpToDate(dpObject, dp);
                }
            }
        }
    }
}
