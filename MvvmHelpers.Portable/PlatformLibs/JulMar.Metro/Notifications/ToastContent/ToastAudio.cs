namespace JulMar.Notifications.ToastContent
{
    internal sealed class ToastAudio : IToastAudio
    {
        public ToastAudioContent Content { get; set; }
        public bool Loop { get; set; }

        public ToastAudio()
        {
            this.Content = ToastAudioContent.Default;
            this.Loop = false;
        }
    }
}