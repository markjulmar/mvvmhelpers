using Windows.UI.Notifications;

namespace JulMar.Windows.UI.Notifications.TileContent
{
    /// <summary>
    /// Base tile notification content interface.
    /// </summary>
    public interface ITileNotificationContent : INotificationContent
    {
        /// <summary>
        /// Whether strict validation should be applied when the Xml or notification object is created,
        /// and when some of the properties are assigned.
        /// </summary>
        bool StrictValidation { get; set; }

        /// <summary>
        /// The language of the content being displayed.  The language should be specified using the
        /// abbreviated language code as defined by BCP 47.
        /// </summary>
        string Lang { get; set; }

        /// <summary>
        /// The BaseUri that should be used for image locations.  Relative image locations use this
        /// field as their base Uri.  The BaseUri must begin with http://, https://, ms-appx:///, or 
        /// ms-appdata:///local/.
        /// </summary>
        string BaseUri { get; set; }

        /// <summary>
        /// Determines the application branding when tile notification content is displayed on the tile.
        /// </summary>
        TileBranding Branding { get; set; }

        /// <summary>
        /// Controls if query strings that denote the client configuration of contrast, scale, and language setting should be appended to the Src
        /// If true, Windows will append query strings onto images that exist in this template
        /// If false (the default), Windows will not append query strings onto images that exist in this template
        /// Query string details:
        /// Parameter: ms-contrast
        ///     Values: standard, black, white
        /// Parameter: ms-scale
        ///     Values: 80, 100, 140, 180
        /// Parameter: ms-lang
        ///     Values: The BCP 47 language tag set in the notification xml, or if omitted, the current preferred language of the user
        /// </summary>
        bool AddImageQuery { get; set; }

        /// <summary>
        /// Creates a WinRT TileNotification object based on the content.
        /// </summary>
        /// <returns>The WinRT TileNotification object</returns>
        TileNotification CreateNotification();
    }

    /// <summary>
    /// Base square tile notification content interface.
    /// </summary>
    public interface ISquareTileNotificationContent : ITileNotificationContent
    {
    }

    /// <summary>
    /// Base wide tile notification content interface.
    /// </summary>
    public interface IWideTileNotificationContent : ITileNotificationContent
    {
        /// <summary>
        /// Corresponding square tile notification content should be a part of every wide tile notification.
        /// </summary>
        ISquareTileNotificationContent SquareContent { get; set; }

        /// <summary>
        /// Whether square tile notification content needs to be added to pass
        /// validation.  Square content is required by default.
        /// </summary>
        bool RequireSquareContent { get; set; }
    }

    /// <summary>
    /// A square tile template that displays two text captions.
    /// </summary>
    public interface ITileSquareBlock : ISquareTileNotificationContent
    {
        /// <summary>
        /// A large block text field.
        /// </summary>
        INotificationContentText TextBlock { get; }

        /// <summary>
        /// The description under the large block text field.
        /// </summary>
        INotificationContentText TextSubBlock { get; }
    }

    /// <summary>
    /// A square tile template that displays an image.
    /// </summary>
    public interface ITileSquareImage : ISquareTileNotificationContent
    {
        /// <summary>
        /// The main image on the tile.
        /// </summary>
        INotificationContentImage Image { get; }
    }

    /// <summary>
    /// A square tile template that displays an image, then transitions to show
    /// four text fields.
    /// </summary>
    public interface ITileSquarePeekImageAndText01 : ISquareTileNotificationContent
    {
        /// <summary>
        /// The main image on the tile.
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

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody3 { get; }
    }

