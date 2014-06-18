using System;
using System.Text;

namespace JulMar.Notifications.TileContent
{
    internal interface ISquareTileInternal
    {
        string SerializeBinding(string globalLang, string globalBaseUri, TileBranding globalBranding, bool globalAddImageQuery);
    }

    internal class TileSquareBase : TileNotificationBase, ISquareTileInternal
    {
        public TileSquareBase(string templateName, int imageCount, int textCount) : base(templateName, imageCount, textCount)
        {
        }               

        public override string GetContent()
        {
            StringBuilder builder = new StringBuilder(String.Empty);
            builder.AppendFormat("<tile><visual version='{0}'", Util.NotificationContentVersion);
            if (!String.IsNullOrWhiteSpace(this.Lang))
            {
                builder.AppendFormat(" lang='{0}'", Util.HttpEncode(this.Lang));
            }
            if (this.Branding != TileBranding.Logo)
            {
                builder.AppendFormat(" branding='{0}'", this.Branding.ToString().ToLowerInvariant());
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
            builder.Append(this.SerializeBinding(this.Lang, this.BaseUri, this.Branding, this.AddImageQuery));
            builder.Append("</visual></tile>");
            return builder.ToString();
        }

        public string SerializeBinding(string globalLang, string globalBaseUri, TileBranding globalBranding, bool globalAddImageQuery)
        {
            StringBuilder bindingNode = new StringBuilder(String.Empty);
            bindingNode.AppendFormat("<binding template='{0}'", this.TemplateName);
            if (!String.IsNullOrWhiteSpace(this.Lang) && !this.Lang.Equals(globalLang))
            {
                bindingNode.AppendFormat(" lang='{0}'", Util.HttpEncode(this.Lang));
                globalLang = this.Lang;
            }
            if (this.Branding != TileBranding.Logo && this.Branding != globalBranding)
            {
                bindingNode.AppendFormat(" branding='{0}'", this.Branding.ToString().ToLowerInvariant());
            }
            if (!String.IsNullOrWhiteSpace(this.BaseUri) && !this.BaseUri.Equals(globalBaseUri))
            {
                bindingNode.AppendFormat(" baseUri='{0}'", Util.HttpEncode(this.BaseUri));
                globalBaseUri = this.BaseUri;
            }
            if (this.AddImageQueryNullable != null && this.AddImageQueryNullable != globalAddImageQuery)
            {
                bindingNode.AppendFormat(" addImageQuery='{0}'", this.AddImageQuery.ToString().ToLowerInvariant());
                globalAddImageQuery = this.AddImageQuery;
            }           
            bindingNode.AppendFormat(">{0}</binding>", this.SerializeProperties(globalLang, globalBaseUri, globalAddImageQuery));

            return bindingNode.ToString();
        }
    }

    internal class TileWideBase : TileNotificationBase
    {
        public TileWideBase(string templateName, int imageCount, int textCount) : base(templateName, imageCount, textCount)
        {
        }

        public ISquareTileNotificationContent SquareContent
        {
            get { return this.m_SquareContent; }
            set { this.m_SquareContent = value; }
        }

        public bool RequireSquareContent
        {
            get { return this.m_RequireSquareContent; }
            set { this.m_RequireSquareContent = value; }
        }

        public override string GetContent()
        {
            if (this.RequireSquareContent && this.SquareContent == null)
            {
                throw new NotificationContentValidationException(
                    "Square tile content should be included with each wide tile. " +
                    "If this behavior is undesired, use the RequireSquareContent property.");
            }

            StringBuilder visualNode = new StringBuilder(String.Empty);
            visualNode.AppendFormat("<visual version='{0}'", Util.NotificationContentVersion);
            if (!String.IsNullOrWhiteSpace(this.Lang))
            {
                visualNode.AppendFormat(" lang='{0}'", Util.HttpEncode(this.Lang));
            }
            if (this.Branding != TileBranding.Logo)
            {
                visualNode.AppendFormat(" branding='{0}'", this.Branding.ToString().ToLowerInvariant());
            }
            if (!String.IsNullOrWhiteSpace(this.BaseUri))
            {
                visualNode.AppendFormat(" baseUri='{0}'", Util.HttpEncode(this.BaseUri));
            }
            if (this.AddImageQuery)
            {
                visualNode.AppendFormat(" addImageQuery='true'");
            }
            visualNode.Append(">");

            StringBuilder builder = new StringBuilder(String.Empty);
            builder.AppendFormat("<tile>{0}<binding template='{1}'>{2}</binding>", visualNode, this.TemplateName, this.SerializeProperties(this.Lang, this.BaseUri, this.AddImageQuery));
            if (this.SquareContent != null)
            {
                ISquareTileInternal squareBase = this.SquareContent as ISquareTileInternal;
                if (squareBase == null)
                {
                    throw new NotificationContentValidationException("The provided square tile content class is unsupported.");
                }
                builder.Append((string) squareBase.SerializeBinding(this.Lang, this.BaseUri, this.Branding, this.AddImageQuery));
            }
            builder.Append("</visual></tile>");

            return builder.ToString();
        }

