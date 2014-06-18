using Windows.UI.Notifications;

namespace JulMar.Notifications.BadgeContent
{
    /// <summary>
    /// Base badge notification content interface.
    /// </summary>
    public interface IBadgeNotificationContent : INotificationContent
    {
        /// <summary>
        /// Creates a WinRT BadgeNotification object based on the content.
        /// </summary>
        /// <returns>A WinRT BadgeNotification object based on the content.</returns>
        BadgeNotification CreateNotification();
    }
}
