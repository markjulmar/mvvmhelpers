using System;
using System.Text;

using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace JulMar.Notifications.ToastContent
{
    internal class ToastNotificationBase : NotificationBase, IToastNotificationContent
    {
        public ToastNotificationBase(string templateName, int imageCount, int textCount) : base(templateName, imageCount, textCount)
        {
            this.Audio = new ToastAudio();
            this.Duration = ToastDuration.Short;
        }

        private bool AudioSrcIsLooping()
        {
            return (this.Audio.Content == ToastAudioContent.LoopingAlarm) || (this.Audio.Content == ToastAudioContent.LoopingAlarm2) ||
                   (this.Audio.Content == ToastAudioContent.LoopingCall) || (this.Audio.Content == ToastAudioContent.LoopingCall2);
        }

        private void ValidateAudio()
        {
            if (this.StrictValidation)
            {
                if (this.Audio.Loop && this.Duration != ToastDuration.Long)
                {
                    throw new NotificationContentValidationException("Looping audio is only available for long duration toasts.");
                }
                if (this.Audio.Loop && !this.AudioSrcIsLooping())
                {
                    throw new NotificationContentValidationException(
                        "A looping audio src must be chosen if the looping audio property is set.");
                }
                if (!this.Audio.Loop && this.AudioSrcIsLooping())
                {
                    throw new NotificationContentValidationException(
                        "The looping audio property needs to be set if a looping audio src is chosen.");
                }
            }
        }

        private void AppendAudioTag(StringBuilder builder)
        {
            if (this.Audio.Content != ToastAudioContent.Default)
            {
                builder.Append("<audio");
                if (this.Audio.Content == ToastAudioContent.Silent)
                {
                    builder.Append(" silent='true'/>");
                }
                else
                {
                    if (this.Audio.Loop == true)
                    {
                        builder.Append(" loop='true'");
                    }

                    // The default looping sound is LoopingCall - save size by not adding it
                    if (this.Audio.Content != ToastAudioContent.LoopingCall)
                    {
                        string audioSrc = null;
                        switch (this.Audio.Content)
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
            this.ValidateAudio();

            StringBuilder builder = new StringBuilder("<toast");
            if (!String.IsNullOrEmpty(this.Launch))
            {
                builder.AppendFormat(" launch='{0}'", Util.HttpEncode(this.Launch));
            }
            if (this.Duration != ToastDuration.Short)
            {
                builder.AppendFormat(" duration='{0}'", this.Duration.ToString().ToLowerInvariant());
            }
            builder.Append(">");

            builder.AppendFormat("<visual version='{0}'", Util.NotificationContentVersion);
            if (!String.IsNullOrWhiteSpace(this.Lang))
            {
                builder.AppendFormat(" lang='{0}'", Util.HttpEncode(this.Lang));
            }
            if (!String.IsNullOrWhiteSpace(this.BaseUri))
            {
                builder.AppendFormat(" baseUri='{0}'", Util.HttpEncode(this.BaseUri));
            }
            if (this.AddImageQuery)
            {
                builder.AppendFormat(" addImageQuery='true'");
            }
            builder.Append(">");
            
            builder.AppendFormat("<binding template='{0}'>{1}</binding>", this.TemplateName, this.SerializeProperties(this.Lang, this.BaseUri, this.AddImageQuery));
            builder.Append("</visual>");

            this.AppendAudioTag(builder);
            
            builder.Append("</toast>");

            return builder.ToString();
        }
        
        public ToastNotification CreateNotification()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(this.GetContent());
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