using System;
using System.Windows;
using System.Windows.Controls;

namespace JulMar.Interactivity
{
    /// <summary>
    /// EventArgs for our drag/drop
    /// </summary>
    public class DragDropEventArgs : EventArgs
    {
        /// <summary>
        /// Source where the item is currently located
        /// </summary>
        public ItemsControl Source { get; private set; }

        /// <summary>
        /// Destination where item is being dropped (may be null)
        /// </summary>
        public ItemsControl Destination { get; private set; }

        /// <summary>
        /// Object being copied or moved
        /// </summary>
        public object Item { get; private set; }

        /// <summary>
        /// Position we are dropping item onto
        /// </summary>
        public int DropIndex { get; private set; }

        /// <summary>
        /// Set to true to disallow drag/drop operation
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Initially holds allowed effects, can be modified to restrict
        /// operations
        /// </summary>
        public DragDropEffects AllowedEffects { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="item"></param>
        /// <param name="dropIndex"></param>
        internal DragDropEventArgs(ItemsControl source, ItemsControl destination, object item, int dropIndex = -1)
        {
            this.Source = source;
            this.Destination = destination;
            this.Item = item;
            this.DropIndex = dropIndex;
        }
    }
}