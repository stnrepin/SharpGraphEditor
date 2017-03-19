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
            System.Diagnostics.Debug.WriteLine("REDO: " + _index.ToString() + " " + _operations.Count);
        }

        public void Undo()
        {
            if (!CanUndo)
            {
                return;
            }

            _operations.ElementAtOrDefault(_index).Undo();
            _index--;
            System.Diagnostics.Debug.WriteLine("UNDO: " + _index.ToString() + " " + _operations.Count);

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
            System.Diagnostics.Debug.WriteLine("ADDOPERATION: " + _index.ToString() + " " + _operations.Count);
        }

        public void RemoveLast()
        {
            _operations.RemoveAt(_operations.Count - 1);
            _index--;
            System.Diagnostics.Debug.WriteLine("REMOVE: " + _index.ToString() + " " + _operations.Count);
        }

        public void Clear()
        {
            _operations.Clear();
            _index = -1;
        }

        private void CutOff()
        {
            int index = _index + 1;
            if (index < _operations.Count)
                _operations.RemoveRange(index, _operations.Count - index);
        }
    }
}