        private ISquareTileNotificationContent m_SquareContent = null;
        private bool m_RequireSquareContent = true;
    }

    internal class TileSquareBlock : TileSquareBase, ITileSquareBlock
    {
        public TileSquareBlock() : base(templateName: "TileSquareBlock", imageCount: 0, textCount: 2)
        {
        }

        public INotificationContentText TextBlock { get { return this.TextFields[0]; } }
        public INotificationContentText TextSubBlock { get { return this.TextFields[1]; } }
    }

    internal class TileSquareImage : TileSquareBase, ITileSquareImage
    {
        public TileSquareImage() : base(templateName: "TileSquareImage", imageCount: 1, textCount: 0)
        {
        }

        public INotificationContentImage Image { get { return this.Images[0]; } }
    }

    internal class TileSquarePeekImageAndText01 : TileSquareBase, ITileSquarePeekImageAndText01
    {
        public TileSquarePeekImageAndText01() : base(templateName: "TileSquarePeekImageAndText01", imageCount: 1, textCount: 4)
        {
        }

        public INotificationContentImage Image { get { return this.Images[0]; } }
        
        public INotificationContentText TextHeading { get { return this.TextFields[0]; } }
        public INotificationContentText TextBody1 { get { return this.TextFields[1]; } }
        public INotificationContentText TextBody2 { get { return this.TextFields[2]; } }
        public INotificationContentText TextBody3 { get { return this.TextFields[3]; } }
    }

    internal class TileSquarePeekImageAndText02 : TileSquareBase, ITileSquarePeekImageAndText02
    {
        public TileSquarePeekImageAndText02() : base(templateName: "TileSquarePeekImageAndText02", imageCount: 1, textCount: 2)
        {
        }

        public INotificationContentImage Image { get { return this.Images[0]; } }

        public INotificationContentText TextHeading { get { return this.TextFields[0]; } }
        public INotificationContentText TextBodyWrap { get { return this.TextFields[1]; } }
    }

    internal class TileSquarePeekImageAndText03 : TileSquareBase, ITileSquarePeekImageAndText03
    {
        public TileSquarePeekImageAndText03() : base(templateName: "TileSquarePeekImageAndText03", imageCount: 1, textCount: 4)
        {
        }

        public INotificationContentImage Image { get { return this.Images[0]; } }

        public INotificationContentText TextBody1 { get { return this.TextFields[0]; } }
        public INotificationContentText TextBody2 { get { return this.TextFields[1]; } }
        public INotificationContentText TextBody3 { get { return this.TextFields[2]; } }
        public INotificationContentText TextBody4 { get { return this.TextFields[3]; } }
    }

    internal class TileSquarePeekImageAndText04 : TileSquareBase, ITileSquarePeekImageAndText04
    {
        public TileSquarePeekImageAndText04() : base(templateName: "TileSquarePeekImageAndText04", imageCount: 1, textCount: 1)
        {
        }

        public INotificationContentImage Image { get { return this.Images[0]; } }

        public INotificationContentText TextBodyWrap { get { return this.TextFields[0]; } }
    }

    internal class TileSquareText01 : TileSquareBase, ITileSquareText01
    {
        public TileSquareText01() : base(templateName: "TileSquareText01", imageCount: 0, textCount: 4)
        {
        }

