using System;
using System.Text;
using Windows.Data.Xml.Dom;

namespace JulMar.Windows.UI.Notifications
{
    /// <summary>
    /// Base class for the notification content creation helper classes.
    /// </summary>
    internal abstract class NotificationBase
    {
        private string _baseUri;

        protected NotificationBase(string templateName, int imageCount, int textCount)
        {
            StrictValidation = true;
            TemplateName = templateName;

            Images = new NotificationContentImage[imageCount];
            for (int i = 0; i < Images.Length; i++)
            {
                Images[i] = new NotificationContentImage();
            }

            TextFields = new INotificationContentText[textCount];
            for (int i = 0; i < TextFields.Length; i++)
            {
                TextFields[i] = new NotificationContentText();
            }
        }

        public string TemplateName { get; private set; }
        public bool StrictValidation { get; set; }

        public abstract string GetContent();

        public override string ToString()
        {
            return GetContent();
        }

        public XmlDocument GetXml()
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(GetContent());
            return xml;
        }

        /// <summary>
        /// Retrieves the list of images that can be manipulated on the notification content.
        /// </summary>
        public NotificationContentImage[] Images { get; private set; }

        /// <summary>
        /// Retrieves the list of text fields that can be manipulated on the notification content.
        /// </summary>
        public INotificationContentText[] TextFields { get; private set; }

        /// <summary>
        /// The base Uri path that should be used for all image references in the notification.
        /// </summary>
        public string BaseUri
        {
            get { return _baseUri; }
            set
            {
                if (this.StrictValidation && !String.IsNullOrEmpty(value))
                {
                    Uri uri;
                    try
                    {
                        uri = new Uri(value);
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException("Invalid URI. Use a valid URI or turn off StrictValidation", e);
                    }
                    if (!(uri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase)
                          || uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase)
                          || uri.Scheme.Equals("ms-appx", StringComparison.OrdinalIgnoreCase)
                          || (uri.Scheme.Equals("ms-appdata", StringComparison.OrdinalIgnoreCase)
                              && (String.IsNullOrEmpty(uri.Authority)) // check to make sure the Uri isn't ms-appdata://foo/local
                              && (uri.AbsolutePath.StartsWith("/local/") || uri.AbsolutePath.StartsWith("local/"))))) // both ms-appdata:local/ and ms-appdata:/local/ are valid
                    {
                        throw new ArgumentException("The BaseUri must begin with http://, https://, ms-appx:///, or ms-appdata:///local/.", "value");
                    }
                }
                _baseUri = value;
            }
        }

        public string Lang { get; set; }

        public bool AddImageQuery
        {
            get { return AddImageQueryNullable != null && AddImageQueryNullable.Value; }
            set { AddImageQueryNullable = value; }
        }

        public bool? AddImageQueryNullable { get; set; }

        protected string SerializeProperties(string globalLang, string globalBaseUri, bool globalAddImageQuery)
        {
            globalLang = globalLang ?? String.Empty;
            globalBaseUri = String.IsNullOrWhiteSpace(globalBaseUri) ? null : globalBaseUri;

            StringBuilder builder = new StringBuilder(String.Empty);
            for (int i = 0; i < Images.Length; i++)
            {
                if (!String.IsNullOrEmpty(Images[i].Src))
                {
                    string escapedSrc = Util.HttpEncode(Images[i].Src);
                    if (!String.IsNullOrWhiteSpace(Images[i].Alt))
                    {
                        string escapedAlt = Util.HttpEncode(Images[i].Alt);
                        if (Images[i].AddImageQueryNullable == null || Images[i].AddImageQueryNullable == globalAddImageQuery)
                        {
                            builder.AppendFormat("<image id='{0}' src='{1}' alt='{2}'/>", i + 1, escapedSrc, escapedAlt);
                        }
                        else
                        {
                            builder.AppendFormat("<image addImageQuery='{0}' id='{1}' src='{2}' alt='{3}'/>", Images[i].AddImageQuery.ToString().ToLowerInvariant(), i + 1, escapedSrc, escapedAlt);
                        }
                    }
                    else
                    {
                        if (Images[i].AddImageQueryNullable == null || Images[i].AddImageQueryNullable == globalAddImageQuery)
                        {
                            builder.AppendFormat("<image id='{0}' src='{1}'/>", i + 1, escapedSrc);
                        }
                        else
                        {
                            builder.AppendFormat("<image addImageQuery='{0}' id='{1}' src='{2}'/>", Images[i].AddImageQuery.ToString().ToLowerInvariant(), i + 1, escapedSrc);
                        }
                    }
                }
            }

            for (int i = 0; i < TextFields.Length; i++)
            {
                if (!String.IsNullOrWhiteSpace(TextFields[i].Text))
                {
                    string escapedValue = Util.HttpEncode(TextFields[i].Text);
                    if (!String.IsNullOrWhiteSpace(TextFields[i].Lang) && !TextFields[i].Lang.Equals(globalLang))
                    {
                        string escapedLang = Util.HttpEncode(TextFields[i].Lang);
                        builder.AppendFormat("<text id='{0}' lang='{1}'>{2}</text>", i + 1, escapedLang, escapedValue);
                    }
                    else
                    {
                        builder.AppendFormat("<text id='{0}'>{1}</text>", i + 1, escapedValue);
                    }
                }
            }

            return builder.ToString();
        }
    }
}