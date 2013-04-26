namespace JulMar.Windows.UI.Notifications.ToastContent
{
    /// <summary>
    /// The audio options that can be played while the toast is on screen.
    /// </summary>
    public enum ToastAudioContent
    {
        /// <summary>
        /// The default toast audio sound.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Audio that corresponds to new mail arriving.
        /// </summary>
        Mail,
        /// <summary>
        /// Audio that corresponds to a new SMS message arriving.
        /// </summary>
        SMS,
        /// <summary>
        /// Audio that corresponds to a new IM arriving.
        /// </summary>
        IM,
        /// <summary>
        /// Audio that corresponds to a reminder.
        /// </summary>
        Reminder,
        /// <summary>
        /// The default looping sound.  Audio that corresponds to a call.
        /// Only valid for toasts that are have the duration set to "Long".
        /// </summary>
        LoopingCall,
        /// <summary>
        /// Audio that corresponds to a call.
        /// Only valid for toasts that are have the duration set to "Long".
        /// </summary>
        LoopingCall2,
        /// <summary>
        /// Audio that corresponds to an alarm.
        /// Only valid for toasts that are have the duration set to "Long".
        /// </summary>
        LoopingAlarm,
        /// <summary>
        /// Audio that corresponds to an alarm.
        /// Only valid for toasts that are have the duration set to "Long".
        /// </summary>
        LoopingAlarm2,
        /// <summary>
        /// No audio should be played when the toast is displayed.
        /// </summary>
        Silent
    }
}