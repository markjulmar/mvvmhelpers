using System.Collections.Generic;
using System.Linq;
#if CLASSIC
using MonoTouch.UIKit;
#else
using UIKit;
#endif

namespace JulMar.Extensions
{
    /// <summary>
    /// UIView (UIKit) extensions
    /// </summary>
    public static class UIViewExtensions
    {
        /// <summary>
        /// Returns the subviews and all descendent's from a given UIView.
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public static IEnumerable<UIView> Descendants(this UIView view)
        {
            return view.Subviews?.Concat(view.Subviews.SelectMany(child => child.Descendants())) 
                ?? Enumerable.Empty<UIView>();
        }
    }
}