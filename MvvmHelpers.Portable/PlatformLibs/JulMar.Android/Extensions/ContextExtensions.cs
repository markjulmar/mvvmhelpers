using System;
using Android.Content;
using Android.Views;
using Android.Views.InputMethods;

namespace JulMar.Extensions
{
    /// <summary>
    /// Helper extensions for the Android Context
    /// </summary>
    public static class ContextExtensions
    {
        /// <summary>
        /// Returns whether this device is considered a tablet (vs. a phone).
        /// </summary>
        /// <param name="context">Context</param>
        /// <returns>True/False</returns>
        public static bool IsTablet(this Context context)
        {
            var min = Math.Min(context.Resources.DisplayMetrics.WidthPixels, context.Resources.DisplayMetrics.HeightPixels);
            var minDp = context.PixelsToDps(min);
            return minDp >= 600;
        }

        /// <summary>
        /// Conversion from DPs to Pixels
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="dp">DPs</param>
        /// <returns>Pixels</returns>
        public static float DpsToPixels(this Context context, double dp)
        {
            using (var metrics = context.Resources.DisplayMetrics)
                return (float)Math.Round(dp * metrics.Density);
        }

        /// <summary>
        /// Conversion from Pixels to DPs
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="pixels">Pixels</param>
        /// <returns>DPs</returns>
        public static double PixelsToDps(this Context context, double pixels)
        {
            using (var metrics = context.Resources.DisplayMetrics)
                return (pixels / metrics.Density);
        }

        /// <summary>
        /// Shows the soft keyboard for a view
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="view">View</param>
        public static void ShowKeyboard(this Context context, View view)
        {
            var service = (InputMethodManager) context.GetSystemService(Context.InputMethodService);
            service.ShowSoftInputFromInputMethod(view.WindowToken, 0); 
        }

        /// <summary>
        /// Hides the keyboard for a view
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="view">View</param>
        public static void HideKeyboard(this Context context, View view)
        {
            var service = (InputMethodManager) context.GetSystemService(Context.InputMethodService);
            service.HideSoftInputFromWindow(view.WindowToken, 0);
        }
    }
}