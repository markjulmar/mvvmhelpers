using System;
using System.Windows.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace JulMar.Windows.Interactivity.Media
{
    /// <summary>
    /// This action plays a sound
    /// </summary>
    public class PlaySoundAction : TriggerAction<FrameworkElement>
    {
        /// <summary>
        /// Backing storage for the Source
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(Uri), typeof(PlaySoundAction), null);

        /// <summary>
        /// URI source for the sound
        /// </summary>
        public Uri Source
        {
            get
            {
                return (Uri)base.GetValue(SourceProperty);
            }
            set
            {
                base.SetValue(SourceProperty, value);
            }
        }

        /// <summary>
        /// Backing storage for the volume
        /// </summary>
        public static readonly DependencyProperty VolumeProperty = DependencyProperty.Register("Volume", typeof(double), typeof(PlaySoundAction), new PropertyMetadata(0.5));

        /// <summary>
        /// Volume for the sound
        /// </summary>
        public double Volume
        {
            get
            {
                return (double)base.GetValue(VolumeProperty);
            }
            set
            {
                base.SetValue(VolumeProperty, value);
            }
        }

        /// <summary>
        /// Invoke method - must be overridden
        /// </summary>
        /// <param name="parameter"></param>
        protected override void Invoke(object parameter)
        {
            if (this.Source != null && base.AssociatedObject != null)
            {
                Popup popup = new Popup();
                MediaElement mediaElement = new MediaElement();
                popup.Child = mediaElement;

                mediaElement.Visibility = Visibility.Collapsed;
                SetMediaElementProperties(mediaElement);

                mediaElement.MediaEnded += (sender, args) =>
                {
                    popup.Child = null;
                    popup.IsOpen = false;
                };
                mediaElement.MediaFailed += (sender, args) =>
                {
                    popup.Child = null;
                    popup.IsOpen = false;
                };

                popup.IsOpen = true;
            }
        }

        /// <summary>
        /// Change the media properties
        /// </summary>
        /// <param name="mediaElement"></param>
        protected virtual void SetMediaElementProperties(MediaElement mediaElement)
        {
            if (mediaElement != null)
            {
                mediaElement.Source = this.Source;
                mediaElement.Volume = this.Volume;
            }
        }
    }

}
