namespace JulMar.Notifications.ToastContent
{
    internal class ToastImageAndText01 : ToastNotificationBase, IToastImageAndText01
    {
        public ToastImageAndText01() : base(templateName: "ToastImageAndText01", imageCount: 1, textCount: 1)
        {
        }

        public INotificationContentImage Image { get { return this.Images[0]; } }

        public INotificationContentText TextBodyWrap { get { return this.TextFields[0]; } }
    }

    internal class ToastImageAndText02 : ToastNotificationBase, IToastImageAndText02
    {
        public ToastImageAndText02() : base(templateName: "ToastImageAndText02", imageCount: 1, textCount: 2)
        {
        }

        public INotificationContentImage Image { get { return this.Images[0]; } }

        public INotificationContentText TextHeading { get { return this.TextFields[0]; } }
        public INotificationContentText TextBodyWrap { get { return this.TextFields[1]; } }
    }

    internal class ToastImageAndText03 : ToastNotificationBase, IToastImageAndText03
    {
        public ToastImageAndText03() : base(templateName: "ToastImageAndText03", imageCount: 1, textCount: 2)
        {
        }

        public INotificationContentImage Image { get { return this.Images[0]; } }

        public INotificationContentText TextHeadingWrap { get { return this.TextFields[0]; } }
        public INotificationContentText TextBody { get { return this.TextFields[1]; } }
    }

    internal class ToastImageAndText04 : ToastNotificationBase, IToastImageAndText04
    {
        public ToastImageAndText04() : base(templateName: "ToastImageAndText04", imageCount: 1, textCount: 3)
        {
        }

        public INotificationContentImage Image { get { return this.Images[0]; } }

        public INotificationContentText TextHeading { get { return this.TextFields[0]; } }
        public INotificationContentText TextBody1 { get { return this.TextFields[1]; } }
        public INotificationContentText TextBody2 { get { return this.TextFields[2]; } }
    }

    internal class ToastText01 : ToastNotificationBase, IToastText01
    {
        public ToastText01() : base(templateName: "ToastText01", imageCount: 0, textCount: 1)
        {
        }

        public INotificationContentText TextBodyWrap { get { return this.TextFields[0]; } }
    }

    internal class ToastText02 : ToastNotificationBase, IToastText02
    {
        public ToastText02() : base(templateName: "ToastText02", imageCount: 0, textCount: 2)
        {
        }

        public INotificationContentText TextHeading { get { return this.TextFields[0]; } }
        public INotificationContentText TextBodyWrap { get { return this.TextFields[1]; } }
    }

    internal class ToastText03 : ToastNotificationBase, IToastText03
    {
        public ToastText03() : base(templateName: "ToastText03", imageCount: 0, textCount: 2)
        {
        }

        public INotificationContentText TextHeadingWrap { get { return this.TextFields[0]; } }
        public INotificationContentText TextBody { get { return this.TextFields[1]; } }
    }

    internal class ToastText04 : ToastNotificationBase, IToastText04
    {
        public ToastText04() : base(templateName: "ToastText04", imageCount: 0, textCount: 3)
        {
        }

        public INotificationContentText TextHeading { get { return this.TextFields[0]; } }
        public INotificationContentText TextBody1 { get { return this.TextFields[1]; } }
        public INotificationContentText TextBody2 { get { return this.TextFields[2]; } }
    }
}
