using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;

namespace JulMar.Windows.Actions
{
    /// <summary>
    /// This changes the window's theme to the given theme URI.
    /// </summary>
    public class ChangeThemeAction : TriggerAction<Window>
    {
        ///<summary>
        /// Theme URI to apply to the application
        ///</summary>
        public readonly DependencyProperty ThemeUriProperty = 
            DependencyProperty.Register("ThemeUri", typeof (Uri), typeof (ChangeThemeAction));
           
        /// <summary>
        /// Theme URI to apply to the application.
        /// </summary>
        [TypeConverter(typeof(UriTypeConverter))]
        public Uri ThemeUri
        {
            get { return (Uri) GetValue(ThemeUriProperty); }
            set { SetValue(ThemeUriProperty, value); }
        }

        /// <summary>
        /// This event is raised when the theme is changed.
        /// </summary>
        public event EventHandler ThemeChanged;

        /// <summary>
        /// Invokes the action.
        /// </summary>
        /// <param name="parameter">The parameter to the action. If the Action does not require a parameter, the parameter may be set to a null reference.</param>
        protected override void Invoke(object parameter)
        {
            // See if we were passed a parameter indicating whether to run our action or reverse the action.
            // Assume to run if not supplied.
            bool isThemeEnabled;
            if (parameter == null || !Boolean.TryParse(parameter.ToString(), out isThemeEnabled))
                isThemeEnabled = true;

            // Reversing theme? Remove resources.
            if (!isThemeEnabled)
            {
                AssociatedObject.Resources.MergedDictionaries.Clear();
                return;
            }

            // Running action -- no theme supplied? Nothing to do.
            if (ThemeUri == null)
                return;

            // See if the element we're applied to is loaded.  If not, we can't do our theme switch
            // here, instead we need to wait until it's loaded.
            if (!AssociatedObject.IsLoaded)
            {
                // Hook up a temporary handler
                RoutedEventHandler waitForLoad = null;
                waitForLoad = (s,e) => {
                   AssociatedObject.Loaded -= waitForLoad;
                   Invoke(isThemeEnabled);
                };
                AssociatedObject.Loaded += waitForLoad;
                
                return;
            }

            // Apply the theme.
            try
            {
                ResourceDictionary rd = Application.LoadComponent(ThemeUri) as ResourceDictionary;
                if (rd != null)
                {
                    // See if we have content, and it's a UIElement type -- if so, we will try to gradually
                    // fade the content for a nice effect.  We also require an Adorner Layer to be present
                    // in the Window's control template
                    UIElement elementContent = AssociatedObject.Content as UIElement;
                    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(elementContent ?? AssociatedObject);
                    if (elementContent == null || adornerLayer == null)
                    {
                        // No layer, just replace the theme.
                        AssociatedObject.Resources.MergedDictionaries.Clear();
                        AssociatedObject.Resources.MergedDictionaries.Add(rd);
                        return;
                    }

                    int width, height;
                    if (elementContent is FrameworkElement)
                    {
                        FrameworkElement fe = (FrameworkElement)elementContent;
                        width = (int) fe.ActualWidth;
                        height = (int) fe.ActualHeight;
                    }
                    else
                    {
                        width = (int) (AssociatedObject.ActualWidth - SystemParameters.BorderWidth*2);
                        height = (int) (AssociatedObject.ActualHeight - SystemParameters.CaptionHeight);
                    }

                    // Capture a bitmap of the current screen.
                    var currentContent = (Visual) VisualTreeHelper.GetChild(AssociatedObject, 0);
                    RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
                    rtb.Render(currentContent);

                    var staticImage = new Image { Source = rtb, Stretch = Stretch.None, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Top };
                    var imageAdorner = new FrameworkElementAdorner(elementContent, staticImage);
                    adornerLayer.Add(imageAdorner);

                    // Merge the resources in.
                    AssociatedObject.Resources.MergedDictionaries.Clear();
                    AssociatedObject.Resources.MergedDictionaries.Add(rd);

                    // Now animate the opacity of the image out so we can see the real controls
                    // underneath, then drop the image off.
                    DoubleAnimation doubleAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(1500)));
                    doubleAnimation.Completed += (s, e) =>
                     {
                         adornerLayer.Remove(imageAdorner);
                         RaiseThemeChanged();
                     };
                    staticImage.BeginAnimation(UIElement.OpacityProperty, doubleAnimation);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Raises the ThemeChanged event
        /// </summary>
        private void RaiseThemeChanged()
        {
            EventHandler handler = ThemeChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}


