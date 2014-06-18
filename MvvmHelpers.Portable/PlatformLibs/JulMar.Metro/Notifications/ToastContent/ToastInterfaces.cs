namespace JulMar.Notifications.ToastContent
{
    /// <summary>
    /// A toast template that displays an image and a text field.
    /// </summary>
    public interface IToastImageAndText01 : IToastNotificationContent
    {
        /// <summary>
        /// The main image on the toast.
        /// </summary>
        INotificationContentImage Image { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBodyWrap { get; }
    }

    /// <summary>
    /// A toast template that displays an image and two text fields.
    /// </summary>
    public interface IToastImageAndText02 : IToastNotificationContent
    {
        /// <summary>
        /// The main image on the toast.
        /// </summary>
        INotificationContentImage Image { get; }

        /// <summary>
        /// A heading text field.
        /// </summary>
        INotificationContentText TextHeading { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBodyWrap { get; }
    }

    /// <summary>
    /// A toast template that displays an image and two text fields.
    /// </summary>
    public interface IToastImageAndText03 : IToastNotificationContent
    {
        /// <summary>
        /// The main image on the toast.
        /// </summary>
        INotificationContentImage Image { get; }

        /// <summary>
        /// A heading text field.
        /// </summary>
        INotificationContentText TextHeadingWrap { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody { get; }
    }

    /// <summary>
    /// A toast template that displays an image and three text fields.
    /// </summary>
    public interface IToastImageAndText04 : IToastNotificationContent
    {
        /// <summary>
        /// The main image on the toast.
        /// </summary>
        INotificationContentImage Image { get; }

        /// <summary>
        /// A heading text field.
        /// </summary>
        INotificationContentText TextHeading { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody1 { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody2 { get; }
    }

    /// <summary>
    /// A toast template that displays a text fields.
    /// </summary>
    public interface IToastText01 : IToastNotificationContent
    {
        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBodyWrap { get; }
    }

    /// <summary>
    /// A toast template that displays two text fields.
    /// </summary>
    public interface IToastText02 : IToastNotificationContent
    {
        /// <summary>
        /// A heading text field.
        /// </summary>
        INotificationContentText TextHeading { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBodyWrap { get; }
    }

    /// <summary>
    /// A toast template that displays two text fields.
    /// </summary>
    public interface IToastText03 : IToastNotificationContent
    {
        /// <summary>
        /// A heading text field.
        /// </summary>
        INotificationContentText TextHeadingWrap { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody { get; }
    }

    /// <summary>
    /// A toast template that displays three text fields.
    /// </summary>
    public interface IToastText04 : IToastNotificationContent
    {
        /// <summary>
        /// A heading text field.
        /// </summary>
        INotificationContentText TextHeading { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody1 { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody2 { get; }
    }
}
