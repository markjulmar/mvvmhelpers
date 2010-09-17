using System;
using JulMar.Core.Interfaces;

namespace JulMar.Core.Undo
{
    /// <summary>
    /// This class implements a generic undo using delegates.
    /// </summary>
    public class DelegateUndo : ISupportUndo
    {
        private readonly Action _undoAction, _redoAction;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="undoAction">Action for undo</param>
        /// <param name="redoAction">Optional action for redo</param>
        public DelegateUndo(Action undoAction, Action redoAction = null)
        {
            _undoAction = undoAction;
            _redoAction = redoAction;
        }

        /// <summary>
        /// Method used to undo operation
        /// </summary>
        public void Undo()
        {
            _undoAction.Invoke();
        }

        /// <summary>
        /// True if operation can be "reapplied" after undo.
        /// </summary>
        public bool CanRedo
        {
            get { return _redoAction != null; }
        }

        /// <summary>
        /// Method to redo operation
        /// </summary>
        public void Redo()
        {
            if (_redoAction != null)
                _redoAction.Invoke();
        }
    }
}