        public INotificationContentText TextHeading { get { return this.TextFields[0]; } }
        public INotificationContentText TextBody1 { get { return this.TextFields[1]; } }
        public INotificationContentText TextBody2 { get { return this.TextFields[2]; } }
        public INotificationContentText TextBody3 { get { return this.TextFields[3]; } }
    }

    internal class TileSquareText02 : TileSquareBase, ITileSquareText02
    {
        public TileSquareText02() : base(templateName: "TileSquareText02", imageCount: 0, textCount: 2)
        {
        }

        public INotificationContentText TextHeading { get { return this.TextFields[0]; } }
        public INotificationContentText TextBodyWrap { get { return this.TextFields[1]; } }
    }

    internal class TileSquareText03 : TileSquareBase, ITileSquareText03
    {
        public TileSquareText03() : base(templateName: "TileSquareText03", imageCount: 0, textCount: 4) 
        {
        }

        public INotificationContentText TextBody1 { get { return this.TextFields[0]; } }
        public INotificationContentText TextBody2 { get { return this.TextFields[1]; } }
        public INotificationContentText TextBody3 { get { return this.TextFields[2]; } }
        public INotificationContentText TextBody4 { get { return this.TextFields[3]; } }
    }

    internal class TileSquareText04 : TileSquareBase, ITileSquareText04
    {
        public TileSquareText04() : base(templateName: "TileSquareText04", imageCount: 0, textCount: 1)
        {
        }

        public INotificationContentText TextBodyWrap { get { return this.TextFields[0]; } }
    }

    internal class TileWideBlockAndText01 : TileWideBase, ITileWideBlockAndText01
    {
        public TileWideBlockAndText01() : base(templateName: "TileWideBlockAndText01", imageCount: 0, textCount: 6)
        {
        }

        public INotificationContentText TextBody1 { get { return this.TextFields[0]; } }
        public INotificationContentText TextBody2 { get { return this.TextFields[1]; } }
        public INotificationContentText TextBody3 { get { return this.TextFields[2]; } }
        public INotificationContentText TextBody4 { get { return this.TextFields[3]; } }
        public INotificationContentText TextBlock { get { return this.TextFields[4]; } }
        public INotificationContentText TextSubBlock { get { return this.TextFields[5]; } }
    }

    internal class TileWideBlockAndText02 : TileWideBase, ITileWideBlockAndText02
    {
        public TileWideBlockAndText02() : base(templateName: "TileWideBlockAndText02", imageCount: 0, textCount: 6)
        {
        }

        public INotificationContentText TextBodyWrap { get { return this.TextFields[0]; } }
        public INotificationContentText TextBlock { get { return this.TextFields[1]; } }
        public INotificationContentText TextSubBlock { get { return this.TextFields[2]; } }
    }

    internal class TileWideImage : TileWideBase, ITileWideImage
    {
        public TileWideImage() : base(templateName: "TileWideImage", imageCount: 1, textCount: 0)
        {
        }

        public INotificationContentImage Image { get { return this.Images[0]; } }
    }

    internal class TileWideImageAndText01 : TileWideBase, ITileWideImageAndText01
    {
        public TileWideImageAndText01() : base(templateName: "TileWideImageAndText01", imageCount: 1, textCount: 1)
        {
        }

        public INotificationContentImage Image { get { return this.Images[0]; } }

        public INotificationContentText TextCaptionWrap { get { return this.TextFields[0]; } }
    }

    internal class TileWideImageAndText02 : TileWideBase, ITileWideImageAndText02
    {
        public TileWideImageAndText02() : base(templateName: "TileWideImageAndText02", imageCount: 1, textCount: 2)
        {
        }

        public INotificationContentImage Image { get { return this.Images[0]; } }

        public INotificationContentText TextCaption1 { get { return this.TextFields[0]; } }
        public INotificationContentText TextCaption2 { get { return this.TextFields[1]; } }
    }

    internal class TileWideImageCollection : TileWideBase, ITileWideImageCollection
    {
        public TileWideImageCollection() : base(templateName: "TileWideImageCollection", imageCount: 5, textCount: 0)
        {
        }

