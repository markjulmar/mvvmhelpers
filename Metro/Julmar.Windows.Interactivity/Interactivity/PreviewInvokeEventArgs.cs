namespace System.Windows.Interactivity
{
    /// <summary>
    /// EventArgs passed with PreviewInvoke event
    /// </summary>
    public class PreviewInvokeEventArgs : EventArgs
    {
        /// <summary>
        /// True if the event should be canceled.
        /// </summary>
        public bool Cancelling { get; set; }
    }
}