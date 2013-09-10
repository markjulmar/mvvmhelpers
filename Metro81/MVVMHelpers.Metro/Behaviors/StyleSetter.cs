using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JulMar.Core.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;

namespace JulMar.Windows.Behaviors
{
    /// <summary>
    /// Class to help bind setters in a Style as Metro doesn't support it in V1. This class may be removed once the support is added to
    /// the binding engine.
    /// </summary>
    /// <notes>
    /// This class was adapted from the Silverlight version written by David Anson
    /// http://blogs.msdn.com/b/delay/archive/2009/11/02/as-the-platform-evolves-so-do-the-workarounds-better-settervaluebindinghelper-makes-silverlight-setters-better-er.aspx
    /// </notes>
    /// <example>
    /// <![CDATA[
    /// 
    /// <Style TargetType="ListBoxItem">
    ///    <Setter Property="julmar:StyleSetter.PropertyBindings">
    ///       <Setter.Value>
    ///          <SetterValueBinding Type="Canvas" Property="Left" Binding="{Binding X}" />
    ///          <SetterValueBinding Type="Canvas" Property="Top" Binding="{Binding Y}" />
    ///       </Setter.Value>
    ///    </Setter>
    /// </Style>
    /// ]]>
    /// </example>
    [ContentProperty(Name = "Values")]
    public sealed class StyleSetter
    {
        private Collection<SetterValueBinding> _values;

        /// <summary>
        /// Collection of SetterValueBindingHelper instances to apply to the
        /// target element.
        /// </summary>
        /// <remarks>
        /// Used when multiple Bindings need to be applied to the same element.
        /// </remarks>
        public Collection<SetterValueBinding> Values
        {
            get
            {
                return _values ?? (_values = new Collection<SetterValueBinding>());
            }
        }

        /// <summary>
        /// PropertyBinding attached DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty PropertyBindingsProperty = DependencyProperty.RegisterAttached("PropertyBindings",
                typeof(StyleSetter), typeof(StyleSetter), new PropertyMetadata(null, OnPropertyBindingPropertyChanged));

        /// <summary>
        /// Gets the value of the PropertyBinding attached DependencyProperty.
        /// </summary>
        /// <param name="element">Element for which to get the property.</param>
        /// <returns>Value of PropertyBinding attached DependencyProperty.</returns>
        public static StyleSetter GetPropertyBindings(FrameworkElement element)
        {
            if (null == element)
                throw new ArgumentNullException("element");

            return (StyleSetter)element.GetValue(PropertyBindingsProperty);
        }

        /// <summary>
        /// Sets the value of the PropertyBinding attached DependencyProperty.
        /// </summary>
        /// <param name="element">Element on which to set the property.</param>
        /// <param name="value">Value forPropertyBinding attached DependencyProperty.</param>
        public static void SetPropertyBindings(FrameworkElement element, StyleSetter value)
        {
            if (null == element)
                throw new ArgumentNullException("element");

            element.SetValue(PropertyBindingsProperty, value);
        }

        /// <summary>
        /// Change handler for the PropertyBinding attached DependencyProperty.
        /// </summary>
        /// <param name="dpo">Object on which the property was changed.</param>
        /// <param name="e">Property change arguments.</param>
        private static void OnPropertyBindingPropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            // Get/validate parameters
            var element = (FrameworkElement)dpo;
            var item = (StyleSetter) e.NewValue;

            // Apply the bindings of each child
            foreach (var child in item.Values)
            {
                ApplyBinding(element, child);
            }
        }

        /// <summary>
        /// Applies the Binding represented by the SetterValueBindingHelper.
        /// </summary>
        /// <param name="element">Element to apply the Binding to.</param>
        /// <param name="item">SetterValueBindingHelper representing the Binding.</param>
        private static void ApplyBinding(FrameworkElement element, SetterValueBinding item)
        {
            // Get the type on which to set the Binding
            Type type = null;
            if (null == item.Type)
            {
                // No type specified; setting for the specified element
                type = element.GetType();
            }
            else
            {
                // Try to get the type from the type system
                type = Type.GetType(item.Type);
                if (null == type)
                {
                    // Search for the type in the list of assemblies
                    foreach (var assembly in AssembliesToSearch)
                    {
                        // Match on short or full name
                        var typeInfo = assembly.DefinedTypes.FirstOrDefault(t => (t.FullName == item.Type) || (t.Name == item.Type));
                        if (null != typeInfo)
                        {
                            // Found; done searching
                            type = typeInfo.AsType();
                            break;
                        }
                    }
                    if (null == type)
                    {
                        // Unable to find the requested type anywhere
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                            "Unable to access type \"{0}\". Try using an assembly qualified type name.",
                            item.Type));
                    }
                }
            }

            // Get the DependencyProperty for which to set the Binding
            DependencyProperty property = null;
            var field = type.GetTypeInfo().DeclaredProperties.FirstOrDefault(fi => fi.Name == item.Property + "Property");
                
            if (null != field)
            {
                property = field.GetValue(null) as DependencyProperty;
            }
            if (null == property)
            {
                // Unable to find the requested property
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                    "Unable to access DependencyProperty \"{0}\" on type \"{1}\".",
                    item.Property, type.Name));
            }

            // Set the specified Binding on the specified property
            element.SetBinding(property, item.Binding);
        }

        /// <summary>
        /// Returns a stream of assemblies to search for the provided type name.
        /// </summary>
        private static IEnumerable<Assembly> AssembliesToSearch
        {
            get
            {
                // Start with the System.Windows assembly (home of all core controls)
                yield return typeof(Control).GetTypeInfo().Assembly;

                // Return list of assemblies included in package.
                List<Assembly> theAsms = null;
                Task.Run(async () =>
                                   {
                                       var results = await DynamicComposer.GetPackageAssemblyListAsync();
                                       theAsms = results.ToList();
                                   }).Wait();
                foreach (var asm in theAsms)
                    yield return asm;
            }
        }
    }

    /// <summary>
    /// A binding value to apply to a Style setter
    /// </summary>
    public sealed class SetterValueBinding
    {
        /// <summary>
        /// Optional type parameter used to specify the type of an attached
        /// DependencyProperty as an assembly-qualified name, full name, or
        /// short name.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Property name for the normal/attached DependencyProperty on which
        /// to set the Binding.
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// Binding to set on the specified property.
        /// </summary>
        public Binding Binding { get; set; }
    }
}
