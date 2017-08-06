using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGraphEditor.Models
{
    public class UndoRedoManager
    {
        private List<IOperation> _operations;
        private int _index;

        public bool CanUndo => _index >= 0;
        public bool CanRedo => _operations.Count > 0 && _index < _operations.Count - 1;
        public int Position => _index;
        public int OperationsCount => _operations.Count;

        public UndoRedoManager()
        {
            _operations = new List<IOperation>();
            _index = -1;
        }

        public void Redo()
        {
            if (!CanRedo)
            {
                return;
            }

            _operations.ElementAtOrDefault(++_index)?.Redo();
        }

        public void Undo()
        {
            if (!CanUndo)
            {
                return;
            }

            _operations.ElementAtOrDefault(_index).Undo();
            _index--;

        }

        public void Add(IOperation operation)
        {
            CutOff();
            _operations.Add(operation);
        }

        public void AddAndExecute(IOperation operation)
        {
            Add(operation);
            operation.Redo();
            _index++;
        }

        public void AppendLast(IOperation operation)
        {
            CutOff();
            var lastOp = _operations.LastOrDefault();
            lastOp?.Append(operation);
        }

        public void AppendLastAndExecute(IOperation operation)
        {
            AppendLast(operation);
            operation.Redo();
        }

        public void RemoveLast()
        {
            _operations.RemoveAt(_operations.Count - 1);
            _index--;
        }

        public void Clear()
        {
            _operations.Clear();
            _index = -1;
        }

        public void CutOff()
        {
            int index = _index + 1;
            if (index < _operations.Count)
            {
                _operations.RemoveRange(index, _operations.Count - index);
            }
        }
    }
}