        public INotificationContentImage ImageMain { get { return this.Images[0]; } }
        public INotificationContentImage ImageSmallColumn1Row1 { get { return this.Images[1]; } }
        public INotificationContentImage ImageSmallColumn2Row1 { get { return this.Images[2]; } }
        public INotificationContentImage ImageSmallColumn1Row2 { get { return this.Images[3]; } }
        public INotificationContentImage ImageSmallColumn2Row2 { get { return this.Images[4]; } }
    }

    internal class TileWidePeekImage01 : TileWideBase, ITileWidePeekImage01
    {
        public TileWidePeekImage01() : base(templateName: "TileWidePeekImage01", imageCount: 1, textCount: 2)
        {
        }

        public INotificationContentImage Image { get { return this.Images[0]; } }

        public INotificationContentText TextHeading { get { return this.TextFields[0]; } }
        public INotificationContentText TextBodyWrap { get { return this.TextFields[1]; } }
    }

    internal class TileWidePeekImage02 : TileWideBase, ITileWidePeekImage02
    {
        public TileWidePeekImage02() : base(templateName: "TileWidePeekImage02", imageCount: 1, textCount: 5)
        {
        }

        public INotificationContentImage Image { get { return this.Images[0]; } }

        public INotificationContentText TextHeading { get { return this.TextFields[0]; } }
        public INotificationContentText TextBody1 { get { return this.TextFields[1]; } }
        public INotificationContentText TextBody2 { get { return this.TextFields[2]; } }
        public INotificationContentText TextBody3 { get { return this.TextFields[3]; } }
        public INotificationContentText TextBody4 { get { return this.TextFields[4]; } }
    }

    internal class TileWidePeekImage03 : TileWideBase, ITileWidePeekImage03
    {
        public TileWidePeekImage03() : base(templateName: "TileWidePeekImage03", imageCount: 1, textCount: 1)
        {
        }

        public INotificationContentImage Image { get { return this.Images[0]; } }

        public INotificationContentText TextHeadingWrap { get { return this.TextFields[0]; } }
    }

    internal class TileWidePeekImage04 : TileWideBase, ITileWidePeekImage04
    {
        public TileWidePeekImage04() : base(templateName: "TileWidePeekImage04", imageCount: 1, textCount: 1)
        {
        }

        public INotificationContentImage Image { get { return this.Images[0]; } }

        public INotificationContentText TextBodyWrap { get { return this.TextFields[0]; } }
    }

    internal class TileWidePeekImage05 : TileWideBase, ITileWidePeekImage05
    {
        public TileWidePeekImage05() : base(templateName: "TileWidePeekImage05", imageCount: 2, textCount: 2)
        {
        }

        public INotificationContentImage ImageMain { get { return this.Images[0]; } }
        public INotificationContentImage ImageSecondary { get { return this.Images[1]; } }

        public INotificationContentText TextHeading { get { return this.TextFields[0]; } }
        public INotificationContentText TextBodyWrap { get { return this.TextFields[1]; } }
    }

    internal class TileWidePeekImage06 : TileWideBase, ITileWidePeekImage06
    {
        public TileWidePeekImage06() : base(templateName: "TileWidePeekImage06", imageCount: 2, textCount: 1)
        {
        }

        public INotificationContentImage ImageMain { get { return this.Images[0]; } }
        public INotificationContentImage ImageSecondary { get { return this.Images[1]; } }

        public INotificationContentText TextHeadingWrap { get { return this.TextFields[0]; } }
    }

    internal class TileWidePeekImageAndText01 : TileWideBase, ITileWidePeekImageAndText01
    {
        public TileWidePeekImageAndText01() : base(templateName: "TileWidePeekImageAndText01", imageCount: 1, textCount: 1) 
        {
        }

        public INotificationContentImage Image { get { return this.Images[0]; } }

        public INotificationContentText TextBodyWrap { get { return this.TextFields[0]; } }
    }

    internal class TileWidePeekImageAndText02 : TileWideBase, ITileWidePeekImageAndText02
    {
        public TileWidePeekImageAndText02() : base(templateName: "TileWidePeekImageAndText02", imageCount: 1, textCount: 5) 
        {
        }

        public INotificationContentImage Image { get { return this.Images[0]; } }

        public INotificationContentText TextBody1 { get { return this.TextFields[0]; } }
        public INotificationContentText TextBody2 { get { return this.TextFields[1]; } }
        public INotificationContentText TextBody3 { get { return this.TextFields[2]; } }
        public INotificationContentText TextBody4 { get { return this.TextFields[3]; } }
        public INotificationContentText TextBody5 { get { return this.TextFields[4]; } }
    }

