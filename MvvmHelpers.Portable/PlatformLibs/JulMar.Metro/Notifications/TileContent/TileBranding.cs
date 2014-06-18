namespace JulMar.Notifications.TileContent
{
    /// <summary>
    /// The types of behavior that can be used for application branding when
    /// tile notification content is displayed on the tile.
    /// </summary>
    public enum TileBranding
    {
        /// <summary>
        /// No application branding will be displayed on the tile content.
        /// </summary>
        None = 0,
        /// <summary>
        /// The application logo will be displayed with the tile content.
        /// </summary>
        Logo,
        /// <summary>
        /// The application name will be displayed with the tile content.
        /// </summary>
        Name
    }
}