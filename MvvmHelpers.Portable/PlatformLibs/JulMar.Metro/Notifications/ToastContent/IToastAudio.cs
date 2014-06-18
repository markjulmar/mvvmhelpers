namespace JulMar.Notifications.ToastContent
{
    /// <summary>
    /// Type representing the toast notification audio properties which is contained within
    /// a toast notification content object.
    /// </summary>
    public interface IToastAudio
    {
        /// <summary>
        /// The audio content that should be played when the toast is shown.
        /// </summary>
        ToastAudioContent Content { get; set; }

        /// <summary>
        /// Whether the audio should loop.  If this property is set to true, the toast audio content
        /// must be a looping sound.
        /// </summary>
        bool Loop { get; set; }
    }
}