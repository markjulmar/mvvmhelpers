using System;
using System.Windows.Markup;
using System.Windows.Media;

namespace JulMar.Windows.Markup
{
    /// <summary>
    /// This creates a gradient color brush
    /// </summary>
    [MarkupExtensionReturnType(typeof(LinearGradientBrush))]
    public class LinearGradientBrushExtension : MarkupExtension
    {
        /// <summary>
        /// Starting color
        /// </summary>
        public Color StartColor { get; set; }

        /// <summary>
        /// Ending color
        /// </summary>
        public Color EndColor { get; set; }

        /// <summary>
        /// Angle for gradient
        /// </summary>
        public double Angle { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public LinearGradientBrushExtension()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="startColor">Starting Color</param>
        /// <param name="endColor">Ending Color</param>
        /// <param name="angle">Angle</param>
        public LinearGradientBrushExtension(Color startColor, Color endColor, double angle)
        {
            StartColor = startColor;
            EndColor = endColor;
            Angle = angle;
        }

        /// <summary>
        /// When implemented in a derived class, returns an object that is set as the value of the target property for this markup extension. 
        /// </summary>
        /// <returns>
        /// The object value to set on the property where the extension is applied. 
        /// </returns>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.
        ///                 </param>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new LinearGradientBrush(StartColor, EndColor, Angle);
        }
    }
}
