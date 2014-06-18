using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using JulMar.Extensions;

namespace JulMar.Interactivity
{
    /// <summary>
    /// Behavior which applies a grayscale effect to an image when it's
    /// parent Button or MenuItem is disabled.
    /// </summary>
    public class AutoDisabledImageBehavior : Behavior<Image>
    {
        private FrameworkElement _owningControl;
        private ImageSource _originalSource;
        private bool _changingSource, _isEnabled;

        /// <summary>
        /// Attached property to allow this to be associated with an image in a Style setter.
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.RegisterAttached("IsActive", typeof(bool), typeof(AutoDisabledImageBehavior),
                new PropertyMetadata(default(bool), OnIsActiveChanged));

        /// <summary>
        /// Attached property getter
        /// </summary>
        /// <param name="theImage"></param>
        /// <returns></returns>
        public static bool GetIsActive(Image theImage)
        {
            return (bool)theImage.GetValue(IsActiveProperty);
        }

        /// <summary>
        /// Attached property setter
        /// </summary>
        /// <param name="theImage"></param>
        /// <param name="value"></param>
        public static void SetIsActive(Image theImage, bool value)
        {
            theImage.SetValue(IsActiveProperty, value);
        }

        /// <summary>
        /// Dependency Property to back the owner type property.
        /// </summary>
        public static readonly DependencyProperty OwnerTypeProperty =
            DependencyProperty.Register("OwnerType", typeof(Type), typeof(AutoDisabledImageBehavior), new PropertyMetadata(null));

        /// <summary>
        /// Owner type to look for (automatically scans for Button or MenuItem)
        /// </summary>
        public Type OwnerType
        {
            get { return (Type)this.GetValue(OwnerTypeProperty); }
            set { this.SetValue(OwnerTypeProperty, value); }
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();

            this._originalSource = this.AssociatedObject.Source;

            var dp = DependencyPropertyDescriptor.FromProperty(Image.SourceProperty, typeof(Image));
            if (dp != null)
                dp.AddValueChanged(this.AssociatedObject, this.OnSourceChanged);

            this.AssociatedObject.Loaded += this.OnImageLoaded;
            this.AssociatedObject.Unloaded += this.OnImageUnloaded;

            if (this.AssociatedObject.IsLoaded)
            {
                this.OnImageLoaded(null, null);
            }
        }


        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>
        /// Override this to unhook functionality from the AssociatedObject.
        /// </remarks>
        protected override void OnDetaching()
        {
            if (this._owningControl != null)
            {
                this.OnImageUnloaded(null, null);
            }

            var dp = DependencyPropertyDescriptor.FromProperty(Image.SourceProperty, typeof(Image));
            if (dp != null)
                dp.RemoveValueChanged(this.AssociatedObject, this.OnSourceChanged);

            this.AssociatedObject.Loaded -= this.OnImageLoaded;
            this.AssociatedObject.Unloaded -= this.OnImageUnloaded;

            this._originalSource = null;

            base.OnDetaching();
        }

        /// <summary>
        /// This is called when the Image.Source property is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSourceChanged(object sender, EventArgs e)
        {
            // If we are not the ones changing the source then cache off the new source.
            if (!this._changingSource)
            {
                this._originalSource = this.AssociatedObject.Source;
                if (this.AssociatedObject.IsLoaded && this._owningControl != null)
                    this.UpdateImageSource(this._isEnabled);
            }
        }

        /// <summary>
        /// This attaches our event handlers by locating the proper parent and hooking
        /// into it's IsEnabled change notification.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnImageLoaded(object sender, RoutedEventArgs e)
        {
            Debug.Assert(this._owningControl == null);

            if (this.OwnerType != null)
                this._owningControl = (FrameworkElement) this.AssociatedObject.FindVisualParent(this.OwnerType);
            else
                this._owningControl = this.AssociatedObject.FindVisualParent<ButtonBase>() 
                                    ?? (FrameworkElement) this.AssociatedObject.FindVisualParent<MenuItem>();

            if (this._owningControl != null)
            {
                this.UpdateImageSource(this._owningControl.IsEnabled);
                this._owningControl.IsEnabledChanged += this.OnOwnerEnabledStateChanged;
            }
        }

        /// <summary>
        /// This detaches the event handlers and unhooks our parent control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnImageUnloaded(object sender, RoutedEventArgs e)
        {
            if (this._owningControl != null)
            {
                this._owningControl.IsEnabledChanged -= this.OnOwnerEnabledStateChanged;
                this._owningControl = null;
            }
        }

        /// <summary>
        /// This is invoked when the owner (Button, etc.) IsEnabled state has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOwnerEnabledStateChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateImageSource(Convert.ToBoolean(e.NewValue));
        }

        /// <summary>
        /// This changes our image state from normal to disabled
        /// </summary>
        /// <param name="isEnabled"></param>
        private void UpdateImageSource(bool isEnabled)
        {
            this._changingSource = true;
            this._isEnabled = isEnabled;

            try
            {
                if (this._isEnabled)
                {
#if WPF_TOOLKIT
                    AssociatedObject.Source = _originalSource;
#else
                    this.AssociatedObject.SetCurrentValue(Image.SourceProperty, this._originalSource);
#endif
                    this.AssociatedObject.OpacityMask = null;
                }
                else
                {
                    BitmapSource bs = this._originalSource as BitmapSource;
                    if (bs == null)
                    {
                        try
                        {
                            // See if we can load an image from the URI
                            bs = new BitmapImage(new Uri(this._originalSource.ToString()));
                        }
                        catch
                        {
                            // Unknown bitmap type
                            return;
                        }
                    }

                    var grayImage = new FormatConvertedBitmap(bs, PixelFormats.Gray32Float, null, 0);
#if WPF_TOOLKIT
                    AssociatedObject.Source = grayImage;
#else
                    this.AssociatedObject.SetCurrentValue(Image.SourceProperty, grayImage);
#endif
                    this.AssociatedObject.OpacityMask = new ImageBrush(bs);

                }
            }
            finally
            {
                this._changingSource = false;
            }
        }

        /// <summary>
        /// This adds the behavior to an image through an attached property.
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="e"></param>
        private static void OnIsActiveChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            BehaviorCollection bc = Interaction.GetBehaviors(dpo);
            if (Convert.ToBoolean(e.NewValue))
            {
                bc.Add(new AutoDisabledImageBehavior());
            }
            else
            {
                var behavior = bc.FirstOrDefault(beh => beh.GetType() == typeof(AutoDisabledImageBehavior));
                if (behavior != null)
                    bc.Remove(behavior);
            }
        }
    }
}
