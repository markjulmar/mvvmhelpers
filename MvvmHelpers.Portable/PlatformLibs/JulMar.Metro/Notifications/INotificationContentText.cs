namespace JulMar.Notifications
{
    /// <summary>
    /// A type contained by the tile and toast notification content objects that
    /// represents a text field in the template.
    /// </summary>
    public interface INotificationContentText
    {
        /// <summary>
        /// The text value that will be shown in the text field.
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// The language of the text field.  This property overrides the language provided in the
        /// containing notification object.  The language should be specified using the
        /// abbreviated language code as defined by BCP 47.
        /// </summary>
        string Lang { get; set; }
    }

    /// <summary>
    /// Internal class
    /// </summary>
    internal sealed class NotificationContentText : INotificationContentText
    {
        public string Text { get; set; }
        public string Lang { get; set; }
    }
}