    internal class TileWidePeekImageCollection01 : TileWideBase, ITileWidePeekImageCollection01
    {
        public TileWidePeekImageCollection01() : base(templateName: "TileWidePeekImageCollection01", imageCount: 5, textCount: 2)
        {
        }

        public INotificationContentImage ImageMain { get { return this.Images[0]; } }
        public INotificationContentImage ImageSmallColumn1Row1 { get { return this.Images[1]; } }
        public INotificationContentImage ImageSmallColumn2Row1 { get { return this.Images[2]; } }
        public INotificationContentImage ImageSmallColumn1Row2 { get { return this.Images[3]; } }
        public INotificationContentImage ImageSmallColumn2Row2 { get { return this.Images[4]; } }

        public INotificationContentText TextHeading { get { return this.TextFields[0]; } }
        public INotificationContentText TextBodyWrap { get { return this.TextFields[1]; } }
    }

    internal class TileWidePeekImageCollection02 : TileWideBase, ITileWidePeekImageCollection02
    {
        public TileWidePeekImageCollection02() : base(templateName: "TileWidePeekImageCollection02", imageCount: 5, textCount: 5)
        {
        }

        public INotificationContentImage ImageMain { get { return this.Images[0]; } }
        public INotificationContentImage ImageSmallColumn1Row1 { get { return this.Images[1]; } }
        public INotificationContentImage ImageSmallColumn2Row1 { get { return this.Images[2]; } }
        public INotificationContentImage ImageSmallColumn1Row2 { get { return this.Images[3]; } }
        public INotificationContentImage ImageSmallColumn2Row2 { get { return this.Images[4]; } }

        public INotificationContentText TextHeading { get { return this.TextFields[0]; } }
        public INotificationContentText TextBody1 { get { return this.TextFields[1]; } }
        public INotificationContentText TextBody2 { get { return this.TextFields[2]; } }
        public INotificationContentText TextBody3 { get { return this.TextFields[3]; } }
        public INotificationContentText TextBody4 { get { return this.TextFields[4]; } }
    }

    internal class TileWidePeekImageCollection03 : TileWideBase, ITileWidePeekImageCollection03
    {
        public TileWidePeekImageCollection03() : base(templateName: "TileWidePeekImageCollection03", imageCount: 5, textCount: 1)
        {
        }

        public INotificationContentImage ImageMain { get { return this.Images[0]; } }
        public INotificationContentImage ImageSmallColumn1Row1 { get { return this.Images[1]; } }
        public INotificationContentImage ImageSmallColumn2Row1 { get { return this.Images[2]; } }
        public INotificationContentImage ImageSmallColumn1Row2 { get { return this.Images[3]; } }
        public INotificationContentImage ImageSmallColumn2Row2 { get { return this.Images[4]; } }

        public INotificationContentText TextHeadingWrap { get { return this.TextFields[0]; } }
    }

    internal class TileWidePeekImageCollection04 : TileWideBase, ITileWidePeekImageCollection04
    {
        public TileWidePeekImageCollection04() : base(templateName: "TileWidePeekImageCollection04", imageCount: 5, textCount: 1)
        {
        }

        public INotificationContentImage ImageMain { get { return this.Images[0]; } }
        public INotificationContentImage ImageSmallColumn1Row1 { get { return this.Images[1]; } }
        public INotificationContentImage ImageSmallColumn2Row1 { get { return this.Images[2]; } }
        public INotificationContentImage ImageSmallColumn1Row2 { get { return this.Images[3]; } }
        public INotificationContentImage ImageSmallColumn2Row2 { get { return this.Images[4]; } }

        public INotificationContentText TextBodyWrap { get { return this.TextFields[0]; } }
    }

    internal class TileWidePeekImageCollection05 : TileWideBase, ITileWidePeekImageCollection05
    {
        public TileWidePeekImageCollection05() : base(templateName: "TileWidePeekImageCollection05", imageCount: 6, textCount: 2)
        {
        }

