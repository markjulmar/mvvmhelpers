using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace JulMar.Windows.UI
{
    /// <summary>
    /// This class generates a Geometry from a block of text in a specific font, weight, etc.
    /// and renders it to WPF as a shape.
    /// </summary>
    public class TextPath : Shape
    {
        /// <summary>
        /// Data member that holds the generated geometry
        /// </summary>
        private Geometry _textGeometry;

        #region Depdendency Properties
        /// <summary>
        /// Text (string) to display
        /// </summary>
        public static readonly DependencyProperty TextProperty = 
            DependencyProperty.Register("Text", typeof(string), typeof(TextPath), 
                                        new FrameworkPropertyMetadata("TextPath",
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnPropertyChanged));

        /// <summary>
        /// Origin (0,0) of the text
        /// </summary>
        public static readonly DependencyProperty OriginPointProperty = 
            DependencyProperty.Register("Origin", typeof(Point), typeof(TextPath),
                                        new FrameworkPropertyMetadata(new Point(0, 0),
                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                      OnPropertyChanged));

        /// <summary>
        /// Font family to render text in
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner(typeof(TextPath), 
                                                                                                               new FrameworkPropertyMetadata(SystemFonts.MessageFontFamily,
                                                                                                                                             FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits,
                                                                                                                                             OnPropertyChanged));

        /// <summary>
        /// Font size to render text in
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty = TextElement.FontSizeProperty.AddOwner(typeof (TextPath),
                                                                                                           new FrameworkPropertyMetadata(SystemFonts.MessageFontSize,
                                                                                                                                         FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                                                                                                         OnPropertyChanged));

        /// <summary>
        /// Font Stretch
        /// </summary>
        public static readonly DependencyProperty FontStretchProperty = TextElement.FontStretchProperty.AddOwner(typeof(TextPath),
                                                                                                                 new FrameworkPropertyMetadata(TextElement.FontStretchProperty.DefaultMetadata.DefaultValue,
                                                                                                                                               FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits,
                                                                                                                                               OnPropertyChanged));

        /// <summary>
        /// Font Style
        /// </summary>
        public static readonly DependencyProperty FontStyleProperty = TextElement.FontStyleProperty.AddOwner(typeof(TextPath),
                                                                                                             new FrameworkPropertyMetadata(SystemFonts.MessageFontStyle,
                                                                                                                                           FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits,
                                                                                                                                           OnPropertyChanged));

        /// <summary>
        /// Font Weight
        /// </summary>
        public static readonly DependencyProperty FontWeightProperty = TextElement.FontWeightProperty.AddOwner(typeof(TextPath),
                                                                                                               new FrameworkPropertyMetadata(SystemFonts.MessageFontWeight,
                                                                                                                                             FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits,
                                                                                                                                             OnPropertyChanged));
        #endregion

        #region Property Accessors
        /// <summary>
        /// Gets/Sets the text origin
        /// </summary>
        [Bindable(true), Category("Appearance")]
        [TypeConverter(typeof(PointConverter))]
        public Point Origin
        {
            get { return (Point)GetValue(OriginPointProperty); }
            set { SetValue(OriginPointProperty, value); }
        }

        /// <summary>
        /// Gets/Sets the Font Family
        /// </summary>
        [Bindable(true), Category("Appearance")]
        [Localizability(LocalizationCategory.Font)]
        [TypeConverter(typeof(FontFamilyConverter))]
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        /// <summary>
        /// Gets/Sets the Font Size
        /// </summary>
        [Bindable(true), Category("Appearance")]
        [TypeConverter(typeof(FontSizeConverter))]
        [Localizability(LocalizationCategory.None)]
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// Gets/Sets the FontStretch property
        /// </summary>
        [Bindable(true), Category("Appearance")]
        [TypeConverter(typeof(FontStretchConverter))]
        public FontStretch FontStretch
        {
            get { return (FontStretch)GetValue(FontStretchProperty); }
            set { SetValue(FontStretchProperty, value); }
        }

        /// <summary>
        /// Gets/Sets the Font Style
        /// </summary>
        [Bindable(true), Category("Appearance")]
        [TypeConverter(typeof(FontStyleConverter))]
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        /// <summary>
        /// Gets/Sets the Font Weight
        /// </summary>
        [Bindable(true), Category("Appearance")]
        [TypeConverter(typeof(FontWeightConverter))]
        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        /// <summary>
        /// Gets/Sets the text to draw
        /// </summary>
        [Bindable(true), Category("Appearance")]
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        #endregion

        /// <summary>
        /// This method is called to retrieve the geometry that defines the shape.
        /// </summary>
        protected override Geometry DefiningGeometry
        {
            get { return _textGeometry ?? Geometry.Empty; }
        }

        /// <summary>
        /// This method is called when any of our dependency properties change - it
        /// changes the geometry so it is drawn properly.
        /// </summary>
        /// <param name="d">Depedency Object</param>
        /// <param name="e">EventArgs</param>
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextPath)d).CreateTextGeometry();
        }

        /// <summary>
        /// This method creates the text geometry.
        /// </summary>
        private void CreateTextGeometry()
        {
            var formattedText = new FormattedText(Text, Thread.CurrentThread.CurrentUICulture,
                                                  FlowDirection.LeftToRight, 
                                                  new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
                                                  FontSize, Brushes.Black);
            _textGeometry = formattedText.BuildGeometry(Origin);
        }
    }
}