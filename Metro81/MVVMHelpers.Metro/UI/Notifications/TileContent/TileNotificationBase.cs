using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace JulMar.Windows.UI.Notifications.TileContent
{
    internal abstract class TileNotificationBase : NotificationBase
    {
        protected TileNotificationBase(string templateName, int imageCount, int textCount) : base(templateName, imageCount, textCount)
        {
            Branding = TileBranding.Logo;
        }

        public TileBranding Branding { get; set; }

        public TileNotification CreateNotification()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(GetContent());
            return new TileNotification(xmlDoc);
        }
    }
}