        public INotificationContentImage ImageMain { get { return this.Images[0]; } }
        public INotificationContentImage ImageSmallColumn1Row1 { get { return this.Images[1]; } }
        public INotificationContentImage ImageSmallColumn2Row1 { get { return this.Images[2]; } }
        public INotificationContentImage ImageSmallColumn1Row2 { get { return this.Images[3]; } }
        public INotificationContentImage ImageSmallColumn2Row2 { get { return this.Images[4]; } }
        public INotificationContentImage ImageSecondary { get { return this.Images[5]; } }

        public INotificationContentText TextHeading { get { return this.TextFields[0]; } }
        public INotificationContentText TextBodyWrap { get { return this.TextFields[1]; } }
    }

    internal class TileWidePeekImageCollection06 : TileWideBase, ITileWidePeekImageCollection06
    {
        public TileWidePeekImageCollection06() : base(templateName: "TileWidePeekImageCollection06", imageCount: 6, textCount: 1)
        {
        }

        public INotificationContentImage ImageMain { get { return this.Images[0]; } }
        public INotificationContentImage ImageSmallColumn1Row1 { get { return this.Images[1]; } }
        public INotificationContentImage ImageSmallColumn2Row1 { get { return this.Images[2]; } }
        public INotificationContentImage ImageSmallColumn1Row2 { get { return this.Images[3]; } }
        public INotificationContentImage ImageSmallColumn2Row2 { get { return this.Images[4]; } }
        public INotificationContentImage ImageSecondary { get { return this.Images[5]; } }

        public INotificationContentText TextHeadingWrap { get { return this.TextFields[0]; } }
    }

    internal class TileWideSmallImageAndText01 : TileWideBase, ITileWideSmallImageAndText01
    {
        public TileWideSmallImageAndText01() : base(templateName: "TileWideSmallImageAndText01", imageCount: 1, textCount: 1)
        {
        }

        public INotificationContentImage Image { get { return this.Images[0]; } }

        public INotificationContentText TextHeadingWrap { get { return this.TextFields[0]; } }
    }
    
    internal class TileWideSmallImageAndText02 : TileWideBase, ITileWideSmallImageAndText02
    {
        public TileWideSmallImageAndText02() : base(templateName: "TileWideSmallImageAndText02", imageCount: 1, textCount: 5)
        {
        }

        public INotificationContentImage Image { get { return this.Images[0]; } }

        public INotificationContentText TextHeading { get { return this.TextFields[0]; } }
        public INotificationContentText TextBody1 { get { return this.TextFields[1]; } }
        public INotificationContentText TextBody2 { get { return this.TextFields[2]; } }
        public INotificationContentText TextBody3 { get { return this.TextFields[3]; } }
        public INotificationContentText TextBody4 { get { return this.TextFields[4]; } }
    }

    internal class TileWideSmallImageAndText03 : TileWideBase, ITileWideSmallImageAndText03
    {
        public TileWideSmallImageAndText03() : base(templateName: "TileWideSmallImageAndText03", imageCount: 1, textCount: 1)
        {
        }

        public INotificationContentImage Image { get { return this.Images[0]; } }

        public INotificationContentText TextBodyWrap { get { return this.TextFields[0]; } }
    }

    internal class TileWideSmallImageAndText04 : TileWideBase, ITileWideSmallImageAndText04
    {
        public TileWideSmallImageAndText04() : base(templateName: "TileWideSmallImageAndText04", imageCount: 1, textCount: 2)
        {
        }

        public INotificationContentImage Image { get { return this.Images[0]; } }

        public INotificationContentText TextHeading { get { return this.TextFields[0]; } }
        public INotificationContentText TextBodyWrap { get { return this.TextFields[1]; } }
    }

    internal class TileWideSmallImageAndText05 : TileWideBase, ITileWideSmallImageAndText05
    {
        public TileWideSmallImageAndText05() : base(templateName: "TileWideSmallImageAndText05", imageCount: 1, textCount: 2)
        {
        }

        public INotificationContentImage Image { get { return this.Images[0]; } }

        public INotificationContentText TextHeading { get { return this.TextFields[0]; } }
        public INotificationContentText TextBodyWrap { get { return this.TextFields[1]; } }
    }

    internal class TileWideText01 : TileWideBase, ITileWideText01
    {
        public TileWideText01() : base(templateName: "TileWideText01", imageCount: 0, textCount: 5)
        {
        }

