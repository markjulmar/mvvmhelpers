namespace JulMar.Windows.UI.Notifications.ToastContent
{
    internal sealed class ToastAudio : IToastAudio
    {
        public ToastAudioContent Content { get; set; }
        public bool Loop { get; set; }

        public ToastAudio()
        {
            Content = ToastAudioContent.Default;
            Loop = false;
        }
    }
}