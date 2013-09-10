using System;
using System.Text;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace JulMar.Windows.UI.Notifications.ToastContent
{
    internal class ToastNotificationBase : NotificationBase, IToastNotificationContent
    {
        public ToastNotificationBase(string templateName, int imageCount, int textCount) : base(templateName, imageCount, textCount)
        {
            Audio = new ToastAudio();
            Duration = ToastDuration.Short;
        }

        private bool AudioSrcIsLooping()
        {
            return (Audio.Content == ToastAudioContent.LoopingAlarm) || (Audio.Content == ToastAudioContent.LoopingAlarm2) ||
                   (Audio.Content == ToastAudioContent.LoopingCall) || (Audio.Content == ToastAudioContent.LoopingCall2);
        }

        private void ValidateAudio()
        {
            if (StrictValidation)
            {
                if (Audio.Loop && Duration != ToastDuration.Long)
                {
                    throw new NotificationContentValidationException("Looping audio is only available for long duration toasts.");
                }
                if (Audio.Loop && !AudioSrcIsLooping())
                {
                    throw new NotificationContentValidationException(
                        "A looping audio src must be chosen if the looping audio property is set.");
                }
                if (!Audio.Loop && AudioSrcIsLooping())
                {
                    throw new NotificationContentValidationException(
                        "The looping audio property needs to be set if a looping audio src is chosen.");
                }
            }
        }

        private void AppendAudioTag(StringBuilder builder)
        {
            if (Audio.Content != ToastAudioContent.Default)
            {
                builder.Append("<audio");
                if (Audio.Content == ToastAudioContent.Silent)
                {
                    builder.Append(" silent='true'/>");
                }
                else
                {
                    if (Audio.Loop == true)
                    {
                        builder.Append(" loop='true'");
                    }

                    // The default looping sound is LoopingCall - save size by not adding it
                    if (Audio.Content != ToastAudioContent.LoopingCall)
                    {
                        string audioSrc = null;
                        switch (Audio.Content)
                        {
                            case ToastAudioContent.IM:
                                audioSrc = "ms-winsoundevent:Notification.IM";
                                break;
                            case ToastAudioContent.Mail:
                                audioSrc = "ms-winsoundevent:Notification.Mail";
                                break;
                            case ToastAudioContent.Reminder:
                                audioSrc = "ms-winsoundevent:Notification.Reminder";
                                break;
                            case ToastAudioContent.SMS:
                                audioSrc = "ms-winsoundevent:Notification.SMS";
                                break;
                            case ToastAudioContent.LoopingAlarm:
                                audioSrc = "ms-winsoundevent:Notification.Looping.Alarm";
                                break;
                            case ToastAudioContent.LoopingAlarm2:
                                audioSrc = "ms-winsoundevent:Notification.Looping.Alarm2";
                                break;
                            case ToastAudioContent.LoopingCall:
                                audioSrc = "ms-winsoundevent:Notification.Looping.Call";
                                break;
                            case ToastAudioContent.LoopingCall2:
                                audioSrc = "ms-winsoundevent:Notification.Looping.Call2";
                                break;
                        }
                        builder.AppendFormat(" src='{0}'", audioSrc);
                    }
                }
                builder.Append("/>");
            }
        }

        public override string GetContent()
        {
            ValidateAudio();

            StringBuilder builder = new StringBuilder("<toast");
            if (!String.IsNullOrEmpty(Launch))
            {
                builder.AppendFormat(" launch='{0}'", Util.HttpEncode(Launch));
            }
            if (Duration != ToastDuration.Short)
            {
                builder.AppendFormat(" duration='{0}'", Duration.ToString().ToLowerInvariant());
            }
            builder.Append(">");

            builder.AppendFormat("<visual version='{0}'", Util.NotificationContentVersion);
            if (!String.IsNullOrWhiteSpace(Lang))
            {
                builder.AppendFormat(" lang='{0}'", Util.HttpEncode(Lang));
            }
            if (!String.IsNullOrWhiteSpace(BaseUri))
            {
                builder.AppendFormat(" baseUri='{0}'", Util.HttpEncode(BaseUri));
            }
            if (AddImageQuery)
            {
                builder.AppendFormat(" addImageQuery='true'");
            }
            builder.Append(">");
            
            builder.AppendFormat("<binding template='{0}'>{1}</binding>", TemplateName, SerializeProperties(Lang, BaseUri, AddImageQuery));
            builder.Append("</visual>");

            AppendAudioTag(builder);
            
            builder.Append("</toast>");

            return builder.ToString();
        }
        
        public ToastNotification CreateNotification()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(GetContent());
            return new ToastNotification(xmlDoc);
        }

        public string FromPackage(string name)
        {
            return name.StartsWith("/") ? "ms-appx://" + name : "ms-appx:///" + name;
        }

        public IToastAudio Audio { get; private set; }
        public string Launch { get; set; }
        public ToastDuration Duration { get; set; }
    }
}