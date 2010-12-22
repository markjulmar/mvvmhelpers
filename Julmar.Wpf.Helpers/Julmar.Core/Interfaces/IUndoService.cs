using System.ComponentModel.Composition;

namespace JulMar.Core.Interfaces
{
    /// <summary>
    /// This interface describes a simple Undo service
    /// </summary>
    [InheritedExport]
    public interface IUndoService
    {
        /// <summary>
        /// Returns true if we have at least one UNDO operation we can perform
        /// </summary>
        bool CanUndo { get; }

        /// <summary>
        /// True if at least one REDO operation is available.
        /// </summary>
        bool CanRedo { get; }

        /// <summary>
        /// Executes the next UNDO operation.
        /// </summary>
        /// <returns>True if an undo was executed</returns>
        bool Undo();

        /// <summary>
        /// Executes the last REDO operation.
        /// </summary>
        /// <returns>True if a REDO operation occurred</returns>
        bool Redo();

        /// <summary>
        /// Adds a new undo operation to the stack.
        /// </summary>
        /// <param name="undoOp">operation</param>
        /// <param name="noInsertIfExecutingOperation">Do not insert record if currently running undo/redo</param>
        /// <returns>True if record inserted, false if not</returns>
        bool Add(ISupportUndo undoOp, bool noInsertIfExecutingOperation = true);

        /// <summary>
        /// Clears all the undo/redo events.  This should be used if some
        /// action makes the operations invalid (clearing a collection where you are tracking changes to indexes inside it for example)
        /// </summary>
        void Clear();
    }
}