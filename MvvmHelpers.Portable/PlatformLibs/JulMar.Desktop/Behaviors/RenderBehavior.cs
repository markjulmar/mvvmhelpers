using System.Windows;

namespace JulMar.Behaviors
{
    ///<summary>
    /// This attached property can be used from a ViewModel to force a part of the tree to render
    /// based on some known property change that can impact it but is not data bound.  To use it, 
    /// associated it onto any Visual to a {Binding} and then change the value - that will force the 
    /// Visual and all it's children to be invalidated.
    ///</summary>
    public static class RenderBehavior
    {
        /// <summary>
        /// Render property - data bind this to force rendering
        /// </summary>
        public static readonly DependencyProperty InvalidateProperty = DependencyProperty.RegisterAttached("Invalidate", typeof(object), typeof(RenderBehavior),
                                            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Retrieve the invalidate render property
        /// </summary>
        /// <param name="dpo"></param>
        /// <returns></returns>
        public static object GetInvalidate(DependencyObject dpo)
        {
            return dpo.GetValue(InvalidateProperty);
        }

        /// <summary>
        /// Set the render property
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="value"></param>
        public static void SetInvalidate(DependencyObject dpo, object value)
        {
            dpo.SetValue(InvalidateProperty, value);
        }
    }
}