        public INotificationContentText TextHeading { get { return this.TextFields[0]; } }
        public INotificationContentText TextBody1 { get { return this.TextFields[1]; } }
        public INotificationContentText TextBody2 { get { return this.TextFields[2]; } }
        public INotificationContentText TextBody3 { get { return this.TextFields[3]; } }
        public INotificationContentText TextBody4 { get { return this.TextFields[4]; } }
    }

    internal class TileWideText02 : TileWideBase, ITileWideText02
    {
        public TileWideText02() : base(templateName: "TileWideText02", imageCount: 0, textCount: 9)
        {
        }

        public INotificationContentText TextHeading { get { return this.TextFields[0]; } }
        public INotificationContentText TextColumn1Row1 { get { return this.TextFields[1]; } }
        public INotificationContentText TextColumn2Row1 { get { return this.TextFields[2]; } }
        public INotificationContentText TextColumn1Row2 { get { return this.TextFields[3]; } }
        public INotificationContentText TextColumn2Row2 { get { return this.TextFields[4]; } }
        public INotificationContentText TextColumn1Row3 { get { return this.TextFields[5]; } }
        public INotificationContentText TextColumn2Row3 { get { return this.TextFields[6]; } }
        public INotificationContentText TextColumn1Row4 { get { return this.TextFields[7]; } }
        public INotificationContentText TextColumn2Row4 { get { return this.TextFields[8]; } }
    }

    internal class TileWideText03 : TileWideBase, ITileWideText03
    {
        public TileWideText03() : base(templateName: "TileWideText03", imageCount: 0, textCount: 1)
        {
        }

        public INotificationContentText TextHeadingWrap { get { return this.TextFields[0]; } }
    }

    internal class TileWideText04 : TileWideBase, ITileWideText04
    {
        public TileWideText04() : base(templateName: "TileWideText04", imageCount: 0, textCount: 1)
        {
        }

        public INotificationContentText TextBodyWrap { get { return this.TextFields[0]; } }
    }

    internal class TileWideText05 : TileWideBase, ITileWideText05
    {
        public TileWideText05() : base(templateName: "TileWideText05", imageCount: 0, textCount: 5)
        {
        }

        public INotificationContentText TextBody1 { get { return this.TextFields[0]; } }
        public INotificationContentText TextBody2 { get { return this.TextFields[1]; } }
        public INotificationContentText TextBody3 { get { return this.TextFields[2]; } }
        public INotificationContentText TextBody4 { get { return this.TextFields[3]; } }
        public INotificationContentText TextBody5 { get { return this.TextFields[4]; } }
    }

    internal class TileWideText06 : TileWideBase, ITileWideText06
    {
        public TileWideText06() : base(templateName: "TileWideText06", imageCount: 0, textCount: 10)
        {
        }

        public INotificationContentText TextColumn1Row1 { get { return this.TextFields[0]; } }
        public INotificationContentText TextColumn2Row1 { get { return this.TextFields[1]; } }
        public INotificationContentText TextColumn1Row2 { get { return this.TextFields[2]; } }
        public INotificationContentText TextColumn2Row2 { get { return this.TextFields[3]; } }
        public INotificationContentText TextColumn1Row3 { get { return this.TextFields[4]; } }
        public INotificationContentText TextColumn2Row3 { get { return this.TextFields[5]; } }
        public INotificationContentText TextColumn1Row4 { get { return this.TextFields[6]; } }
        public INotificationContentText TextColumn2Row4 { get { return this.TextFields[7]; } }
        public INotificationContentText TextColumn1Row5 { get { return this.TextFields[8]; } }
        public INotificationContentText TextColumn2Row5 { get { return this.TextFields[9]; } }
    }

    internal class TileWideText07 : TileWideBase, ITileWideText07
    {
        public TileWideText07() : base(templateName: "TileWideText07", imageCount: 0, textCount: 9)
        {
        }

