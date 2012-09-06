using System;
using System.Collections.Generic;
using JulMar.Core.Interfaces;
using System.ComponentModel.Composition;

namespace JulMar.Core.Undo
{
    /// <summary>
    /// Class used for Undo/Redo support.
    /// </summary>
    [Export(typeof(IUndoService))]
    public class UndoManager : IUndoService
    {
        private const int DefaultMaxCount = 100;

        #region Private Data
        private readonly object _lock = new object();
        private int _maxSupportedOperations;
        private bool _isUndoingOperation;
        private readonly LinkedList<ISupportUndo> _undoOperations = new LinkedList<ISupportUndo>();
        private readonly LinkedList<ISupportUndo> _redoOperations = new LinkedList<ISupportUndo>();
        #endregion

        /// <summary>
        /// Default constructor (used by MEF)
        /// </summary>
        public UndoManager()
            : this(DefaultMaxCount)
        {
        }

        /// <summary>
        /// Constructor (if used directly)
        /// </summary>
        public UndoManager(int maxSupported)
        {
            MaxSupportedOperations = maxSupported;
        }

        /// <summary>
        /// Maximum number of supported operations.
        /// </summary>
        public int MaxSupportedOperations
        {
            get { return _maxSupportedOperations; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("MaxSupportedOperation", "MaxSupportedOperations must be greater than zero.");

                if (_maxSupportedOperations != value)
                {
                    _maxSupportedOperations = value;
                    lock (_lock)
                    {
                        while (_undoOperations.Count > _maxSupportedOperations)
                            _undoOperations.RemoveLast();
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if we have at least one UNDO operation we can perform
        /// </summary>
        public bool CanUndo
        {
            get
            {
                lock (_lock)
                {
                    return !_isUndoingOperation && _undoOperations.Count > 0;
                }
            }
        }

        /// <summary>
        /// True if at least one REDO operation is available.
        /// </summary>
        public bool CanRedo
        {
            get
            {
                lock (_lock)
                {
                    return !_isUndoingOperation && _redoOperations.Count > 0;
                }
            }
        }

        /// <summary>
        /// Executes the next UNDO operation.
        /// </summary>
        /// <returns>True if an undo was executed</returns>
        public bool Undo()
        {
            if (CanUndo)
            {
                ISupportUndo undo = null;

                lock (_lock)
                {
                    if (_undoOperations.Count > 0)
                    {
                        _isUndoingOperation = true;
                        undo = _undoOperations.First.Value;
                        _undoOperations.RemoveFirst();
                    }
                }

                if (undo != null)
                {
                    try
                    {
                        undo.Undo();
                        if (undo.CanRedo)
                        {
                            lock (_lock)
                            {
                                _redoOperations.AddFirst(undo);
                                while (_redoOperations.Count > _maxSupportedOperations)
                                    _redoOperations.RemoveLast();
                            }
                        }
                        return true;
                    }
                    finally
                    {
                        lock (_lock)
                        {
                            _isUndoingOperation = false;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Executes the last REDO operation.
        /// </summary>
        /// <returns>True if a REDO operation occurred</returns>
        public bool Redo()
        {
            if (CanRedo)
            {
                ISupportUndo undo = null;

                lock (_lock)
                {
                    if (_redoOperations.Count > 0)
                    {
                        _isUndoingOperation = true;
                        undo = _redoOperations.First.Value;
                        _redoOperations.RemoveFirst();
                    }
                }

                if (undo != null && undo.CanRedo)
                {
                    try
                    {
                        undo.Redo();
                        lock (_lock)
                        {
                            _undoOperations.AddFirst(undo);
                            while (_undoOperations.Count > _maxSupportedOperations)
                                _undoOperations.RemoveLast();
                        }
                        return true;
                    }
                    finally
                    {
                        lock (_lock)
                        {
                            _isUndoingOperation = false;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Adds a new undo operation to the stack.
        /// </summary>
        /// <param name="undoOp">operation</param>
        /// <param name="noInsertIfInUndoOperation">True if the insertion should not occur if in an UNDO operation</param>
        /// <returns>True if undo operation was added to stack</returns>
        public bool Add(ISupportUndo undoOp, bool noInsertIfInUndoOperation=true)
        {
            if (undoOp == null)
                throw new ArgumentNullException("undoOp");

            if (noInsertIfInUndoOperation && _isUndoingOperation)
                return false;

            lock (_lock)
            {
                _undoOperations.AddFirst(undoOp);
                while (_undoOperations.Count > MaxSupportedOperations)
                    _undoOperations.RemoveLast();
            }

            return true;
        }

        /// <summary>
        /// Clears all the undo/redo events.  This should be used if some
        /// action makes the operations invalid (clearing a collection for example)
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _undoOperations.Clear();
                _redoOperations.Clear();
                _isUndoingOperation = false;
            }
        }
    }
}
