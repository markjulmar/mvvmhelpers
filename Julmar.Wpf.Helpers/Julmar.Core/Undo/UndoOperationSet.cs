using System.Collections.Generic;
using System.Linq;
using JulMar.Core.Interfaces;

namespace JulMar.Core.Undo
{
    /// <summary>
    /// A single undo/redo operation "set" that is executed together.
    /// </summary>
    public class UndoOperationSet : ISupportUndo
    {
        private readonly List<ISupportUndo> _undoStack = new List<ISupportUndo>();

        /// <summary>
        /// Default constructor
        /// </summary>
        public UndoOperationSet()
        {
        }

        /// <summary>
        /// Constructor that takes a list of items to add
        /// </summary>
        /// <param name="ops">Operations</param>
        public UndoOperationSet(IEnumerable<ISupportUndo> ops)
        {
            _undoStack.AddRange(ops);
        }

        /// <summary>
        /// Adds a new undo operation to the collection
        /// </summary>
        /// <param name="operation">Operation</param>
        public void Add(ISupportUndo operation)
        {
            _undoStack.Add(operation);
        }

        /// <summary>
        /// Adds a range of operations
        /// </summary>
        /// <param name="ops">Operations to add</param>
        public void AddRange(IEnumerable<ISupportUndo> ops)
        {
            _undoStack.AddRange(ops);
        }

        #region ISupportUndo Members

        /// <summary>
        /// Method used to undo operation
        /// </summary>
        public void Undo()
        {
            for (int index = _undoStack.Count - 1; index >= 0; index--)
            {
                var operation = _undoStack[index];
                operation.Undo();
            }
        }

        /// <summary>
        /// True if operation can be "reapplied" after undo.
        /// </summary>
        public bool CanRedo
        {
            get { return _undoStack.All(op => op.CanRedo); }
        }

        /// <summary>
        /// Method to redo operation
        /// </summary>
        public void Redo()
        {
            if (CanRedo)
            {
                _undoStack.ForEach(op => op.Redo());
            }
        }

        #endregion
    }
}
