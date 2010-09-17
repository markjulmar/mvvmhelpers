namespace JulMar.Core.Interfaces
{
    /// <summary>
    /// Interface that describes a single Undo/Redo operation
    /// </summary>
    public interface ISupportUndo
    {
        /// <summary>
        /// Method used to undo operation
        /// </summary>
        void Undo();

        /// <summary>
        /// True if operation can be "reapplied" after undo.
        /// </summary>
        bool CanRedo { get; }

        /// <summary>
        /// Method to redo operation
        /// </summary>
        void Redo();
    }
}