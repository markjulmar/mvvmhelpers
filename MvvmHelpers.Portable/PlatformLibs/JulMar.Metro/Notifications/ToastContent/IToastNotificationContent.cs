using Windows.UI.Notifications;

namespace JulMar.Notifications.ToastContent
{
    /// <summary>
    /// Base toast notification content interface.
    /// </summary>
    public interface IToastNotificationContent : INotificationContent
    {
        /// <summary>
        /// Whether strict validation should be applied when the Xml or notification object is created,
        /// and when some of the properties are assigned.
        /// </summary>
        bool StrictValidation { get; set; }

        /// <summary>
        /// The language of the content being displayed.  The language should be specified using the
        /// abbreviated language code as defined by BCP 47.
        /// </summary>
        string Lang { get; set; }

        /// <summary>
        /// The BaseUri that should be used for image locations.  Relative image locations use this
        /// field as their base Uri.  The BaseUri must begin with http://, https://, ms-appx:///, or 
        /// ms-appdata:///local/.
        /// </summary>
        string BaseUri { get; set; }

        /// <summary>
        /// Controls if query strings that denote the client configuration of contrast, scale, and language setting should be appended to the Src
        /// If true, Windows will append query strings onto images that exist in this template
        /// If false (the default, Windows will not append query strings onto images that exist in this template            
        /// Query string details:
        /// Parameter: ms-contrast
        ///     Values: standard, black, white
        /// Parameter: ms-scale
        ///     Values: 80, 100, 140, 180
        /// Parameter: ms-lang
        ///     Values: The BCP 47 language tag set in the notification xml, or if omitted, the current preferred language of the user
        /// </summary>
        bool AddImageQuery { get; set; }

        /// <summary>
        /// The launch parameter passed into the Windows Store app when the toast is activated.
        /// </summary>
        string Launch { get; set; }

        /// <summary>
        /// The audio that should be played when the toast is displayed.
        /// </summary>
        IToastAudio Audio { get; }

        /// <summary>
        /// The length that the toast should be displayed on screen.
        /// </summary>
        ToastDuration Duration { get; set; }

        /// <summary>
        /// Creates a WinRT ToastNotification object based on the content.
        /// </summary>
        /// <returns>A WinRT ToastNotification object based on the content.</returns>
        ToastNotification CreateNotification();

        /// <summary>
        /// Generates a package URI
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string FromPackage(string name);
    }
}