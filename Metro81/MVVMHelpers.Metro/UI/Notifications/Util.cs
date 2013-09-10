namespace JulMar.Windows.UI.Notifications
{
    internal static class Util
    {
        public const int NotificationContentVersion = 1;

        public static string HttpEncode(string value)
        {
            return value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
        }
    }
}