        public INotificationContentText TextHeading { get { return this.TextFields[0]; } }
        public INotificationContentText TextShortColumn1Row1 { get { return this.TextFields[1]; } }
        public INotificationContentText TextColumn2Row1 { get { return this.TextFields[2]; } }
        public INotificationContentText TextShortColumn1Row2 { get { return this.TextFields[3]; } }
        public INotificationContentText TextColumn2Row2 { get { return this.TextFields[4]; } }
        public INotificationContentText TextShortColumn1Row3 { get { return this.TextFields[5]; } }
        public INotificationContentText TextColumn2Row3 { get { return this.TextFields[6]; } }
        public INotificationContentText TextShortColumn1Row4 { get { return this.TextFields[7]; } }
        public INotificationContentText TextColumn2Row4 { get { return this.TextFields[8]; } }
    }

    internal class TileWideText08 : TileWideBase, ITileWideText08
    {
        public TileWideText08() : base(templateName: "TileWideText08", imageCount: 0, textCount: 10)
        {
        }

        public INotificationContentText TextShortColumn1Row1 { get { return this.TextFields[0]; } }
        public INotificationContentText TextColumn2Row1 { get { return this.TextFields[1]; } }
        public INotificationContentText TextShortColumn1Row2 { get { return this.TextFields[2]; } }
        public INotificationContentText TextColumn2Row2 { get { return this.TextFields[3]; } }
        public INotificationContentText TextShortColumn1Row3 { get { return this.TextFields[4]; } }
        public INotificationContentText TextColumn2Row3 { get { return this.TextFields[5]; } }
        public INotificationContentText TextShortColumn1Row4 { get { return this.TextFields[6]; } }
        public INotificationContentText TextColumn2Row4 { get { return this.TextFields[7]; } }
        public INotificationContentText TextShortColumn1Row5 { get { return this.TextFields[8]; } }
        public INotificationContentText TextColumn2Row5 { get { return this.TextFields[9]; } }
    }

    internal class TileWideText09 : TileWideBase, ITileWideText09
    {
        public TileWideText09() : base(templateName: "TileWideText09", imageCount: 0, textCount: 2)
        {
        }

        public INotificationContentText TextHeading { get { return this.TextFields[0]; } }
        public INotificationContentText TextBodyWrap { get { return this.TextFields[1]; } }
    }

    internal class TileWideText10 : TileWideBase, ITileWideText10
    {
        public TileWideText10() : base(templateName: "TileWideText10", imageCount: 0, textCount: 9)
        {
        }

        public INotificationContentText TextHeading { get { return this.TextFields[0]; } }
        public INotificationContentText TextPrefixColumn1Row1 { get { return this.TextFields[1]; } }
        public INotificationContentText TextColumn2Row1 { get { return this.TextFields[2]; } }
        public INotificationContentText TextPrefixColumn1Row2 { get { return this.TextFields[3]; } }
        public INotificationContentText TextColumn2Row2 { get { return this.TextFields[4]; } }
        public INotificationContentText TextPrefixColumn1Row3 { get { return this.TextFields[5]; } }
        public INotificationContentText TextColumn2Row3 { get { return this.TextFields[6]; } }
        public INotificationContentText TextPrefixColumn1Row4 { get { return this.TextFields[7]; } }
        public INotificationContentText TextColumn2Row4 { get { return this.TextFields[8]; } }
    }

    internal class TileWideText11 : TileWideBase, ITileWideText11
    {
        public TileWideText11() : base(templateName: "TileWideText11", imageCount: 0, textCount: 10)
        {
        }

        public INotificationContentText TextPrefixColumn1Row1 { get { return this.TextFields[0]; } }
        public INotificationContentText TextColumn2Row1 { get { return this.TextFields[1]; } }
        public INotificationContentText TextPrefixColumn1Row2 { get { return this.TextFields[2]; } }
        public INotificationContentText TextColumn2Row2 { get { return this.TextFields[3]; } }
        public INotificationContentText TextPrefixColumn1Row3 { get { return this.TextFields[4]; } }
        public INotificationContentText TextColumn2Row3 { get { return this.TextFields[5]; } }
        public INotificationContentText TextPrefixColumn1Row4 { get { return this.TextFields[6]; } }
        public INotificationContentText TextColumn2Row4 { get { return this.TextFields[7]; } }
        public INotificationContentText TextPrefixColumn1Row5 { get { return this.TextFields[8]; } }
        public INotificationContentText TextColumn2Row5 { get { return this.TextFields[9]; } }
    }
}
