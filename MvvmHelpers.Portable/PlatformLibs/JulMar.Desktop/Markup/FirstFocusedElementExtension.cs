using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

using JulMar.Extensions;

namespace JulMar.Markup
{
    /// <summary>
    /// This markup extension locates the first focusable child and returns it.
    /// It is intended to be used with FocusManager.FocusedElement:
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// 
    ///    <Window ... FocusManager.FocusedElement={ft:FirstFocusedElement} />
    /// 
    /// ]]>
    /// </example>
    [MarkupExtensionReturnType(typeof(FrameworkElement))]
    public class FirstFocusedElementExtension : MarkupExtension
    {
        /// <summary>
        /// Unhook the handler after it has set focus to the element the first time
        /// </summary>
        public bool OneTime { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public FirstFocusedElementExtension()
        {
            this.OneTime = true;
        }

        /// <summary>
        /// This method locates the first focusable + visible element we can
        /// change focus to.
        /// </summary>
        /// <param name="serviceProvider">IServiceProvider from XAML</param>
        /// <returns>Focusable Element or null</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // Ignore if in design mode
            if (Designer.InDesignMode)
                return null;

            // Get the IProvideValue interface which gives us access to the target property 
            // and object.  Note that MSDN calls this interface "internal" but it's necessary
            // here because we need to know what property we are assigning to.
            var pvt = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (pvt != null)
            {
                var fe = pvt.TargetObject as FrameworkElement;
                object targetProperty = pvt.TargetProperty;
                if (fe != null)
                {
                    // If the element isn't loaded yet, then wait for it.
                    if (!fe.IsLoaded)
                    {
                        RoutedEventHandler deferredFocusHookup = null;
                        deferredFocusHookup = (sender, e) =>
                             {
                                 // Ignore if the element is now loaded but not
                                 // visible -- this happens for things like TabItem.
                                 // Instead, we'll wait until the item becomes visible and
                                 // then set focus.
                                 if (fe.IsVisible == false)
                                     return;

                                 // Look for the first focusable leaf child and set the property
                                 IInputElement ie = GetLeafFocusableChild(fe);
                                 if (targetProperty is DependencyProperty)
                                 {
                                     // Specific case where we are setting focused element.
                                     // We really need to set this property onto the focus scope, 
                                     // so we'll use UIElement.Focus() which will do exactly that.
                                     if (targetProperty == FocusManager.FocusedElementProperty)
                                     {
                                         ie.Focus();
                                     }
                                         // Being assigned to some other property - just assign it.
                                     else
                                     {
                                         fe.SetValue((DependencyProperty) targetProperty, ie);
                                     }
                                 }
                                     // Simple property assignment through reflection.
                                 else if (targetProperty is PropertyInfo)
                                 {
                                     var pi = (PropertyInfo) targetProperty;
                                     pi.SetValue(fe, ie, null);
                                 }

                                 // Unhook the handler if we are supposed to.
                                 if (this.OneTime)
                                     fe.Loaded -= deferredFocusHookup;
                             };

                        // Wait for the element to load
                        fe.Loaded += deferredFocusHookup;
                    }
                    else
                    {
                        return GetLeafFocusableChild(fe);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Locate the first real focusable child.  We keep going down
        /// the visual tree until we hit a leaf node.
        /// </summary>
        /// <param name="fe"></param>
        /// <returns></returns>
        static IInputElement GetLeafFocusableChild(IInputElement fe)
        {
            IInputElement ie = GetFirstFocusableChild(fe), final = ie;
            while (final != null)
            {
                ie = final;
                final = GetFirstFocusableChild(final);
            }

            return ie;
        }

        /// <summary>
        /// This searches the Visual Tree looking for a valid child which can have focus.
        /// </summary>
        /// <param name="fe"></param>
        /// <returns></returns>
        static IInputElement GetFirstFocusableChild(IInputElement fe)
        {
            var dpo = fe as DependencyObject;
            return dpo == null ? null : (from vc in dpo.EnumerateVisualTree<DependencyObject>(c => !FocusManager.GetIsFocusScope(c))
                                         let iic = vc as IInputElement
                                         where iic != null && iic.Focusable && iic.IsEnabled &&
                                               (!(iic is FrameworkElement) || (((FrameworkElement)iic).IsVisible))
                                         select iic).FirstOrDefault();
        }


    }
}