    /// <summary>
    /// A square tile template that displays an image, then transitions to show
    /// two text fields.
    /// </summary>
    public interface ITileSquarePeekImageAndText02 : ISquareTileNotificationContent
    {
        /// <summary>
        /// The main image on the tile.
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
    /// A square tile template that displays an image, then transitions to show
    /// four text fields.
    /// </summary>
    public interface ITileSquarePeekImageAndText03 : ISquareTileNotificationContent
    {
        /// <summary>
        /// The main image on the tile.
        /// </summary>
        INotificationContentImage Image { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody1 { get; }
        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody2 { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody3 { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody4 { get; }
    }

    /// <summary>
    /// A square tile template that displays an image, then transitions to 
    /// show a text field.
    /// </summary>
    public interface ITileSquarePeekImageAndText04 : ISquareTileNotificationContent
    {
        /// <summary>
        /// The main image on the tile.
        /// </summary>
        INotificationContentImage Image { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBodyWrap { get; }
    }

    /// <summary>
    /// A square tile template that displays four text fields.
    /// </summary>
    public interface ITileSquareText01 : ISquareTileNotificationContent
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

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody3 { get; }
    }

    /// <summary>
    /// A square tile template that displays two text fields.
    /// </summary>
    public interface ITileSquareText02 : ISquareTileNotificationContent
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
    /// A square tile template that displays four text fields.
    /// </summary>
    public interface ITileSquareText03 : ISquareTileNotificationContent
    {
        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody1 { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody2 { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody3 { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody4 { get; }
    }

    /// <summary>
    /// A square tile template that displays a text field.
    /// </summary>
    public interface ITileSquareText04 : ISquareTileNotificationContent
    {
        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBodyWrap { get; }
    }

    /// <summary>
    /// A square tile template that displays six text fields.
    /// </summary>
    public interface ITileWideBlockAndText01 : IWideTileNotificationContent
    {
        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody1 { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody2 { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody3 { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody4 { get; }

        /// <summary>
        /// A large block text field.
        /// </summary>
        INotificationContentText TextBlock { get; }

        /// <summary>
        /// The description under the large block text field.
        /// </summary>
        INotificationContentText TextSubBlock { get; }
    }

    /// <summary>
    /// A square tile template that displays three text fields.
    /// </summary>
    public interface ITileWideBlockAndText02 : IWideTileNotificationContent
    {
        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBodyWrap { get; }

        /// <summary>
        /// A large block text field.
        /// </summary>
        INotificationContentText TextBlock { get; }

        /// <summary>
        /// The description under the large block text field.
        /// </summary>
        INotificationContentText TextSubBlock { get; }
    }

    /// <summary>
    /// A wide tile template that displays an image.
    /// </summary>
    public interface ITileWideImage : IWideTileNotificationContent
    {
        /// <summary>
        /// The main image on the tile.
        /// </summary>
        INotificationContentImage Image { get; }
    }

    /// <summary>
    /// A wide tile template that displays an image and a text caption.
    /// </summary>
    public interface ITileWideImageAndText01 : IWideTileNotificationContent
    {
        /// <summary>
        /// The main image on the tile.
        /// </summary>
        INotificationContentImage Image { get; }

        /// <summary>
        /// A caption for the image.
        /// </summary>
        INotificationContentText TextCaptionWrap { get; }
    }

    /// <summary>
    /// A wide tile template that displays an image and two text captions.
    /// </summary>
    public interface ITileWideImageAndText02 : IWideTileNotificationContent
    {
        /// <summary>
        /// The main image on the tile.
        /// </summary>
        INotificationContentImage Image { get; }

        /// <summary>
        /// The first caption for the image.
        /// </summary>
        INotificationContentText TextCaption1 { get; }

        /// <summary>
        /// The second caption for the image.
        /// </summary>
        INotificationContentText TextCaption2 { get; }
    }

    /// <summary>
    /// A wide tile template that displays five images - one main image,
    /// and four square images in a grid.
    /// </summary>
    public interface ITileWideImageCollection : IWideTileNotificationContent
    {
        /// <summary>
        /// The main image on the tile.
        /// </summary>
        INotificationContentImage ImageMain { get; }

        /// <summary>
        /// A small square image on the tile.
        /// </summary>
        INotificationContentImage ImageSmallColumn1Row1 { get; }

        /// <summary>
        /// A small square image on the tile.
        /// </summary>
        INotificationContentImage ImageSmallColumn2Row1 { get; }

        /// <summary>
        /// A small square image on the tile.
        /// </summary>
        INotificationContentImage ImageSmallColumn1Row2 { get; }

        /// <summary>
        /// A small square image on the tile.
        /// </summary>
        INotificationContentImage ImageSmallColumn2Row2 { get; }
    }

    /// <summary>
    /// A wide tile template that displays an image, then transitions to show
    /// two text fields.
    /// </summary>
    public interface ITileWidePeekImage01 : IWideTileNotificationContent
    {
        /// <summary>
        /// The main image on the tile.
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
    /// A wide tile template that displays an image, then transitions to show
    /// five text fields.
    /// </summary>
    public interface ITileWidePeekImage02 : IWideTileNotificationContent
    {
        /// <summary>
        /// The main image on the tile.
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

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody3 { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody4 { get; }
    }

    /// <summary>
    /// A wide tile template that displays an image, then transitions to show
    /// a text field.
    /// </summary>
    public interface ITileWidePeekImage03 : IWideTileNotificationContent
    {
        /// <summary>
        /// The main image on the tile.
        /// </summary>
        INotificationContentImage Image { get; }

        /// <summary>
        /// A heading text field.
        /// </summary>
        INotificationContentText TextHeadingWrap { get; }
    }

    /// <summary>
    /// A wide tile template that displays an image, then transitions to show
    /// a text field.
    /// </summary>
    public interface ITileWidePeekImage04 : IWideTileNotificationContent
    {
        /// <summary>
        /// The main image on the tile.
        /// </summary>
        INotificationContentImage Image { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBodyWrap { get; }
    }

    /// <summary>
    /// A wide tile template that displays an image, then transitions to show
    /// another image and two text fields.
    /// </summary>
    public interface ITileWidePeekImage05 : IWideTileNotificationContent
    {
        /// <summary>
        /// The main image on the tile.
        /// </summary>
        INotificationContentImage ImageMain { get; }

        /// <summary>
        /// The secondary image on the tile.
        /// </summary>
        INotificationContentImage ImageSecondary { get; }

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
    /// A wide tile template that displays an image, then transitions to show
    /// another image and a text field.
    /// </summary>
    public interface ITileWidePeekImage06 : IWideTileNotificationContent
    {
        /// <summary>
        /// The main image on the tile.
        /// </summary>
        INotificationContentImage ImageMain { get; }

        /// <summary>
        /// The secondary image on the tile.
        /// </summary>
        INotificationContentImage ImageSecondary { get; }

        /// <summary>
        /// A heading text field.
        /// </summary>
        INotificationContentText TextHeadingWrap { get; }
    }

    /// <summary>
    /// A wide tile template that displays an image and a portion of a text field,
    /// then transitions to show all of the text field.
    /// </summary>
    public interface ITileWidePeekImageAndText01 : IWideTileNotificationContent
    {
        /// <summary>
        /// The main image on the tile.
        /// </summary>
        INotificationContentImage Image { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBodyWrap { get; }
    }

    /// <summary>
    /// A wide tile template that displays an image and a text field,
    /// then transitions to show the text field and four other text fields.
    /// </summary>
    public interface ITileWidePeekImageAndText02 : IWideTileNotificationContent
    {
        /// <summary>
        /// The main image on the tile.
        /// </summary>
        INotificationContentImage Image { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody1 { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody2 { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody3 { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody4 { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody5 { get; }
    }

    /// <summary>
    /// A wide tile template that displays five images - one main image,
    /// and four square images in a grid, then transitions to show two
    /// text fields.
    /// </summary>
    public interface ITileWidePeekImageCollection01 : ITileWideImageCollection
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
    /// A wide tile template that displays five images - one main image,
    /// and four square images in a grid, then transitions to show five
    /// text fields.
    /// </summary>
    public interface ITileWidePeekImageCollection02 : ITileWideImageCollection
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

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody3 { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody4 { get; }
    }

    /// <summary>
    /// A wide tile template that displays five images - one main image,
    /// and four square images in a grid, then transitions to show a
    /// text field.
    /// </summary>
    public interface ITileWidePeekImageCollection03 : ITileWideImageCollection
    {
        /// <summary>
        /// A heading text field.
        /// </summary>
        INotificationContentText TextHeadingWrap { get; }
    }

    /// <summary>
    /// A wide tile template that displays five images - one main image,
    /// and four square images in a grid, then transitions to show a
    /// text field.
    /// </summary>
    public interface ITileWidePeekImageCollection04 : ITileWideImageCollection
    {
        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBodyWrap { get; }
    }

    /// <summary>
    /// A wide tile template that displays five images - one main image,
    /// and four square images in a grid, then transitions to show an image
    /// and two text fields.
    /// </summary>
    public interface ITileWidePeekImageCollection05 : ITileWideImageCollection
    {
        /// <summary>
        /// The secondary image on the tile.
        /// </summary>
        INotificationContentImage ImageSecondary { get; }

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
    /// A wide tile template that displays five images - one main image,
    /// and four square images in a grid, then transitions to show an image
    /// and a text field.
    /// </summary>
    public interface ITileWidePeekImageCollection06 : ITileWideImageCollection
    {
        /// <summary>
        /// The secondary image on the tile.
        /// </summary>
        INotificationContentImage ImageSecondary { get; }

        /// <summary>
        /// A heading text field.
        /// </summary>
        INotificationContentText TextHeadingWrap { get; }
    }

    /// <summary>
    /// A wide tile template that displays an image and a text field.
    /// </summary>
    public interface ITileWideSmallImageAndText01 : IWideTileNotificationContent
    {
        /// <summary>
        /// The main image on the tile.
        /// </summary>
        INotificationContentImage Image { get; }

        /// <summary>
        /// A heading text field.
        /// </summary>
        INotificationContentText TextHeadingWrap { get; }
    }

    /// <summary>
    /// A wide tile template that displays an image and 5 text fields.
    /// </summary>
    public interface ITileWideSmallImageAndText02 : IWideTileNotificationContent
    {
        /// <summary>
        /// The main image on the tile.
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

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody3 { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody4 { get; }
    }

    /// <summary>
    /// A wide tile template that displays an image and a text field.
    /// </summary>
    public interface ITileWideSmallImageAndText03 : IWideTileNotificationContent
    {
        /// <summary>
        /// The main image on the tile.
        /// </summary>
        INotificationContentImage Image { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBodyWrap { get; }
    }

    /// <summary>
    /// A wide tile template that displays an image and two text fields.
    /// </summary>
    public interface ITileWideSmallImageAndText04 : IWideTileNotificationContent
    {
        /// <summary>
        /// The main image on the tile.
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
    /// A wide tile template that displays a poster image and two text fields.
    /// </summary>
    public interface ITileWideSmallImageAndText05 : IWideTileNotificationContent
    {
        /// <summary>
        /// The main image on the tile.
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
    /// A wide tile template that displays five text fields.
    /// </summary>
    public interface ITileWideText01 : IWideTileNotificationContent
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

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody3 { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody4 { get; }
    }

    /// <summary>
    /// A wide tile template that displays nine text fields - a heading and two columns
    /// of four text fields.
    /// </summary>
    public interface ITileWideText02 : IWideTileNotificationContent
    {
        /// <summary>
        /// A heading text field.
        /// </summary>
        INotificationContentText TextHeading { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn1Row1 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn2Row1 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn1Row2 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn2Row2 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn1Row3 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn2Row3 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn1Row4 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn2Row4 { get; }
    }

    /// <summary>
    /// A wide tile template that displays a text field.
    /// </summary>
    public interface ITileWideText03 : IWideTileNotificationContent
    {
        /// <summary>
        /// A heading text field.
        /// </summary>
        INotificationContentText TextHeadingWrap { get; }
    }

    /// <summary>
    /// A wide tile template that displays a text field.
    /// </summary>
    public interface ITileWideText04 : IWideTileNotificationContent
    {
        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBodyWrap { get; }
    }

    /// <summary>
    /// A wide tile template that displays five text fields.
    /// </summary>
    public interface ITileWideText05 : IWideTileNotificationContent
    {
        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody1 { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody2 { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody3 { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody4 { get; }

        /// <summary>
        /// A body text field.
        /// </summary>
        INotificationContentText TextBody5 { get; }
    }

    /// <summary>
    /// A wide tile template that displays ten text fields - two columns
    /// of five text fields.
    /// </summary>
    public interface ITileWideText06 : IWideTileNotificationContent
    {
        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn1Row1 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn2Row1 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn1Row2 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn2Row2 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn1Row3 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn2Row3 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn1Row4 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn2Row4 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn1Row5 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn2Row5 { get; }
    }

    /// <summary>
    /// A wide tile template that displays nine text fields - a heading and two columns
    /// of four text fields.
    /// </summary>
    public interface ITileWideText07 : IWideTileNotificationContent
    {
        /// <summary>
        /// A heading text field.
        /// </summary>
        INotificationContentText TextHeading { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextShortColumn1Row1 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn2Row1 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextShortColumn1Row2 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn2Row2 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextShortColumn1Row3 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn2Row3 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextShortColumn1Row4 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn2Row4 { get; }
    }

    /// <summary>
    /// A wide tile template that displays ten text fields - two columns
    /// of five text fields.
    /// </summary>
    public interface ITileWideText08 : IWideTileNotificationContent
    {
        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextShortColumn1Row1 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextShortColumn1Row2 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextShortColumn1Row3 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextShortColumn1Row4 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextShortColumn1Row5 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn2Row1 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn2Row2 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn2Row3 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn2Row4 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn2Row5 { get; }
    }

    /// <summary>
    /// A wide tile template that displays two text fields.
    /// </summary>
    public interface ITileWideText09 : IWideTileNotificationContent
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
    /// A wide tile template that displays nine text fields - a heading and two columns
    /// of four text fields.
    /// </summary>
    public interface ITileWideText10 : IWideTileNotificationContent
    {
        /// <summary>
        /// A heading text field.
        /// </summary>
        INotificationContentText TextHeading { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextPrefixColumn1Row1 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn2Row1 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextPrefixColumn1Row2 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn2Row2 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextPrefixColumn1Row3 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn2Row3 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextPrefixColumn1Row4 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn2Row4 { get; }
    }

    /// <summary>
    /// A wide tile template that displays ten text fields - two columns
    /// of five text fields.
    /// </summary>
    public interface ITileWideText11 : IWideTileNotificationContent
    {
        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextPrefixColumn1Row1 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn2Row1 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextPrefixColumn1Row2 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn2Row2 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextPrefixColumn1Row3 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn2Row3 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextPrefixColumn1Row4 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn2Row4 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextPrefixColumn1Row5 { get; }

        /// <summary>
        /// A text field displayed in a column and row.
        /// </summary>
        INotificationContentText TextColumn2Row5 { get; }
    }
}
