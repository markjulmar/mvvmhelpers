using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace JulMar.Notifications.TileContent
{
    internal abstract class TileNotificationBase : NotificationBase
    {
        protected TileNotificationBase(string templateName, int imageCount, int textCount) : base(templateName, imageCount, textCount)
        {
            this.Branding = TileBranding.Logo;
        }

        public TileBranding Branding { get; set; }

        public TileNotification CreateNotification()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(this.GetContent());
            return new TileNotification(xmlDoc);
        }
    }
}