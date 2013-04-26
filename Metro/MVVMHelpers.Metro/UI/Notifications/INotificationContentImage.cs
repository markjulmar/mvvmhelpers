namespace JulMar.Windows.UI.Notifications
{
    /// <summary>
    /// A type contained by the tile and toast notification content objects that
    /// represents an image in a template.
    /// </summary>
    public interface INotificationContentImage
    {
        /// <summary>
        /// The location of the image.  Relative image paths use the BaseUri provided in the containing
        /// notification object.  If no BaseUri is provided, paths are relative to ms-appx:///.
        /// Only png and jpg images are supported.  Images must be 1024x1024 pixels or less, and smaller than
        /// 200 kB in size.
        /// </summary>
        string Src { get; set; }

        /// <summary>
        /// Alt text that describes the image.
        /// </summary>
        string Alt { get; set; }

        /// <summary>
        /// Controls if query strings that denote the client configuration of contrast, scale, and language setting should be appended to the Src
        /// If true, Windows will append query strings onto the Src
        /// If false, Windows will not append query strings onto the Src
        /// Query string details:
        /// Parameter: ms-contrast
        ///     Values: standard, black, white
        /// Parameter: ms-scale
        ///     Values: 80, 100, 140, 180
        /// Parameter: ms-lang
        ///     Values: The BCP 47 language tag set in the notification xml, or if omitted, the current preferred language of the user
        /// </summary>
        bool AddImageQuery { get; set; }     
    }

    /// <summary>
    /// Internal class
    /// </summary>
    internal sealed class NotificationContentImage : INotificationContentImage
    {
        public string Src { get; set; }
        public string Alt { get; set; }
        public bool? AddImageQueryNullable { get; set; }

        public bool AddImageQuery
        {
            get { return AddImageQueryNullable != null && AddImageQueryNullable.Value; }
            set { AddImageQueryNullable = value; }
        }
    }
}
