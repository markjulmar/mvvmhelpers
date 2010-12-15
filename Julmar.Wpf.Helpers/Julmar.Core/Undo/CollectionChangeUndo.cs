using System.Collections;
using JulMar.Core.Interfaces;

namespace JulMar.Core.Undo
{
    /// <summary>
    /// This describes the specific change types for a collection
    /// </summary>
    public enum CollectionChangeType
    {
        /// <summary>
        /// Item has been added to collection
        /// </summary>
        Add, 
        /// <summary>
        /// Item has been removed from collection
        /// </summary>
        Remove, 
        /// <summary>
        /// Item has been replaced within collection
        /// </summary>
        Replace, 
        /// <summary>
        /// Item has been moved inside collection
        /// </summary>
        Move
    }

    /// <summary>
    /// This class implements the undo/redo support for collection changes.
    /// A single instance can undo one operation performed to a collection.
    /// The collection must implement IList.
    /// </summary>
    public class CollectionChangeUndo : ISupportUndo
    {
        /// <summary>
        /// Collection we are working with
        /// </summary>
        readonly IList _collection;
        /// <summary>
        /// Change type that has occurred
        /// </summary>
        readonly CollectionChangeType _changeType;
        /// <summary>
        /// Position where change occurred (old)
        /// </summary>
        readonly int _pos;
        /// <summary>
        /// New position where movement/insertion occurred
        /// </summary>
        readonly int _newPos;
        /// <summary>
        /// New value
        /// </summary>
        readonly object _newValue;
        /// <summary>
        /// Old value
        /// </summary>
        readonly object _oldValue;

        /// <summary>
        /// Constructor used for add/delete/replace
        /// </summary>
        /// <param name="coll">Collection to work with</param>
        /// <param name="type">Type of change</param>
        /// <param name="oldPos">Position of change</param>
        /// <param name="newPos">New position of change</param>
        /// <param name="oldValue">Old value at position</param>
        /// <param name="newValue">New value at position</param>
        public CollectionChangeUndo(IList coll, CollectionChangeType type, 
            int oldPos, int newPos, object oldValue, object newValue)
        {
            _collection = coll;
            _changeType = type;
            _pos = oldPos;
            _newPos = newPos;
            _oldValue = oldValue;
            _newValue = newValue;
        }

        /// <summary>
        /// True if operation can be "reapplied" after undo.
        /// </summary>
        public bool CanRedo { get { return true; } }

        /// <summary>
        /// Method used to undo operation
        /// </summary>
        public void Undo()
        {
            switch (_changeType)
            {
                case CollectionChangeType.Add:
                    _collection.Remove(_newValue);
                    break;
                case CollectionChangeType.Remove:
                    if (_pos >= _collection.Count)
                        _collection.Add(_oldValue);
                    else
                        _collection.Insert(_pos, _oldValue);
                    break;
                case CollectionChangeType.Replace:
                    _collection[_pos] = _oldValue;
                    break;
                case CollectionChangeType.Move:
                    _collection.RemoveAt(_newPos);
                    _collection.Insert(_pos, _newValue);
                    break;
            }
        }

        /// <summary>
        /// Method to redo operation
        /// </summary>
        public void Redo()
        {
            switch (_changeType)
            {
                case CollectionChangeType.Remove:
                    _collection.Remove(_oldValue);
                    break;
                case CollectionChangeType.Add:
                    if (_newPos == -1 || _newPos >= _collection.Count)
                        _collection.Add(_newValue);
                    else
                        _collection.Insert(_newPos, _newValue);
                    break;
                case CollectionChangeType.Replace:
                    _collection[_pos] = _newValue;
                    break;
                case CollectionChangeType.Move:
                    _collection.RemoveAt(_pos);
                    if (_newPos == -1 || _newPos >= _collection.Count)
                        _collection.Add(_newValue);
                    else
                        _collection.Insert(_newPos, _newValue);
                    break;
            }
        }
    }
}