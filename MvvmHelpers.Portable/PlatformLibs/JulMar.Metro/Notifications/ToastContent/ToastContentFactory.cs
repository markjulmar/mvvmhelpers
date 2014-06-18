namespace JulMar.Notifications.ToastContent
{
    /// <summary>
    /// A factory which creates toast content objects for all of the toast template types.
    /// </summary>
    public static class ToastContentFactory
    {
        /// <summary>
        /// Creates a ToastImageAndText01 template content object.
        /// </summary>
        /// <returns>A ToastImageAndText01 template content object.</returns>
        public static IToastImageAndText01 CreateToastImageAndText01()
        {
            return new ToastImageAndText01();
        }

        /// <summary>
        /// Creates a ToastImageAndText02 template content object.
        /// </summary>
        /// <returns>A ToastImageAndText02 template content object.</returns>
        public static IToastImageAndText02 CreateToastImageAndText02()
        {
            return new ToastImageAndText02();
        }

        /// <summary>
        /// Creates a ToastImageAndText03 template content object.
        /// </summary>
        /// <returns>A ToastImageAndText03 template content object.</returns>
        public static IToastImageAndText03 CreateToastImageAndText03()
        {
            return new ToastImageAndText03();
        }

        /// <summary>
        /// Creates a ToastImageAndText04 template content object.
        /// </summary>
        /// <returns>A ToastImageAndText04 template content object.</returns>
        public static IToastImageAndText04 CreateToastImageAndText04()
        {
            return new ToastImageAndText04();
        }

        /// <summary>
        /// Creates a ToastText01 template content object.
        /// </summary>
        /// <returns>A ToastText01 template content object.</returns>
        public static IToastText01 CreateToastText01()
        {
            return new ToastText01();
        }

        /// <summary>
        /// Creates a ToastText02 template content object.
        /// </summary>
        /// <returns>A ToastText02 template content object.</returns>
        public static IToastText02 CreateToastText02()
        {
            return new ToastText02();
        }

        /// <summary>
        /// Creates a ToastText03 template content object.
        /// </summary>
        /// <returns>A ToastText03 template content object.</returns>
        public static IToastText03 CreateToastText03()
        {
            return new ToastText03();
        }

        /// <summary>
        /// Creates a ToastText04 template content object.
        /// </summary>
        /// <returns>A ToastText04 template content object.</returns>
        public static IToastText04 CreateToastText04()
        {
            return new ToastText04();
        }
    }
}